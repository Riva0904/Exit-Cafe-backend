using System.Text.RegularExpressions;

namespace ExitCafe.Application.Common.Utilities;

public static class SlugHelper
{
    public static string GenerateSlug(string phrase)
    {
        var str = phrase.ToLowerInvariant().Trim();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-").Trim('-');
        str = Regex.Replace(str, @"-+", "-");
        return str;
    }
}
