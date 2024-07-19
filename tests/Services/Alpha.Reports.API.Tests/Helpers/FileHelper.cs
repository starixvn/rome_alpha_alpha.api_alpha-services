using System.Data;
using System.Text.Json;

namespace Alpha.Reports.API.Tests.Helpers;

public class FileHelper
{
    public static string GetFilePath(string relativeFilePath)
    {
        string codeBase = typeof(FileHelper).Assembly.Location;
        var uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);

        return Path.GetDirectoryName(path) + relativeFilePath;
    }

    public static DataSet LoadDataSet(string relativeFilePath)
    {
        var dataSet = new DataSet();
        dataSet.ReadXml(GetFilePath(relativeFilePath));

        return dataSet;
    }

    public static T ReadJson<T>(string relativeFilePath, JsonSerializerOptions settings)
    {
        var json = ReadContent(relativeFilePath);
        return JsonSerializer.Deserialize<T>(json, settings);
    }

    public static T ReadJson<T>(string relativeFilePath)
    {
        var settings = new JsonSerializerOptions
        {
            IgnoreReadOnlyFields = false
        };
        return ReadJson<T>(relativeFilePath, settings);
    }

    public static string ReadContent(string relativeFilePath)
    {
        var filePath = GetFilePath(relativeFilePath);
        return File.ReadAllText(filePath);
    }
}
