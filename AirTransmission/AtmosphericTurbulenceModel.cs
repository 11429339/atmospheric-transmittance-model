/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现大气湍流对光传输影响的计算模型。作者：田建勇
 */

using System;

namespace AirTransmission
{
    /// <summary>
    /// 大气湍流模型类，用于计算大气湍流对光传输的影响
    /// </summary>
    public class AtmosphericTurbulenceModel
    {
        // 波数，假设波长为1.06微米（常用的激光波长）
        private const double k = 2 * Math.PI / 1.06e-6;

        /// <summary>
        /// 计算大气湍流对光传输的综合影响
        /// </summary>
        /// <param name="distance">传输距离（米）</param>
        /// <param name="height">传输高度（米）</param>
        /// <param name="windSpeed">风速（米/秒）</param>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <returns>湍流效应（0到1之间的值，1表示无影响，0表示完全衰减）</returns>
        public static double CalculateTurbulenceEffect(double distance, double height, double windSpeed, double C2n)
        {
            double r0 = CalculateFriedParameter(C2n, distance);
            double sigmaI2 = CalculateScintillationIndex(C2n, k, distance);
            double beamWander = CalculateBeamWander(C2n, distance, height);

            // 修改综合湍流效应计算
            double turbulenceEffect = Math.Exp(-sigmaI2) * (1 - Math.Exp(-r0 / beamWander));

            // 增加一个缩放因子，使效应更明显
            double scalingFactor = 0.8;
            turbulenceEffect = 1 - scalingFactor * (1 - turbulenceEffect);

            return turbulenceEffect;
        }

        /// <summary>
        /// 使用修改后的 Hufnagel-Valley 模型计算大气折射率结构常数
        /// </summary>
        /// <param name="height">高度（米）</param>
        /// <param name="windSpeed">风速（米/秒）</param>
        /// <returns>大气折射率结构常数</returns>
        public static double CalculateC2n(double height, double windSpeed)
        {
            // 修改 Hufnagel-Valley 模型，增加湍流强度
            double A = 1.7e-13; // 增加了一个数量级
            double v = windSpeed;
            return A * Math.Pow(v / 27, 2) * Math.Pow(height * 1e-5, 10) * Math.Exp(-height / 1000) 
                   + 2.7e-15 * Math.Exp(-height / 1500);
        }

        /// <summary>
        /// 计算弗里德参数（Fried parameter）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="L">传输距离（米）</param>
        /// <returns>弗里德参数（米）</returns>
        private static double CalculateFriedParameter(double C2n, double L)
        {
            return Math.Pow(0.423 * k * k * C2n * L, -3.0 / 5.0);
        }

        /// <summary>
        /// 计算闪烁指数（Scintillation Index）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="k">波数</param>
        /// <param name="L">传输距离（米）</param>
        /// <returns>闪烁指数（无量纲）</returns>
        private static double CalculateScintillationIndex(double C2n, double k, double L)
        {
            return 1.23 * C2n * Math.Pow(k, 7.0 / 6.0) * Math.Pow(L, 11.0 / 6.0);
        }

        /// <summary>
        /// 计算光束漂移（Beam Wander）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="L">传输距离（米）</param>
        /// <param name="h">传输高度（米）</param>
        /// <returns>光束漂移（弧度）</returns>
        private static double CalculateBeamWander(double C2n, double L, double h)
        {
            return 2.87 * Math.Pow(C2n * L * Math.Pow(h, 5.0/3.0), 1.0/3.0);
        }

        /// <summary>
        /// 计算相干长度（Coherence Length）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="L">传输距离（米）</param>
        /// <returns>相干长度（米）</returns>
        private static double CalculateCoherenceLength(double C2n, double L)
        {
            return Math.Pow(1.46 * k * k * C2n * L, -3.0 / 5.0);
        }

        /// <summary>
        /// 计算到达角（Angle of Arrival）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="L">传输距离（米）</param>
        /// <param name="D">接收器口径（米）</param>
        /// <returns>到达角（弧度）</returns>
        public static double CalculateAngleOfArrival(double C2n, double L, double D)
        {
            return 2.91 * Math.Pow(C2n * L / Math.Pow(D, 1.0/3.0), 0.6);
        }

        /// <summary>
        /// 计算等晕角（Isoplanatism Angle）
        /// </summary>
        /// <param name="C2n">大气折射率结构常数</param>
        /// <param name="L">传输距离（米）</param>
        /// <returns>等晕角（弧度）</returns>
        public static double CalculateIsoplanatismAngle(double C2n, double L)
        {
            return 0.314 * Math.Pow(C2n * k * k * L, -3.0 / 5.0);
        }
    }
}
