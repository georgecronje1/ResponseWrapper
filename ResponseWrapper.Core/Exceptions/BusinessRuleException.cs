using System;

namespace ResponseWrapper.Core.Exceptions
{
    /// <summary>
    /// Represents an error due to violation of a
    /// Business Rule
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
