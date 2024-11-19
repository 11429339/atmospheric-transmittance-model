/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，提供各种大气传输模型的计算方法。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，考虑目标位置关系和大气参数分布。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 大气透过率计算器
    /// </summary>
    public static class AtmosphericTransmittanceCalculator
    {
        /// <summary>
        /// 计算激光在给定天气条件和距离下的大气透过率
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcLaser(WeatherCondition weather, double distance)
        {
            var model = new LaserTransmittanceModel(weather);
            return model.CalculateTransmittance(distance);
        }

        /// <summary>
        /// 计算激光在有烟雾条件下的大气透过率
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <param name="smokeConcentration">烟雾浓度</param>
        /// <param name="smokeThickness">烟雾厚度（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcLaserWithSmoke(WeatherCondition weather, double distance, double smokeConcentration = 0, double smokeThickness = 0)
        {
            var model = new LaserTransmittanceModel(weather);
            return model.CalculateTransmittanceWithSmoke(distance, smokeConcentration, smokeThickness);
        }

        /// <summary>
        /// 计算红外线在给定条件下的大气透过率
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <param name="smokeConcentration">烟雾浓度</param>
        /// <param name="smokeThickness">烟雾厚度（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcIR(WeatherCondition weather, double distance, double smokeConcentration = 0, double smokeThickness = 0)
        {
            var model = new IRTransmittanceModel(weather);
            double transmittance = model.CalculateTransmittance(distance);
            double smokeTransmittance = CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
            return transmittance * smokeTransmittance;
        }

        /// <summary>
        /// 计算毫米波在给定条件下的大气透过率
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <param name="smokeConcentration">烟雾浓度</param>
        /// <param name="smokeThickness">烟雾厚度（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcMillimeterWave(WeatherCondition weather, double distance, double smokeConcentration = 0, double smokeThickness = 0)
        {
            var model = new MillimeterWaveTransmittanceModel(weather);
            double transmittance = model.CalculateTransmittance(distance);
            double smokeTransmittance = CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
            return transmittance * smokeTransmittance;
        }

        /// <summary>
        /// 计算双程传输后接收到的辐射功率
        /// </summary>
        /// <param name="transmittance">大气透过率</param>
        /// <param name="laserEnergy">激光能量（焦耳）</param>
        /// <param name="pulseWidth">脉冲宽度（纳秒）</param>
        /// <param name="targetDistance">目标距离（米）</param>
        /// <param name="receiverDistance">接收器距离（米）</param>
        /// <param name="targetReflectivity">目标反射率</param>
        /// <returns>接收到的辐射功率（瓦特）</returns>
        public static double CalculateReceivedRadiation(double transmittance, double laserEnergy, double pulseWidth, double targetDistance, double receiverDistance, double targetReflectivity)
        {
            double totalDistance = targetDistance + receiverDistance;
            // 计算峰值功率（W）
            double peakPower = laserEnergy / (pulseWidth * 1e-9);

            // 假设坦克为朗伯反射体
            double reflectedPower = peakPower * targetReflectivity / Math.PI;

            // 计算立体角（假设导弹导引头接收面积为10cm^2）
            double receiverArea = 0.001; // 10cm^2 转换为m^2
            double solidAngle = receiverArea / (receiverDistance * receiverDistance);

            // 计算接收到的能量（J）
            double receivedEnergy = reflectedPower * transmittance * solidAngle * (pulseWidth * 1e-9);

            // 输出计算过程中的关键参数
            Console.WriteLine($"Laser energy: {laserEnergy:F2} J");
            Console.WriteLine($"Pulse width: {pulseWidth} ns");
            Console.WriteLine($"Peak power: {peakPower:E2} W");
            Console.WriteLine($"Target distance: {targetDistance} km");
            Console.WriteLine($"Target reflected power: {reflectedPower:E2} W");
            Console.WriteLine($"Receiver distance: {receiverDistance} km");
            Console.WriteLine($"Double path transmittance: {transmittance:F4}");
            Console.WriteLine($"Received energy: {receivedEnergy:E2} J");
            Console.WriteLine($"Received power: {receivedEnergy / (pulseWidth * 1e-9):E2} W");

            return receivedEnergy / (pulseWidth * 1e-9); // 返回接收功率（W）
        }

        /// <summary>
        /// 计算单程传输后接收到的辐射功率
        /// </summary>
        /// <param name="transmittance">大气透过率</param>
        /// <param name="targetRadiation">目标辐射（W/Sr）</param>
        /// <param name="receiverDistance">接收器距离（米）</param>
        /// <returns>接收到的辐射功率（W/Sr）</returns>
        public static double CalculateReceivedRadiationSinglePath(double transmittance, double targetRadiation, double receiverDistance)
        {
            // 计算立体角（假设导弹导引头接收面积为10cm^2）
            double receiverArea = 0.001; // 10cm^2 转换为m^2
            double solidAngle = receiverArea / (receiverDistance * receiverDistance);

            double receivedPower = targetRadiation * transmittance * solidAngle;

            // 输出计算过程中的关键参数
            Console.WriteLine($"Target radiation: {targetRadiation} W/Sr");
            Console.WriteLine($"Receiver distance: {receiverDistance} m");
            Console.WriteLine($"Single path transmittance: {transmittance:F4}");
            Console.WriteLine($"Received power: {receivedPower:E2} W/Sr");

            return receivedPower;
        }

        /// <summary>
        /// 计算紫外线在给定天气条件和距离下的大气透过率
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcUV(WeatherCondition weather, double distance)
        {
            var model = new UVTransmittanceModel(weather);
            return model.CalculateTransmittance(distance);
        }

        /// <summary>
        /// 计算湍流效应对激光透过率的影响
        /// </summary>
        /// <param name="weather">天气条件</param>
        /// <param name="distance">传输距离（米）</param>
        /// <returns>大气透过率</returns>
        public static double CalcTurbulenceEffect(WeatherCondition weather, double distance)
        {
            var model = new LaserTransmittanceModel(weather);
            return model.CalculateTransmittanceWithTurbulence(distance);
        }

        /// <summary>
        /// 计算烟幕对电磁波的透过率
        /// </summary>
        /// <param name="smokeConcentration">烟幕浓度（g/m³）</param>
        /// <param name="smokeThickness">烟幕厚度（米）</param>
        /// <returns>烟幕透过率（0到1之间的值）</returns>
        public static double CalculateSmokeScreenTransmittance(double smokeConcentration, double smokeThickness)
        {
            if (smokeConcentration <= 0 || smokeThickness <= 0)
                return 1; // 如果没有烟幕，返回1（无衰减）

            // 烟幕衰减系数（假设值，需要根据实际烟幕特性调整）
            double smokeAttenuationCoefficient = 0.5;

            // 使用Beer-Lambert定律计算透过率
            double transmittance = Math.Exp(-smokeAttenuationCoefficient * smokeConcentration * smokeThickness);

            Console.WriteLine($"烟幕透过率计算:");
            Console.WriteLine($"烟幕浓度: {smokeConcentration:F2} g/m³");
            Console.WriteLine($"烟幕厚度: {smokeThickness:F2} m");
            Console.WriteLine($"烟幕透过率: {transmittance:F4}");

            return transmittance;
        }
    }
}
