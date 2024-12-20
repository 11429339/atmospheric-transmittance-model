/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现大气透过率模型的基类，包括各种天气条件下的衰减计算。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，考虑目标位置关系和大气参数分布。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 大气透过率模型的抽象基类，提供了各种大气条件下的透过率计算方法
    /// </summary>
    internal abstract class TransmittanceModel(WeatherCondition weather)
    {
        // 常量
        /// <summary>
        /// 标准大气透过率
        /// </summary>
        protected const double STANDARD_TRANSMITTANCE = 0.975; // 标准大气透过率
        /// <summary>
        /// 标准能见度（公里）
        /// </summary>
        protected const double STANDARD_VISIBILITY = 23.0; // 标准能见度 (km)
        /// <summary>
        /// 标准气溶胶密度（粒子/立方厘米）
        /// </summary>
        protected const double STANDARD_AEROSOL_DENSITY = 100.0; // 标准气溶胶密度 (particles/cm³)


        /// <summary>
        /// 当前天气条件
        /// </summary>
        protected WeatherCondition weatherCondition = weather;
        // 大气模型参数
        /// <summary>
        /// 温度（开尔文）
        /// </summary>
        protected double Temperature { get; set; } = weather.Temperature + 273.15;
        /// <summary>
        /// 大气压力（百帕）
        /// </summary>
        protected double Pressure { get; set; } = 1013.25;
        /// <summary>
        /// 相对湿度（百分比）
        /// </summary>
        protected double Humidity { get; set; } = weather.RelativeHumidity;
        /// <summary>
        /// 气溶胶密度（粒子/立方厘米）
        /// </summary>
        protected double AerosolDensity { get; set; } = CalculateAerosolDensity(weather.Visibility);
        /// <summary>
        /// 能见度（公里）
        /// </summary>
        protected double Visibility { get; set; } = weather.Visibility;
        /// <summary>
        /// 是否下雨
        /// </summary>
        protected bool IsRaining { get; set; } = weather.Type == WeatherType.雨天;
        /// <summary>
        /// 降雨量（毫米/小时）
        /// </summary>
        protected double RainRate { get; set; } = weather.Type == WeatherType.雨天 ? (weather.Precipitation ?? 0) : 0;
        /// <summary>
        /// 是否有雾
        /// </summary>
        protected bool IsFoggy { get; set; } = weather.Type == WeatherType.雾天;
        /// <summary>
        /// 是否有沙尘
        /// </summary>
        protected bool IsDusty { get; set; } = weather.Type == WeatherType.沙尘;
        /// <summary>
        /// 是否下雪
        /// </summary>
        protected bool IsSnowing { get; set; } = weather.Type == WeatherType.雪天;
        /// <summary>
        /// 降雪量（毫米/小时）
        /// </summary>
        protected double SnowRate { get; set; } = weather.Type == WeatherType.雪天 ? (weather.Precipitation ?? 0) : 0;
        /// <summary>
        /// 二氧化碳浓度（ppm）
        /// </summary>
        protected double CO2Concentration { get; set; } = 415; // ppm, 默认值

        /// <summary>
        /// 计算给定距离的大气透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>大气透过率（0到1之间的值）</returns>
        public abstract double CalculateTransmittance(double distance);

        /// <summary>
        /// 根据能见度计算气溶胶密度
        /// </summary>
        /// <param name="visibility">能见度（公里）</param>
        /// <returns>气溶胶密度（粒子/立方厘米）</returns>
        protected static double CalculateAerosolDensity(double visibility)
        {
            // 使用指数关系来计算气溶胶密度
            return STANDARD_AEROSOL_DENSITY * Math.Pow(STANDARD_VISIBILITY / Math.Max(visibility, 0.1), 0.75);
        }

        /// <summary>
        /// 计算雨对电磁波的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雨衰减系数K</returns>
        protected abstract double CalculateRainKCoefficient(double wavelength);

        /// <summary>
        /// 计算雨对电磁波的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雨衰减系数α</returns>
        protected abstract double CalculateRainAlphaCoefficient(double wavelength);

        /// <summary>
        /// 计算雪对电磁波的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雪衰减系数K</returns>
        protected abstract double CalculateSnowKCoefficient(double wavelength);
        /// <summary>
        /// 计算雪对电磁波的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雪衰减系数α</returns>
        protected abstract double CalculateSnowAlphaCoefficient(double wavelength);
        /// <summary>
        /// 计算雨对电磁波的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雨衰减（dB）</returns>
        protected double CalculateRainAttenuation(double pathLength, double wavelength)
        {
            if (!IsRaining || RainRate <= 0)
                return 0;

            // 计算 k 和 α 系数
            double k = CalculateRainKCoefficient(wavelength);
            double alpha = CalculateRainAlphaCoefficient(wavelength);

            // 计算特定衰减（dB/km）
            double specificAttenuation = k * Math.Pow(RainRate, alpha);

            // 计算总衰减（dB）
            return specificAttenuation * pathLength;
        }

        /// <summary>
        /// 计算雪对电磁波的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <param name="wavelength">波长（微米或毫米）</param>
        /// <returns>雪衰减（dB）</returns>
        protected double CalculateSnowAttenuation(double pathLength, double wavelength)
        {
            if (!IsSnowing || SnowRate <= 0)
                return 0;

            // 雪的衰减系数（这里使用一个简化模型，实际上可能需要更复杂的计算）
            double k = CalculateSnowKCoefficient(wavelength);
            double alpha = CalculateSnowAlphaCoefficient(wavelength);

            // 计算特定衰减（dB/km）
            double specificAttenuation = k * Math.Pow(SnowRate, alpha);

            // 计算总衰减（dB）
            return specificAttenuation * pathLength;
        }

        /// <summary>
        /// 计算沙尘对电磁波的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>沙尘衰减（dB）</returns>
        protected double CalculateDustAttenuation(double pathLength)
        {
            if (!IsDusty)
                return 0;

            // 沙尘天气衰减计算
            double dustFactor = 1.5; // 沙尘对衰减的额外影响因子
            double beta = (3.91 / Visibility) * dustFactor;
            return beta * pathLength;
        }

        /// <summary>
        /// 计算能见度因子
        /// </summary>
        /// <returns>能见度因子</returns>
        protected double CalculateVisibilityFactor()
        {
            // 调整能见度因子计算，减小低能见度的影响
            double factor;
            if (Visibility >= STANDARD_VISIBILITY)
                factor = 1;
            else if (Visibility > 5)
                factor = Math.Pow(STANDARD_VISIBILITY / Visibility, 0.25); //0.2
            else
                factor = Math.Pow(STANDARD_VISIBILITY / Visibility, 0.4); //

            if (IsDusty)
            {
                // 沙尘天气下，进一步降低能见度因子
                factor *= 0.9;
            }

            return factor;
        }
    }
}