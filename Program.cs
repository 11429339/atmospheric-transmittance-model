/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现基本的大气透过率计算和测试功能。作者：田建勇
 */

using AirTransmission;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("脉冲激光目标指示器辐射能量计算");

        // 创建一个晴天的天气条件
        var weatherCondition = new WeatherCondition(
            type: WeatherType.晴天,
            temperature: 25,
            relativeHumidity: 60,
            visibility: 10
        );

        // 设置激光和目标参数
        double laserEnergy = 0.130; // 130 mJ
        double pulseWidth = 15; // 15 ns
        double targetDistance = 1; // 1公里
        double receiverDistance = 1; // 1公里
        double targetReflectivity = 0.2; // 假设目标反射率为20%

        // 计算激光透过率和接收功率
        double transmittance = AtmosphericTransmittanceCalculator.CalcLaser(weatherCondition, targetDistance);
        double receivedPower = AtmosphericTransmittanceCalculator.CalculateReceivedRadiation(transmittance, laserEnergy, pulseWidth, targetDistance, receiverDistance, targetReflectivity);

        Console.WriteLine($"\n导弹导引头接收到的平均功率: {receivedPower:E2} W");

        // 执行各种测试
        Console.WriteLine("\n单程辐射能量计算");
        CalculateSinglePathRadiation();

        Console.WriteLine("\n烟幕影响计算");
        TestSmokeScreenEffect();

        Console.WriteLine("\n不同天气条件下的激光透过率测试");
        TestLaserTransmittanceInDifferentWeather();

        Console.WriteLine("\n不同天气条件下的红外线透过率测试");
        TestIRTransmittanceInDifferentWeather();

        Console.WriteLine("\n不同天气条件下的毫米波透过率测试");
        TestMillimeterWaveTransmittanceInDifferentWeather();

        Console.WriteLine("\n不同天气条件下的紫外线透过率测试");
        TestUVTransmittanceInDifferentWeather();

        Console.WriteLine("\n湍流效应对激光透过率的影响");
        TestTurbulenceEffect();
    }

    // 测试不同天气条件下的激光透过率
    static void TestLaserTransmittanceInDifferentWeather()
    {
        // 测试各种天气条件
        TestLaserTransmittance(WeatherType.晴天, "晴朗");
        TestLaserTransmittance(WeatherType.雨天, "小雨", relativeHumidity: 60, precipitation: 2.5, visibility: 5);
        TestLaserTransmittance(WeatherType.雨天, "大雨", relativeHumidity: 90, precipitation: 25, visibility: 2.5);
        TestLaserTransmittance(WeatherType.雾天, "雾", relativeHumidity: 80, visibility: 5);
        TestLaserTransmittance(WeatherType.沙尘, "沙尘", visibility: 0.5);
        TestLaserTransmittance(WeatherType.雪天, "雪天", temperature: -5, relativeHumidity: 80, visibility: 1, precipitation: 2.5);
    }

    // 测试特定天气条件下的激光透过率
    static void TestLaserTransmittance(WeatherType type, string description, 
                                     double temperature = 25, double relativeHumidity = 50, 
                                     double visibility = 23, double? precipitation = null, double latitude = 30)
    {
        double[] distances = [0.1, 0.5, 1, 5, 10]; // 测试不同距离（公里）

        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: type,
            temperature: temperature,
            relativeHumidity: relativeHumidity,
            visibility: visibility,
            precipitation: precipitation
        );

        // 输出结果
        Console.WriteLine($"\n[{description}条件下的激光透过率]");
        TransmittanceModel.PrintWeatherInfo(weatherCondition);
        foreach (var d in distances){
            double laserTransmittance = AtmosphericTransmittanceCalculator.CalcLaser(weatherCondition, d);
            Console.WriteLine($"距离：{d}公里, 1.06μm激光透过率为: {laserTransmittance:P2}");
        }
    }

    // 计算单程辐射能量
    static void CalculateSinglePathRadiation()
    {
        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: WeatherType.晴天,
            temperature: 25,
            relativeHumidity: 60,
            visibility: 10
        );

        double targetRadiation = 2E+006; // 100 W/Sr
        double receiverDistance = 3; // 3公里

        // 计算透过率和接收功率
        double transmittance = AtmosphericTransmittanceCalculator.CalcLaser(weatherCondition, receiverDistance);
        double receivedPower = AtmosphericTransmittanceCalculator.CalculateReceivedRadiationSinglePath(transmittance, targetRadiation, receiverDistance);

        Console.WriteLine($"\n导弹导引头接收到的辐射能量: {receivedPower:E2} W/Sr");
    }

    // 测试特定天气条件下的红外线透过率
    static void TestIRTransmittance(WeatherType type, string description, 
                                double temperature = 25, double relativeHumidity = 50, 
                                double visibility = 10, double? precipitation = null, double latitude = 30)
    {
        double[] distances = [0.1, 0.5, 1, 5, 10]; // 测试不同距离（公里）

        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: type,
            temperature: temperature,
            relativeHumidity: relativeHumidity,
            visibility: visibility,
            precipitation: precipitation
        );

        // 输出结果
        Console.WriteLine($"\n[{description}条件下的红外线透过率]");
        TransmittanceModel.PrintWeatherInfo(weatherCondition);
        foreach (var d in distances)
        {
            double irTransmittance = AtmosphericTransmittanceCalculator.CalcIR(weatherCondition, d);
            Console.WriteLine($"距离：{d}公里, 红外线透过率为: {irTransmittance:P2}");
        }
    }

    // 测试烟幕效应
    static void TestSmokeScreenEffect()
    {
        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: WeatherType.晴天,
            temperature: 25,
            relativeHumidity: 60,
            visibility: 10
        );

        double smokeConcentration = 0.5; // 假设烟幕浓度为0.5 g/m³
        double smokeThickness = 15; // 假设烟幕墙厚度为5米

        // 计算烟幕透过率
        double smokeScreenTransmittance = TransmittanceModel.CalculateSmokeScreenTransmittance(smokeConcentration, smokeThickness);
        Console.WriteLine($"\n仅考虑烟幕的透过率: {smokeScreenTransmittance:P2}");
    }

    // 测试不同天气条件下的毫米波透过率
    static void TestMillimeterWaveTransmittanceInDifferentWeather()
    {
        // 测试各种天气条件
        TestMillimeterWaveTransmittance(WeatherType.晴天, "晴朗");
        TestMillimeterWaveTransmittance(WeatherType.雨天, "小雨", relativeHumidity: 60, precipitation: 2.5, visibility: 5);
        TestMillimeterWaveTransmittance(WeatherType.雨天, "大雨", relativeHumidity: 90, precipitation: 25, visibility: 2.5);
        TestMillimeterWaveTransmittance(WeatherType.雾天, "雾", relativeHumidity: 70, visibility: 1);
        TestMillimeterWaveTransmittance(WeatherType.沙尘, "沙尘", visibility: 0.5);
        TestMillimeterWaveTransmittance(WeatherType.雪天, "雪天", temperature: -5, relativeHumidity: 80, visibility: 1, precipitation: 5);
    }

    // 测试特定天气条件下的毫米波透过率
    static void TestMillimeterWaveTransmittance(WeatherType type, string description, 
                                            double temperature = 25, double relativeHumidity = 50, 
                                            double visibility = 10, double? precipitation = null)
    {
        double[] distances = [0.1, 0.5, 1, 5, 10]; // 测试不同距离（公里）

        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: type,
            temperature: temperature,
            relativeHumidity: relativeHumidity,
            visibility: visibility,
            precipitation: precipitation
        );

        // 输出结果
        Console.WriteLine($"\n[{description}条件下的毫米波透过率]");
        TransmittanceModel.PrintWeatherInfo(weatherCondition);
        foreach (var d in distances)
        {
            double mmWaveTransmittance = AtmosphericTransmittanceCalculator.CalcMillimeterWave(weatherCondition, d);
            Console.WriteLine($"距离：{d}公里, 毫米波透过率为: {mmWaveTransmittance:P2}");
        }
    }

    // 测试不同天气条件下的红外线透过率
    static void TestIRTransmittanceInDifferentWeather()
    {
        // 测试各种天气条件
        TestIRTransmittance(WeatherType.晴天, "晴朗");
        TestIRTransmittance(WeatherType.雨天, "小雨", relativeHumidity: 60, precipitation: 2.5, visibility: 5);
        TestIRTransmittance(WeatherType.雨天, "大雨", relativeHumidity: 90, precipitation: 25, visibility: 2.5);
        TestIRTransmittance(WeatherType.雾天, "雾", relativeHumidity: 70, visibility: 1);
        TestIRTransmittance(WeatherType.沙尘, "沙尘", visibility: 0.5);
        TestIRTransmittance(WeatherType.雪天, "雪天", temperature: -5, relativeHumidity: 80, visibility: 1, precipitation: 5);
    }

    // 测试不同天气条件下的紫外线透过率
    static void TestUVTransmittanceInDifferentWeather()
    {
        // 测试各种天气条件
        TestUVTransmittance(WeatherType.晴天, "晴朗");
        TestUVTransmittance(WeatherType.雨天, "小雨", relativeHumidity: 60, precipitation: 2.5, visibility: 5);
        TestUVTransmittance(WeatherType.雨天, "大雨", relativeHumidity: 90, precipitation: 25, visibility: 2.5);
        TestUVTransmittance(WeatherType.雾天, "雾", relativeHumidity: 70, visibility: 1);
        TestUVTransmittance(WeatherType.沙尘, "沙尘", visibility: 0.5);
        TestUVTransmittance(WeatherType.雪天, "雪天", temperature: -5, relativeHumidity: 80, visibility: 1, precipitation: 5);
    }

    // 测试特定天气条件下的紫外线透过率
    static void TestUVTransmittance(WeatherType type, string description, 
                                    double temperature = 25, double relativeHumidity = 50, 
                                    double visibility = 10, double? precipitation = null)
    {
        double[] distances = [0.1, 0.5, 1, 5, 10]; // 测试不同距离（公里）

        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: type,
            temperature: temperature,
            relativeHumidity: relativeHumidity,
            visibility: visibility,
            precipitation: precipitation
        );

        // 输出结果
        Console.WriteLine($"\n[{description}条件下的紫外线透过率]");
        TransmittanceModel.PrintWeatherInfo(weatherCondition);
        foreach (var d in distances)
        {
            double uvTransmittance = AtmosphericTransmittanceCalculator.CalcUV(weatherCondition, d);
            Console.WriteLine($"距离：{d}公里, 紫外线透过率为: {uvTransmittance:P2}");
        }
    }

    // 测试湍流效应对激光透过率的影响
    static void TestTurbulenceEffect()
    {
        // 创建天气条件
        var weatherCondition = new WeatherCondition(
            type: WeatherType.晴天,
            temperature: 25,
            relativeHumidity: 60,
            visibility: 10
        );

        var laserModel = new LaserTransmittanceModel(weatherCondition);

        double[] distances = [0.1, 0.5, 1, 5, 10]; // 测试不同距离（公里）

        // 输出结果
        Console.WriteLine("\n[湍流效应对激光透过率的影响]");
        foreach (var d in distances)
        {
            double transmittanceWithoutTurbulence = laserModel.CalculateTransmittance(d);
            double transmittanceWithTurbulence = laserModel.CalculateTransmittanceWithTurbulence(d);
            Console.WriteLine($"距离：{d}公里, 无湍流透过率: {transmittanceWithoutTurbulence:P2}, 有湍流透过率: {transmittanceWithTurbulence:P2}");
        }
    }
}
