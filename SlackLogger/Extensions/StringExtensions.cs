namespace SlackLogger.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string str, int maxLength)
        {
            if (str?.Length > maxLength)
            {
                return str?.Substring(0, 1900) + "...";
            }
            return str;
        }

    }
}
