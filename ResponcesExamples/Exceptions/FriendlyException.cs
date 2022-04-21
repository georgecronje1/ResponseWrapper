using System;

namespace ResponcesExamples.Exceptions
{
    public class FriendlyException : Exception
    {
        public FriendlyException(string userFriendlyMessage) : base(message: userFriendlyMessage)
        {

        }
    }
}
