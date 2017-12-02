using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fbi.Std.Core
{
    //Extension methods must be defined in a static class
    public static class StringCleaner
    {
        private static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string Clean(string s)
        {
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            StringBuilder sb = new StringBuilder(s);

            sb.Replace("'", "''");
            sb.Replace(" ", "");
            sb.Replace("\r\n", string.Empty);
            sb.Replace("\n", string.Empty);
            sb.Replace("\r", string.Empty);
            sb.Replace(lineSeparator, string.Empty);
            sb.Replace(paragraphSeparator, string.Empty);

            return ConvertWhitespacesToSingleSpaces(sb.ToString()).Trim();
        }

    }
}
