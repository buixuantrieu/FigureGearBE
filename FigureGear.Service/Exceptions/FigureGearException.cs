using System.Globalization;
using System.Net;

namespace eKonect.Service.Exceptions
{
    [Serializable]
    public class FigureGearException : Exception
    {

        public FigureGearException(string errorMessage) : base(errorMessage)
        {
            StatusCode = HttpStatusCode.BadRequest;
        }

        public FigureGearException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            StatusCode = HttpStatusCode.BadRequest;
        }

        public Object ToSerializableObject()
        {

            return new
            {
                MessageCode,
                StatusCode,
                Message,
                StackTrace
            };
        }


        public string MessageCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
