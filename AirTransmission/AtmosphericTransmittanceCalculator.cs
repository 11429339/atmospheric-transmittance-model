using System;

namespace AirTransmission
{
    /// <summary>
    /// 提供各种大气传输模型的计算方法
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
            double smokeTransmittance = TransmittanceModel.CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
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
            double smokeTransmittance = TransmittanceModel.CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
            return transmittance * smokeTransmittance;
        }

        /// <summary>
        /// 计算接收到的辐射功率
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
        /// 计算单程传输中接收到的辐射功率
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
    }
}
