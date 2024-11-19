/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现毫米波在大气中的传输特性计算。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，考虑目标位置关系和大气参数分布。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 毫米波传输模型类，用于计算毫米波在大气中的传输特性
    /// </summary>
    internal class MillimeterWaveTransmittanceModel : TransmittanceModel
    {
        /// <summary>
        /// 毫米波波长（毫米）
        /// </summary>
        private const double MILLIMETER_WAVE_WAVELENGTH = 3.19; // 毫米（对应94 GHz）

        public MillimeterWaveTransmittanceModel(WeatherCondition weather) : base(weather) { }

        /// <summary>
        /// 计算给定距离的毫米波透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>毫米波透过率（0到1之间的值）</returns>
        public override double CalculateTransmittance(double distance)
        {
            // 计算各种衰减机制
            double molecularScattering = CalculateMolecularScattering();
            double aerosolScattering = CalculateAerosolScattering();
            double waterVaporAttenuation = CalculateWaterVaporAttenuation();
            double oxygenAttenuation = CalculateOxygenAttenuation();
            
            // 计算天气效应
            double rainAttenuation = CalculateRainAttenuation(distance, MILLIMETER_WAVE_WAVELENGTH);
            double snowAttenuation = CalculateSnowAttenuation(distance, MILLIMETER_WAVE_WAVELENGTH);
            double fogAttenuation = CalculateMillimeterWaveFogAttenuation(distance);

            // 计算总衰减
            double totalAttenuation = (molecularScattering + aerosolScattering + waterVaporAttenuation + oxygenAttenuation) * distance +
                                    rainAttenuation + snowAttenuation + fogAttenuation;

            // 计算透过率
            double transmittance = Math.Exp(-totalAttenuation);

            return Math.Max(0, Math.Min(1, transmittance));
        }

        /// <summary>
        /// 计算分子散射系数
        /// </summary>
        /// <returns>分子散射系数（km^-1）</returns>
        private double CalculateMolecularScattering()
        {
            // 分子散射系数计算（km^-1）
            return 0.0015 * (Pressure / 1013.25) * (288.15 / Temperature) * 
                   Math.Pow(MILLIMETER_WAVE_WAVELENGTH / 3.0, -4);
        }

        /// <summary>
        /// 计算气溶胶散射系数
        /// </summary>
        /// <returns>气溶胶散射系数（km^-1）</returns>
        private double CalculateAerosolScattering()
        {
            // 气溶胶散射系数计算（km^-1）
            double beta = 0.0434 * (AerosolDensity / STANDARD_AEROSOL_DENSITY);
            return beta * Math.Pow(MILLIMETER_WAVE_WAVELENGTH / 3.0, -1.2);
        }

        /// <summary>
        /// 计算水汽吸收系数
        /// </summary>
        /// <returns>水汽吸收系数（km^-1）</returns>
        private double CalculateWaterVaporAttenuation()
        {
            // 水汽吸收系数计算（km^-1）
            double rho = CalculateWaterVaporDensity();
            double pressureFactor = Math.Sqrt(Pressure / 1013.25);
            return 0.05 * rho * (MILLIMETER_WAVE_WAVELENGTH / 100.0) * (Temperature / 293.15) * pressureFactor;
        }

        /// <summary>
        /// 计算水汽密度
        /// </summary>
        /// <returns>水汽密度（g/m³）</returns>
        private double CalculateWaterVaporDensity()
        {
            // 使用Magnus公式计算水汽密度（g/m³）
            double saturationVaporPressure = 6.112 * Math.Exp(17.27 * (Temperature - 273.15) / (Temperature - 35.85));
            double actualVaporPressure = (Humidity / 100.0) * saturationVaporPressure;
            return (actualVaporPressure * 2.16679) / Temperature;
        }

        /// <summary>
        /// 计算氧气吸收系数
        /// </summary>
        /// <returns>氧气吸收系数（km^-1）</returns>
        private double CalculateOxygenAttenuation()
        {
            // 氧气吸收系数计算（km^-1）
            double gamma = 0.01 * (MILLIMETER_WAVE_WAVELENGTH / 60.0) * (Temperature / 293.15);
            return gamma * (Pressure / 1013.25);
        }

        /// <summary>
        /// 计算雾对毫米波的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>雾衰减</returns>
        private double CalculateMillimeterWaveFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            // 雾的衰减计算
            double liquidWaterDensity = 0.05 * (1 - Visibility / 10.0); // 根据能见度估算液态水含量
            double specificAttenuation = 0.4 * MILLIMETER_WAVE_WAVELENGTH * liquidWaterDensity / 1000.0;
            return specificAttenuation * pathLength;
        }

        protected override double CalculateRainKCoefficient(double wavelength)
        {
            // 对应94 GHz毫米波
            return 0.926;
        }

        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            // 对应94 GHz毫米波
            return 0.9551;
        }

        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            return 0.997;
        }

        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            return 1.064;
        }
    }
}
