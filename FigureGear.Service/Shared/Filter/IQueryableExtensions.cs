using FigureGear.Service.Shared.Filter.Model;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FigureGear.Service.Shared.Filter
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> ApplyAdvancedFilterAsync<T>( this IQueryable<T> query,PagedFilterRequest request) where T : class
        {
            // --- Apply FilterText ---
            if (!string.IsNullOrWhiteSpace(request.FilterText))
            {
                var filters = ParseFilters(request.FilterText);
                foreach (var f in filters)
                {
                    query = query.Where(BuildFilterExpression<T>(f.Field, f.Operator, f.Value));
                }
            }

            // --- Apply SearchGlobal (hỗ trợ nested property & DateTime) ---
            if (!string.IsNullOrWhiteSpace(request.SearchGlobal))
            {
                var search = ParseSearchGlobal(request.SearchGlobal);
                if (search.Fields.Any())
                {
                    var param = Expression.Parameter(typeof(T), "x");
                    Expression? body = null;

                    foreach (var field in search.Fields)
                    {
                        Expression prop = param;
                        foreach (var part in field.Split('.'))
                        {
                            prop = Expression.PropertyOrField(prop, part);
                        }

                        Expression expr = BuildGlobalSearchExpression(prop, search.Value, prop.Type, search.Operator);

                        body = body == null ? expr : Expression.OrElse(body, expr);
                    }

                    if (body != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                        query = query.Where(lambda);
                    }
                }
            }

            // --- Apply Sorting (Sort = "Field.Desc" hoặc "Field.Asc") ---
            if (!string.IsNullOrWhiteSpace(request.Sort))
            {
                var parts = request.Sort.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var field = parts[0];
                var descending = parts.Length > 1 && parts[1].Equals("Desc", StringComparison.OrdinalIgnoreCase);

                query = descending ? query.OrderByDescendingDynamic(field) : query.OrderByDynamic(field);
            }

            // --- Apply Paging ---
            var total = query.Count();
            var data = query.Skip((request.Page - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .ToList();

            return new PagedResult<T>(data, total, request.Page, request.PageSize);
        }

        #region Helpers

        private static List<(string Field, string Operator, string Value)> ParseFilters(string filterText)
        {
            var filters = new List<(string, string, string)>();
            var parts = filterText.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var match = Regex.Match(part, @"^(?<field>[\w\.]+)\[(?<op>\w+)\]""(?<value>.+)""$");
                if (match.Success)
                {
                    filters.Add((
                        match.Groups["field"].Value,
                        match.Groups["op"].Value,
                        match.Groups["value"].Value
                    ));
                }
            }
            return filters;
        }

        private static (List<string> Fields, string Operator, string Value) ParseSearchGlobal(string searchGlobal)
        {
            var match = Regex.Match(searchGlobal, @"^(?<fields>[\w\.,]+)\[(?<op>\w+)\]""(?<value>.+)""$");
            if (!match.Success) return (new List<string>(), "Equal", "");
            return (
                match.Groups["fields"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                match.Groups["op"].Value,
                match.Groups["value"].Value
            );
        }

        private static Expression BuildGlobalSearchExpression(Expression prop, string value, Type type, string op)
        {
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                if (int.TryParse(value, out var number))
                {
                    // Số nguyên: coi như tháng/ ngày/ năm
                    var monthProp = Expression.Property(prop, nameof(DateTime.Month));
                    var dayProp = Expression.Property(prop, nameof(DateTime.Day));
                    var yearProp = Expression.Property(prop, nameof(DateTime.Year));
                    var numberExpr = Expression.Constant(number);

                    return Expression.OrElse(
                        Expression.Equal(monthProp, numberExpr),
                        Expression.OrElse(
                            Expression.Equal(dayProp, numberExpr),
                            Expression.Equal(yearProp, numberExpr)
                        )
                    );
                }
                else if (DateTime.TryParse(value, out var dt))
                {
                    // Ngày đầy đủ
                    var start = dt.Date;
                    var end = dt.Date.AddDays(1);

                    var ge = Expression.GreaterThanOrEqual(prop, Expression.Constant(start, type));
                    var lt = Expression.LessThan(prop, Expression.Constant(end, type));

                    return Expression.AndAlso(ge, lt);
                }
            }

            // --- Default string/number conversion ---
            var constant = Expression.Constant(Convert.ChangeType(value, Nullable.GetUnderlyingType(type) ?? type));

            return op.ToLower() switch
            {
                "equal" => Expression.Equal(prop, constant),
                "notequal" => Expression.NotEqual(prop, constant),
                "greaterthan" => Expression.GreaterThan(prop, constant),
                "lessthan" => Expression.LessThan(prop, constant),
                "contains" or "like" => Expression.Call(prop, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                "startswith" => Expression.Call(prop, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant),
                "endswith" => Expression.Call(prop, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constant),
                _ => throw new NotSupportedException($"Operator {op} is not supported")
            };
        }

        private static Expression<Func<T, bool>> BuildFilterExpression<T>(string field, string op, string value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression prop = param;
            foreach (var part in field.Split('.'))
            {
                prop = Expression.PropertyOrField(prop, part);
            }

            Expression body = BuildGlobalSearchExpression(prop, value, prop.Type, op);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        #endregion
    }

    public static class IQueryableOrderExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, propertyName);
            var lambda = Expression.Lambda(prop, param);

            var method = typeof(Queryable).GetMethods()
                                          .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                                          .MakeGenericMethod(typeof(T), prop.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> query, string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.PropertyOrField(param, propertyName);
            var lambda = Expression.Lambda(prop, param);

            var method = typeof(Queryable).GetMethods()
                                          .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                                          .MakeGenericMethod(typeof(T), prop.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }
    }
}
