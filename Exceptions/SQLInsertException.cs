using System;

namespace STCore.Exceptions
{
    public class SQLInsertException : Exception
    {
        public SQLInsertException()
        {

        }
        public SQLInsertException(string message) : base(message)
        {

        }
        public SQLInsertException(string message, Exception Inner) : base(message, Inner)
        {

        }
    }
}
