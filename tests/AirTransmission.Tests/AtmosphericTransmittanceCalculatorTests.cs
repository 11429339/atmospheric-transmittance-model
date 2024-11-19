using Xunit;

namespace AirTransmission.Tests;

public class AtmosphericTransmittanceCalculatorTests
{
    [Fact]
    public void CalcLaser_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var weather = new WeatherCondition(
            WeatherType.晴天,         // 天气类型
            temperature: 20,         // 温度(℃)
            relativeHumidity: 45,    // 相对湿度
            visibility: 10,          // 能见度(公里)
            precipitation: 0         // 降雨量(mm/h)
        );
        double distance = 1;     // 1公里

        // Act
        double result = AtmosphericTransmittanceCalculator.CalcLaser(weather, distance);

        Console.WriteLine($"计算得到的透过率为: {result}");

        // Assert
        Assert.True(result > 0 && result <= 1);
        Assert.True(result > 0.5);  // 在晴朗天气下，1公里距离的透过率应该大于0.5
    }
} 