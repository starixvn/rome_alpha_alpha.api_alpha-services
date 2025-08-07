using System.Collections.Generic;
using System.Linq;
using Alpha.PartialUM.Helpers;
using Alpha.PartialUM.Models;
using Xunit;

namespace Alpha.PartialUM.Tests.Helpers;

public class PartialUMSettingsHelperTests
{
    [Fact]
    public void GetFunctionNames_WithValidSettingKey_ReturnsFunctionNames()
    {
        // Arrange
        var settings = new PartialUMSettings
        {
            Functions = new List<PartialUMFunctionSetting>
            {
                new PartialUMFunctionSetting
                {
                    Name = "FuncA",
                    Settings = new List<PartialUMSPSetting>
                    {
                        new PartialUMSPSetting { SettingKey = "Key1", SPName = "SP1" },
                        new PartialUMSPSetting { SettingKey = "Key2", SPName = "SP2" }
                    }
                },
                new PartialUMFunctionSetting
                {
                    Name = "FuncB",
                    Settings = new List<PartialUMSPSetting>
                    {
                        new PartialUMSPSetting { SettingKey = "Key1", SPName = "SP3" }
                    }
                }
            }
        };

        // Act
        var result = PartialUMSettingsHelper.GetFunctionNames("Key1", settings).ToList();

        // Assert
        Assert.Contains("FuncA", result);
        Assert.Contains("FuncB", result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetFunctionNames_WithUnknownSettingKey_ReturnsEmpty()
    {
        // Arrange
        var settings = new PartialUMSettings
        {
            Functions = new List<PartialUMFunctionSetting>
            {
                new PartialUMFunctionSetting
                {
                    Name = "FuncA",
                    Settings = new List<PartialUMSPSetting>
                    {
                        new PartialUMSPSetting { SettingKey = "Key1", SPName = "SP1" }
                    }
                }
            }
        };

        // Act
        var result = PartialUMSettingsHelper.GetFunctionNames("UnknownKey", settings);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFunctionNames_WithNullFunctions_ReturnsEmpty()
    {
        // Arrange
        var settings = new PartialUMSettings { Functions = null };

        // Act
        var result = PartialUMSettingsHelper.GetFunctionNames("Key1", settings);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFunctionNames_WithNullSettingKeyInSPSetting_DoesNotThrow()
    {
        // Arrange
        var settings = new PartialUMSettings
        {
            Functions = new List<PartialUMFunctionSetting>
            {
                new PartialUMFunctionSetting
                {
                    Name = "FuncA",
                    Settings = new List<PartialUMSPSetting>
                    {
                        new PartialUMSPSetting { SettingKey = null, SPName = "SP1" }
                    }
                }
            }
        };

        // Act
        var result = PartialUMSettingsHelper.GetFunctionNames("Key1", settings);

        // Assert
        Assert.Empty(result);
    }
}
