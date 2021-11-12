using System;

namespace TvMazeScrapper.Domain.Exceptions
{
    public class OperationInProgressException : Exception
    {
        public OperationInProgressException(string message) : base(message)
        {

        }
    }
}
