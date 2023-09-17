using System;

namespace STCore.Exceptions
{
    public class SQLUpdateException : Exception
    {
        public SQLUpdateException()
        {

        }
        public SQLUpdateException(string message) : base(message)
        {

        }
        public SQLUpdateException(string message, Exception InnerEx) : base(message, InnerEx)
        {

        }
    }
}
