/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现紫外线在大气中的传输特性计算。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 紫外线传输模型类，用于计算紫外线在大气中的传输特性
    /// </summary>
    public class UVTransmittanceModel(WeatherCondition weather) : TransmittanceModel(weather)
    {
        /// <summary>
        /// 紫外线波长（微米）
        /// </summary>
        private const double UV_WAVELENGTH = 0.308; // 微米 (308 nm)

        /// <summary>
        /// 计算给定距离的紫外线透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>紫外线透过率（0到1之间的值）</returns>
        public override double CalculateTransmittance(double distance)
        {
            double attenuationFactor = CalculateUVAttenuationFactor();
            double rainAttenuation = CalculateRainAttenuation(distance, UV_WAVELENGTH);
            double fogAttenuation = CalculateFogAttenuation(distance);
            double dustAttenuation = CalculateDustAttenuation(distance);
            double snowAttenuation = CalculateSnowAttenuation(distance, UV_WAVELENGTH);

            double transmittance = Math.Pow(STANDARD_TRANSMITTANCE, attenuationFactor * distance) * 
                   Math.Exp(-rainAttenuation - fogAttenuation - dustAttenuation - snowAttenuation);

            return Math.Max(0, Math.Min(1, transmittance));
        }

        /// <summary>
        /// 计算雨对紫外线的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数K</returns>
        protected override double CalculateRainKCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.4715;
        }

        /// <summary>
        /// 计算雨对紫外线的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数α</returns>
        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.6296;
        }

        /// <summary>
        /// 计算雪对紫外线的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数K</returns>
        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.5225;
        }

        /// <summary>
        /// 计算雪对紫外线的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数α</returns>
        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.7937;
        }

        /// <summary>
        /// 计算紫外线的总衰减因子
        /// </summary>
        /// <returns>紫外线总衰减因子</returns>
        private double CalculateUVAttenuationFactor()
        {
            double molecularFactor = CalculateUVMolecularFactor();
            double aerosolFactor = CalculateUVAerosolFactor();
            double visibilityFactor = CalculateVisibilityFactor();

            return molecularFactor * aerosolFactor * visibilityFactor;
        }

        /// <summary>
        /// 计算紫外线的分子散射因子
        /// </summary>
        /// <returns>紫外线分子散射因子</returns>
        private double CalculateUVMolecularFactor()
        {
            // UV分子散射比可见光更强
            return (Pressure / 1013.25) * (288.15 / Temperature) * 2.0;
        }

        /// <summary>
        /// 计算紫外线的气溶胶散射因子
        /// </summary>
        /// <returns>紫外线气溶胶散射因子</returns>
        private double CalculateUVAerosolFactor()
        {
            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double aerosolFactor = 3.91 / Visibility * Math.Pow(UV_WAVELENGTH / 0.55, -q);
            
            if (Visibility < 5)
            {
                aerosolFactor *= (1 + (5 - Visibility) / 5);
            }

            if (IsDusty)
            {
                aerosolFactor *= 1.5;
            }

            return Math.Max(1, aerosolFactor);
        }

        /// <summary>
        /// 计算雾对紫外线的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>雾对紫外线的衰减</returns>
        private double CalculateFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double beta = 3.91 / Visibility * Math.Pow(UV_WAVELENGTH / 0.55, -q);
            return beta * pathLength * 0.7; // UV在雾中的衰减可能比可见光更强
        }
    }
}
