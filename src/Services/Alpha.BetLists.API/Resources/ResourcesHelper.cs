using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;
using Alpha.BetLists.API.Resources;
using Alpha.BetLists.API.Constants;

namespace Alpha.BetLists.API.Resources;

[ExcludeFromCodeCoverage]
public static class ResourcesHelper
{
    private const int MaxRowLength = 31;

    public static string Get(string key, string language)
    {
        var value = BetListResources.ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(language));

        return string.IsNullOrWhiteSpace(value)
            ? BetListResources.ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(LanguageCodeConstants.English))
            : value;
    }

    public static string BuildTitle(string language, string resourceKey, string date)
        => $"[{Get(resourceKey, language)} - {date}]";

    public static string BuildTitleWithoutStrikeThrough(string language, string resourceKey, string date)
        => $"[{Get(resourceKey, language)} {date}]";

    public static string FormatRow(string resourceKey, string value, string language)
    {
        var key = Get(resourceKey, language);
        var length = language switch
        {
            LanguageCodeConstants.ChineseSimplified or LanguageCodeConstants.ChineseTraditional => MaxRowLength - value.Length - key.Length * 2,
            LanguageCodeConstants.Thai => MaxRowLength - value.Length - ThaiLength(key),
            _ => MaxRowLength - value.Length - key.Length,
        };
        return $"{key}{" ".PadLeft(length > 0 ? length : 0)} {value}";
    }

    public static string EncodeXml(string text, bool isAttribute = true, int attrSubStringLength = 5)
        => isAttribute
        ? new XAttribute("__n", text).ToString().Substring(attrSubStringLength).TrimEnd('\"')
        : new XElement("t", text).LastNode.ToString();

    private static int ThaiLength(string text)
    {
        int c = 0;
        int l = text.Length;

        for (int i = 0; i < l; ++i)
        {
            if (char.GetUnicodeCategory(text[i]) != UnicodeCategory.NonSpacingMark)
            {
                ++c;
            }
        }

        return c;
    }
}