using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alpha.Reports.API.Tests.Helpers;

public static class FileHelper
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

    public static T ReadJson<T>(string relativeFilePath, JsonSerializerSettings settings)
    {
        var json = ReadContent(relativeFilePath);
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static T ReadJson<T>(string relativeFilePath)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver()
        };
        return ReadJson<T>(relativeFilePath, settings);
    }

    public static string ReadContent(string relativeFilePath)
    {
        var filePath = GetFilePath(relativeFilePath);
        return File.ReadAllText(filePath);
    }
}
