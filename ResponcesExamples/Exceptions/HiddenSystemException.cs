using System;

namespace ResponcesExamples.Exceptions
{
    public class HiddenSystemException : Exception
    {
        public HiddenSystemException(string hiddenMessage) : base(message: $"Dont show to end user: {hiddenMessage}")
        {

        }
    }
}
