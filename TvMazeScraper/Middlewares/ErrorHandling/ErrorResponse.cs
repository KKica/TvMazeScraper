using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Serialization;

namespace TvMazeScraper.Middlewares.ErrorHandling
{
    public class ErrorResponse
    {
        public ErrorResponse(string message, HttpStatusCode statusCode, LogLevel? logLevel, string logMessage)
        {
            Message = message;
            StatusCode = statusCode;
            LogLevel = logLevel;
            LogMessage = logMessage;
        }

        public string Message { get; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore]
        public LogLevel? LogLevel { get; set; }

        [JsonIgnore]
        public string LogMessage { get; set; }
    }
}
