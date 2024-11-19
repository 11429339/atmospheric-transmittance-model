using System;

namespace AirTransmission
{
    /// <summary>
    /// 天气条件类
    /// </summary>
    public class WeatherCondition(WeatherType type, double temperature, double relativeHumidity, double visibility, double? precipitation = null, double co2Concentration = 415)
    {
        /// <summary>
        /// 天气类型
        /// </summary>
        public WeatherType Type { get; set; } = type; // 天气类型
        /// <summary>
        /// 温度（摄氏度）
        /// </summary>
        public double Temperature { get; set; } = temperature; // 温度
        /// <summary>
        /// 相对湿度（百分比）
        /// </summary>
        public double RelativeHumidity { get; set; } = relativeHumidity; // 相对湿度
        /// <summary>
        /// 能见度（公里）
        /// </summary>
        public double Visibility { get; set; } = visibility; // 能见度
        /// <summary>
        /// 降水量（毫米/小时）
        /// </summary>
        public double? Precipitation { get; set; } = precipitation; // 降水量
        /// <summary>
        /// 二氧化碳浓度（ppm）
        /// </summary>
        public double CO2Concentration { get; set; } = co2Concentration;

        /// <summary>
        /// 打印天气信息
        /// </summary>
        /// <param name="weather">天气条件</param>
        public static void PrintWeatherInfo(WeatherCondition weather){
            Console.WriteLine($"---天气类型: {weather.Type}, 温度: {weather.Temperature}°C, 相对湿度: {weather.RelativeHumidity}%, 能见度: {weather.Visibility}km, 降水量: {weather.Precipitation ?? 0}mm/h");
        }
    }

    /// <summary>
    /// 天气类型枚举
    /// </summary>
    public enum WeatherType
    {
        晴天,   // 晴朗天气
        雨天,   // 雨天
        雪天,   // 雪天
        雾天,   // 雾天
        沙尘   // 沙尘
    }
}