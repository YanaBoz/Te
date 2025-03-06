namespace Web_Library.API.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException() : base("The request has timed out.")
        {
        }

        public RequestTimeoutException(string message) : base(message)
        {
        }

        public RequestTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
