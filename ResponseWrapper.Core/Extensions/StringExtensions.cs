namespace ResponseWrapper.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to lower case and trim it.
        /// This standardised conversions will help with string comparisons.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToComparable(this string input)
        {
            return input?.ToLowerInvariant().Trim();
        }
    }
}
