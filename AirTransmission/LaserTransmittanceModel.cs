/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现激光在大气中的传输特性计算，包括湍流效应。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，考虑目标位置关系和大气参数分布。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 激光传输模型类，用于计算激光在大气中的传输特性
    /// </summary>
    public class LaserTransmittanceModel(WeatherCondition weather) : TransmittanceModel(weather)
    {
        /// <summary>
        /// 激光波长（微米）
        /// </summary>
        private const double LASER_WAVELENGTH = 1.06;

        /// <summary>
        /// 计算给定距离的激光透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>激光透过率（0到1之间的值）</returns>
        public override double CalculateTransmittance(double distance)
        {
            double attenuationFactor = CalculateAttenuationFactor();
            double rainAttenuation = CalculateRainAttenuation(distance, LASER_WAVELENGTH);
            double snowAttenuation = CalculateSnowAttenuation(distance, LASER_WAVELENGTH);
            double fogAttenuation = CalculateFogAttenuation(distance);
            double dustAttenuation = CalculateDustAttenuation(distance);
            
            double transmittance = Math.Pow(STANDARD_TRANSMITTANCE, attenuationFactor * distance) * 
                   Math.Exp(-rainAttenuation - snowAttenuation - fogAttenuation - dustAttenuation);

            // 确保透过率在有效范围内
            transmittance = Math.Max(0, Math.Min(1, transmittance));

            return transmittance;
        }

        /// <summary>
        /// 计算考虑湍流效应的激光透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>考虑湍流效应的激光透过率</returns>
        public double CalculateTransmittanceWithTurbulence(double distance)
        {
            double transmittance = CalculateTransmittance(distance);
            return ApplyTurbulenceEffect(transmittance, distance);
        }

        /// <summary>
        /// 计算雨对激光的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数K</returns>
        protected override double CalculateRainKCoefficient(double wavelength)
        {
            // 对于1.06μm激光
            return 0.25;
        }

        /// <summary>
        /// 计算雨对激光的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雨衰减系数α</returns>
        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            // 对于1.06μm激光
            return 0.659;
        }

        /// <summary>
        /// 计算雪对激光的衰减系数K
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数K</returns>
        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            if (wavelength == LASER_WAVELENGTH)
                return 0.56;
            else if (wavelength == 10.6)
                return 0.8;
            else
                return 0;
        }

        /// <summary>
        /// 计算雪对激光的衰减系数α
        /// </summary>
        /// <param name="wavelength">波长（微米）</param>
        /// <returns>雪衰减系数α</returns>
        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            if (wavelength == LASER_WAVELENGTH)
                return 0.57;
            else if (wavelength == 10.6)
                return 0.75;
            else
                return 0;
        }

        /// <summary>
        /// 计算激光的总衰减因子
        /// </summary>
        /// <returns>激光总衰减因子</returns>
        private double CalculateAttenuationFactor()
        {
            double molecularFactor = CalculateMolecularFactor();
            double aerosolFactor = CalculateAerosolFactor();
            double visibilityFactor = CalculateVisibilityFactor();

            // 增加基础衰减
            return molecularFactor * aerosolFactor * visibilityFactor * 1.2; // 增加1.2倍的基础衰减
        }

        /// <summary>
        /// 计算分子散射因子
        /// </summary>
        /// <returns>分子散射因子</returns>
        private double CalculateMolecularFactor()
        {
            // 增加分子散射的影响
            return (Pressure / 1013.25) * (288.15 / Temperature) * 1.15; // 增加15%的分子散射
        }

        /// <summary>
        /// 计算气溶胶散射因子
        /// </summary>
        /// <returns>气溶胶散射因子</returns>
        private double CalculateAerosolFactor()
        {
            double q;
            if (Visibility > 50)
                q = 1.6;
            else if (Visibility > 6)
                q = 1.3;
            else if (Visibility > 1)
                q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            else
                q = 0.585 * Math.Pow(1, 1.0/3.0);

            // 增加气溶胶散射的影响
            double aerosolFactor = 3.91 / Visibility * Math.Pow(LASER_WAVELENGTH / 0.55, -q) * 1.1; // 增加10%的气溶胶散射
            
            if (Visibility < 5)
            {
                aerosolFactor *= (1 + (5 - Visibility) / 5);
            }

            if (IsDusty)
            {
                aerosolFactor *= 1.3;
            }

            return Math.Max(1, aerosolFactor);
        }

        /// <summary>
        /// 计算雾对激光的衰减
        /// </summary>
        /// <param name="pathLength">传输路径长度（米）</param>
        /// <returns>雾对激光的衰减</returns>
        private double CalculateFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            // 增加雾的衰减效应
            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double beta = 3.91 / Visibility * Math.Pow(LASER_WAVELENGTH / 0.55, -q);
            return beta * pathLength * 1.2; // 从0.5改为1.2，增加雾的影响
        }

        /// <summary>
        /// 计算考虑烟雾的激光透过率
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <param name="smokeConcentration">烟雾浓度</param>
        /// <param name="smokeThickness">烟雾厚度（米）</param>
        /// <returns>考虑烟雾的激光透过率</returns>
        public double CalculateTransmittanceWithSmoke(double distance, double smokeConcentration, double smokeThickness)
        {
            double transmittance = CalculateTransmittance(distance);
            double smokeTransmittance = CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
            return transmittance * smokeTransmittance;
        }

        /// <summary>
        /// 应用湍流效应到透过率
        /// </summary>
        /// <param name="transmittance">原始透过率</param>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>考虑湍流效应后的透过率</returns>
        protected static double ApplyTurbulenceEffect(double transmittance, double distance)
        {
            double height = 10; // 假设光束平均高度为10米
            double windSpeed = 5; // 假设风速为5 m/s
            double C2n = AtmosphericTurbulenceModel.CalculateC2n(height, windSpeed);

            double turbulenceEffect = AtmosphericTurbulenceModel.CalculateTurbulenceEffect(distance * 1000, height, windSpeed, C2n);
            
            // 修改湍流效应的应用方式
            return transmittance * (0.7 + 0.3 * turbulenceEffect);
        }
    }
}
