using eKonect.Service.Exceptions;
using System.Net;
using System.Text.Json;

namespace eKonect.API.MiddleWares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                string jsonResult;
                switch (error)
                {
                    case FigureGearException e:
                        response.StatusCode = (int)e.StatusCode;
                        jsonResult = JsonSerializer.Serialize(e.ToSerializableObject());
                        break;
                    default:
                        // unhandled error
                        jsonResult = JsonSerializer.Serialize(new
                        {
                            MessageCode = "eKonect_ERROR_UNKNOWN",
                            error?.Message,
                            StackTrace = error?.StackTrace
                        });
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                await response.WriteAsync(jsonResult);
            }
        }
    }
}
