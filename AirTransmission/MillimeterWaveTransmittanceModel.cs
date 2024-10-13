/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现毫米波在大气中的传输特性计算，包括氧气和水蒸气的影响。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 毫米波传输模型类，用于计算毫米波在大气中的传输特性
    /// </summary>
    public class MillimeterWaveTransmittanceModel(WeatherCondition weather) : TransmittanceModel(weather)
    {
        /// <summary>
        /// 毫米波波长（毫米）
        /// </summary>
        private const double MILLIMETER_WAVE_WAVELENGTH = 3.19; // 毫米（对应94 GHz）

        /// <summary>
        /// 计算给定距离的毫米波透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>毫米波透过率（0到1之间的值）</returns>
        public override double CalculateTransmittance(double distance)
        {
            double attenuationFactor = CalculateMillimeterWaveAttenuationFactor();
            double rainAttenuation = CalculateRainAttenuation(distance, MILLIMETER_WAVE_WAVELENGTH);
            double fogAttenuation = CalculateMillimeterWaveFogAttenuation(distance);
            double snowAttenuation = CalculateSnowAttenuation(distance, MILLIMETER_WAVE_WAVELENGTH);
            double transmittance = Math.Exp(-attenuationFactor * distance - rainAttenuation - fogAttenuation - snowAttenuation);

            return transmittance;
        }

        /// <summary>
        /// 计算雨对毫米波的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（毫米）</param>
        /// <returns>雨衰减系数K</returns>
        protected override double CalculateRainKCoefficient(double wavelength)
        {
            // 对应94 GHz毫米波
            return 0.926;
        }

        /// <summary>
        /// 计算雨对毫米波的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（毫米）</param>
        /// <returns>雨衰减系数α</returns>
        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            // 对应94 GHz毫米波
            return 0.9551;
        }

        /// <summary>
        /// 计算雪对毫米波的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（毫米）</param>
        /// <returns>雪衰减系数K</returns>
        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            return 0.997;
        }

        /// <summary>
        /// 计算雪对毫米波的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（毫米）</param>
        /// <returns>雪衰减系数α</returns>
        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            return 1.064;
        }

        /// <summary>
        /// 计算毫米波的总衰减因子
        /// </summary>
        /// <returns>毫米波总衰减因子</returns>
        private double CalculateMillimeterWaveAttenuationFactor()
        {
            double oxygenAttenuation = CalculateOxygenAttenuation();
            double waterVaporAttenuation = CalculateWaterVaporAttenuation();
            return oxygenAttenuation + waterVaporAttenuation;
        }

        /// <summary>
        /// 计算氧气对毫米波的衰减
        /// </summary>
        /// <returns>氧气衰减因子</returns>
        private double CalculateOxygenAttenuation()
        {
            double gamma = 0.01 * (MILLIMETER_WAVE_WAVELENGTH / 60.0) * (Temperature / 293.15);
            return gamma * (Pressure / 1013.25);
        }

        /// <summary>
        /// 计算水蒸气对毫米波的衰减
        /// </summary>
        /// <returns>水蒸气衰减因子</returns>
        private double CalculateWaterVaporAttenuation()
        {
            double rho = Humidity * 0.01 * 6.11 * Math.Exp(17.27 * (Temperature - 273.15) / (Temperature - 35.85));
            return 0.05 * rho * (MILLIMETER_WAVE_WAVELENGTH / 100.0) * (Temperature / 293.15);
        }

        /// <summary>
        /// 计算雾对毫米波的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>雾对毫米波的衰减</returns>
        private double CalculateMillimeterWaveFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            double liquidWaterDensity = 0.05; // 假设的液态水密度，单位：g/m³
            double specificAttenuation = 0.4 * MILLIMETER_WAVE_WAVELENGTH * liquidWaterDensity / 1000.0;
            return specificAttenuation * pathLength;
        }
    }
}
