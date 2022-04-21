using System;

namespace ResponcesExamples.Exceptions
{
    public class FriendlyExceptionWithData : Exception
    {
        public string SomeData { get; set; }

        public FriendlyExceptionWithData(string someData) : base("There was a problem with the data")
        {
            SomeData = someData;
        }

        public static string GetFriendlyMessage(Exception ex)
        {
            var custEx = ex as FriendlyExceptionWithData;

            return $"{ex.Message}: {custEx.SomeData}";
        }
    }
}
