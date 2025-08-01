using Alpha.PartialUM.Models;

namespace Alpha.PartialUM.Helpers;

public static class PartialUMSettingsHelper
{
    private static readonly IDictionary<string, IList<string>> functions = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);

    private static void AssignSettings(PartialUMSettings settings)
    {
        if (functions.Any() || settings.Functions == null)
        {
            return;
        }

        foreach (var function in settings.Functions)
        {
            AddFunctionSettings(function);
        }
    }

    private static void AddFunctionSettings(PartialUMFunctionSetting? function)
    {
        if (function?.Settings == null || function.Name == null)
        {
            return;
        }

        foreach (var setting in function.Settings)
        {
            AddSettingToFunctions(setting, function.Name);
        }
    }

    private static void AddSettingToFunctions(PartialUMSPSetting? setting, string functionName)
    {
        if (setting?.SettingKey == null)
        {
            return;
        }

        if (!functions.TryGetValue(setting.SettingKey, out var functionNames))
        {
            functionNames = new List<string>();
            functions[setting.SettingKey] = functionNames;
        }

        if (!functionNames.Any(f => f.Equals(functionName, StringComparison.OrdinalIgnoreCase)))
        {
            functionNames.Add(functionName);
        }
    }

    public static IEnumerable<string> GetFunctionNames(string settingKey, PartialUMSettings settings)
    {
        AssignSettings(settings);

        return functions.TryGetValue(settingKey, out var functionNames)
            ? functionNames
            : Enumerable.Empty<string>();
    }
}