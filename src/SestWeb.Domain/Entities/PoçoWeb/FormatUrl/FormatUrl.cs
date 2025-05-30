using System.Text.RegularExpressions;

namespace SestWeb.Domain.Entities.PoçoWeb.FormatUrl
{
    public class FormatUrl
    {
        public static string[] Execute(string url)
        {
            // Define a regular expression for repeated words.
            Regex rx = new Regex(@"\b\/(?<id>\d+)\/(\?rev=(?<rev>\d+))?",
              RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            MatchCollection matches = rx.Matches(url);

            GroupCollection groups = matches[0].Groups;

            if (groups["rev"].Success)
            {
                return new string[] { groups["id"].Value, groups["rev"].Value };
            }
            return new string[] { groups["id"].Value };
        }

    }
}
