using System;

namespace STCore.Exceptions
{
    public class SQLDeleteException : Exception
    {
        public SQLDeleteException()
        {

        }
        public SQLDeleteException(string message) : base(message)
        {

        }
        public SQLDeleteException(string message, Exception InnerEx) : base(message, InnerEx)
        {

        }
    }
}
