using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace TvMazeScraper.Middlewares.ErrorHandling
{
    public class ErrorDetailedResponse : ErrorResponse
    {
        public ErrorDetailedResponse(string message, Exception exception, HttpStatusCode statusCode, LogLevel? logLevel, string logMessage)
            : base(message, statusCode, logLevel, logMessage)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
