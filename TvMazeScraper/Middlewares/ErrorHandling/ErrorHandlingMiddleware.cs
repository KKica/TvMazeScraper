using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TvMazeScrapper.Domain.Exceptions;

namespace TvMazeScraper.Middlewares.ErrorHandling
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(IHostEnvironment hostEnvironment, ILogger<ErrorHandlingMiddleware> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex) when (LogError(ex, context))
            {
                if (context.Response.HasStarted)
                {
                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private bool LogError(Exception exception, HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                return true;
            }

            ErrorResponse errorDetails = GetResponseData(exception, context);

            if (exception is AggregateException aggregateException)
            {
                foreach (Exception ex in aggregateException.Flatten().InnerExceptions)
                {
                    _logger.Log(errorDetails.LogLevel.Value, ex, errorDetails.LogMessage);
                }

                return true;
            }

            if (errorDetails.LogLevel.HasValue)
            {
                _logger.Log(errorDetails.LogLevel.Value, exception, errorDetails.LogMessage);
            }

            return true;
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ErrorResponse errorResponse = GetResponseData(exception, context);

            context.Response.StatusCode = (int)errorResponse.StatusCode;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(
                text: JsonSerializer.Serialize(errorResponse)
            );
        }
        private ErrorResponse GetResponseData(Exception exception, HttpContext context)
        {
            switch (exception)
            {
                case AggregateException aggregateEx:
                    var knownException = aggregateEx.Flatten().InnerExceptions.FirstOrDefault(ex => ex is OperationInProgressException);
                    if (knownException != null)
                    {
                        return GetResponseData(knownException, context);
                    }

                    return CreateErrorResponse(
                        message: "An internal error has occured. Please contact the administrator",
                        exception,
                        HttpStatusCode.InternalServerError,
                        LogLevel.Error,
                        "An unidentified aggregate exception has been thrown"
                    );

                case OperationInProgressException operationInProgressException:
                    return CreateErrorResponse(
                        message: operationInProgressException.Message,
                        operationInProgressException,
                        HttpStatusCode.BadRequest,
                        LogLevel.Information,
                        null
                    );

                case OperationCanceledException opcEx
                  when context.RequestAborted.IsCancellationRequested:
                    return CreateErrorResponse(
                        message: "Operation is cancelled by the client",
                        opcEx,
                        HttpStatusCode.BadRequest,
                        null,
                        null
                    );

                default:
                    return CreateErrorResponse(
                        message: "An internal error has occured. Please contact the administrator",
                        exception,
                        HttpStatusCode.InternalServerError,
                        LogLevel.Error,
                        "An unidentified exception has been thrown"
                    );
            }
        }

        private ErrorResponse CreateErrorResponse(string message, Exception exception, HttpStatusCode statusCode, LogLevel? logLevel, string logMessage)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                return new ErrorDetailedResponse(message, exception, statusCode, logLevel, logMessage);
            }

            return new ErrorResponse(message, statusCode, logLevel, logMessage);
        }
    }
}
