using System;

namespace ResponcesExamples.Exceptions
{
    public class SpecialStuffNotFoundException : Exception
    {
        public string Reasource { get; set; }

        public SpecialStuffNotFoundException(string reasource) : base(message: $"Requested reasourceId: {reasource} was not found")
        {
            Reasource = reasource;
        }
    }
}
