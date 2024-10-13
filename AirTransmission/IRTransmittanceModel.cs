/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现红外线在大气中的传输特性计算，包括水蒸气和CO2的影响。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 红外线传输模型类，用于计算红外线在大气中的传输特性
    /// </summary>
    public class IRTransmittanceModel(WeatherCondition weather) : TransmittanceModel(weather)
    {
        // 红外线波长（微米）
        private const double IR_WAVELENGTH = 10.0;

        /// <summary>
        /// 计算给定距离的红外线透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>红外线透过率（0到1之间的值）</returns>
        public override double CalculateTransmittance(double distance)
        {
            double attenuationFactor = CalculateIRAttenuationFactor();
            double rainAttenuation = CalculateRainAttenuation(distance, IR_WAVELENGTH);
            double fogAttenuation = CalculateFogAttenuation(distance);
            double dustAttenuation = CalculateDustAttenuation(distance);
            double snowAttenuation = CalculateSnowAttenuation(distance, IR_WAVELENGTH);
            double waterVaporAttenuation = CalculateWaterVaporAttenuation(distance);
            double co2Attenuation = CalculateCO2Attenuation(distance);

            double transmittance = Math.Exp(-attenuationFactor * distance - rainAttenuation - fogAttenuation - dustAttenuation - snowAttenuation - waterVaporAttenuation - co2Attenuation);

            return Math.Max(0, Math.Min(1, transmittance));
        }

        /// <summary>
        /// 计算雨对红外线的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数K</returns>
        protected override double CalculateRainKCoefficient(double wavelength)
        {
            if (wavelength >= 8 && wavelength <= 12)
            {
                // 对于远红外波段（8-12μm）
                return 0.187;
            }
            else if (wavelength >= 3 && wavelength <= 5)
            {
                // 对于中远红外波段（3-5μm）
                return 0.2656;   
            }

            return 0;
        }

        /// <summary>
        /// 计算雨对红外线的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数α</returns>
        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            if (wavelength >= 8 && wavelength <= 12)
            {
                // 对于远红外波段（8-12μm）
                return 0.784;
            }
            else if (wavelength >= 3 && wavelength <= 5)
            {
                // 对于中远红外波段（3-5μm）
                return 0.7978;
            }

            return 0;
        }

        /// <summary>
        /// 计算雪对红外线的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数K</returns>
        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            return 0.365;
        }

        /// <summary>
        /// 计算雪对红外线的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数α</returns>
        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            return 0.88;
        }

        /// <summary>
        /// 计算红外线的总衰减因子
        /// </summary>
        /// <returns>红外线总衰减因子</returns>
        private double CalculateIRAttenuationFactor()
        {
            double molecularFactor = CalculateIRMolecularFactor();
            double aerosolFactor = CalculateIRAerosolFactor();
            return molecularFactor + aerosolFactor;
        }
        
        /// <summary>
        /// 计算红外线的分子散射因子
        /// </summary>
        /// <returns>红外线分子散射因子</returns>
        private double CalculateIRMolecularFactor()
        {
            // 对于10μm波长，分子散射可以忽略不计
            return 0.0001;
        }

        /// <summary>
        /// 计算红外线的气溶胶散射因子
        /// </summary>
        /// <returns>红外线气溶胶散射因子</returns>
        private double CalculateIRAerosolFactor()
        {
            // 使用更适合IR的模型
            double beta = 0.0116 * Math.Pow(Visibility, -0.75);
            return beta;
        }

        /// <summary>
        /// 计算雾对红外线的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>雾对红外线的衰减</returns>
        private double CalculateFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double beta = 3.91 / Visibility * Math.Pow(IR_WAVELENGTH / 0.55, -q);
            return beta * pathLength * 0.5;
        }

        /// <summary>
        /// 计算水蒸气对红外线的衰减
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>水蒸气对红外线的衰减</returns>
        private double CalculateWaterVaporAttenuation(double distance)
        {
            double waterVaporDensity = CalculateWaterVaporDensity();
            double absorptionCoefficient = 0.0005; // 这个值需要根据具体波长调整
            return absorptionCoefficient * waterVaporDensity * distance;
        }

        /// <summary>
        /// 计算水蒸气密度
        /// </summary>
        /// <returns>水蒸气密度（g/m³）</returns>
        private double CalculateWaterVaporDensity()
        {
            // 使用Magnus公式计算饱和水汽压
            double a = 17.27;
            double b = 237.7;
            double saturationVaporPressure = 6.112 * Math.Exp((a * Temperature) / (b + Temperature));
            
            // 计算实际水汽压
            double actualVaporPressure = (Humidity / 100.0) * saturationVaporPressure;
            
            // 计算水汽密度 (g/m³)
            return (actualVaporPressure * 2.16679) / (Temperature + 273.15);
        }

        /// <summary>
        /// 计算二氧化碳对红外线的衰减
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>二氧化碳对红外线的衰减</returns>
        private double CalculateCO2Attenuation(double distance)
        {
            double co2Concentration = CO2Concentration; // ppm
            double absorptionCoefficient = 0.00001; // 这个值需要根据具体波长调整
            return absorptionCoefficient * (co2Concentration / 1000000) * distance;
        }
    }
}
