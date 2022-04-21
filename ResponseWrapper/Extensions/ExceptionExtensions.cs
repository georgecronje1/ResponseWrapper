using System;
using System.Collections.Generic;

namespace ResponseWrapper.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ExtractExceptionDescription(this Exception ex)
        {
            List<string> errors = new List<string>();
            errors.Add(ex.Message);
            errors.Add(ex.StackTrace);

            if (ex.InnerException != null)
            {
                errors.Add(ex.InnerException.Message);
                errors.Add(ex.InnerException.StackTrace);
            }

            if (ex.Data.Count > 0)
            {
                foreach (var key in ex.Data.Keys)
                {
                    errors.Add($"Data.{key}:{ex.Data[key].ToString()}");
                }
            }

            string reasonDescription = string.Join("|", errors.ToArray());
            return reasonDescription;
        }
    }
}
