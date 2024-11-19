/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现红外线在大气中的传输特性计算。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，采用光谱模型法计算红外线透过率。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 红外线传输模型类，用于计算红外线在大气中的传输特性
    /// </summary>
    internal class IRTransmittanceModel : TransmittanceModel
    {
        // 光谱分段参数
        private const double MIN_WAVELENGTH = 3.0;   // 红外波段最小波长（微米）
        private const double MAX_WAVELENGTH = 12.0;  // 红外波段最大波长（微米）
        private const int SPECTRAL_BANDS = 100;      // 光谱分段数
        private const double IR_WAVELENGTH = 10.0;   // 主要计算波长（微米）

        public IRTransmittanceModel(WeatherCondition weather) : base(weather) { }

        public override double CalculateTransmittance(double distance)
        {
            // 将距离转换为千米
            double distanceInKm = distance / 1000.0;

            // 计算各种衰减机制的总和
            double totalAttenuation = 0;
            double wavelengthStep = (MAX_WAVELENGTH - MIN_WAVELENGTH) / SPECTRAL_BANDS;

            for (int i = 0; i < SPECTRAL_BANDS; i++)
            {
                double wavelength = MIN_WAVELENGTH + i * wavelengthStep;
                double bandAttenuation = CalculateBandAttenuation(wavelength);
                double spectralWeight = GetSpectralWeight(wavelength);
                
                totalAttenuation += bandAttenuation * spectralWeight;
            }

            // 计算基本透过率
            double baseTransmittance = Math.Exp(-totalAttenuation * distanceInKm);

            // 应用天气条件的影响
            double weatherEffect = CalculateWeatherEffect(distance);
            double finalTransmittance = baseTransmittance * weatherEffect;

            return Math.Max(0, Math.Min(1, finalTransmittance));
        }

        private double CalculateBandAttenuation(double wavelength)
        {
            // 进一步增加衰减系数
            double rayleighScattering = CalculateRayleighScattering(wavelength) * 4.0;  // 从2.0改为4.0
            double mieScattering = CalculateMieScattering(wavelength) * 5.0;            // 从3.0改为5.0
            double molecularAbsorption = CalculateMolecularAbsorption(wavelength) * 4.0; // 从2.5改为4.0

            return rayleighScattering + mieScattering + molecularAbsorption;
        }

        private double CalculateRayleighScattering(double wavelength)
        {
            // Rayleigh散射系数计算（km^-1）
            // 进一步增加 Rayleigh 散射的影响
            double wavelengthMicrons = wavelength;
            return 0.02735 * Math.Pow(wavelengthMicrons, -4.08) * (Pressure / 1013.25) * (288.15 / Temperature);
        }

        private double CalculateMieScattering(double wavelength)
        {
            // Mie散射系数计算（km^-1）
            // 进一步增加 Mie 散射的影响
            double beta = 3.91 / Visibility;
            double alpha = 1.2 * Math.Pow(wavelength / 0.55, -1.3);
            return beta * alpha * (AerosolDensity / STANDARD_AEROSOL_DENSITY) * 3.5; // 从2.0改为3.5
        }

        private double CalculateMolecularAbsorption(double wavelength)
        {
            // 分子吸收系数计算（km^-1）
            double waterVaporAbsorption = CalculateWaterVaporAbsorption(wavelength);
            double co2Absorption = CalculateCO2Absorption(wavelength);
            
            return waterVaporAbsorption + co2Absorption;
        }

        private double CalculateWaterVaporAbsorption(double wavelength)
        {
            // 水汽吸收系数（km^-1）
            // 进一步增加水汽吸收的影响
            double waterVaporDensity = CalculateWaterVaporDensity();

            if (wavelength >= 5.5 && wavelength <= 7.5)
                return 2.4 * waterVaporDensity;  // 从1.2改为2.4
            else if (wavelength >= 2.5 && wavelength <= 3.5)
                return 1.8 * waterVaporDensity;  // 从0.9改为1.8
            else
                return 0.6 * waterVaporDensity;  // 从0.3改为0.6
        }

        private double CalculateWaterVaporDensity()
        {
            // 使用Magnus公式计算饱和水汽压
            double a = 17.27;
            double b = 237.7;
            double saturationVaporPressure = 6.112 * Math.Exp((a * (Temperature - 273.15)) / (b + (Temperature - 273.15)));
            
            // 计算实际水汽压
            double actualVaporPressure = (Humidity / 100.0) * saturationVaporPressure;
            
            // 计算水汽密度 (g/m³)
            return (actualVaporPressure * 2.16679) / Temperature;
        }

        private double CalculateCO2Absorption(double wavelength)
        {
            // CO2吸收系数（km^-1）
            // 增加CO2吸收的影响
            if (wavelength >= 4.2 && wavelength <= 4.4)
                return 1.2 * (CO2Concentration / 400.0);   // 从0.6改为1.2
            else if (wavelength >= 14.0 && wavelength <= 16.0)
                return 1.0 * (CO2Concentration / 400.0);   // 从0.5改为1.0
            else
                return 0.16 * (CO2Concentration / 400.0);  // 从0.08改为0.16
        }

        private double GetSpectralWeight(double wavelength)
        {
            // 调整权重分布
            if (wavelength >= 3.0 && wavelength <= 5.0)
                return 0.8;  // 从1.0改为0.8
            else if (wavelength >= 8.0 && wavelength <= 12.0)
                return 0.6;  // 从0.8改为0.6
            else
                return 0.2;  // 从0.3改为0.2
        }

        private double CalculateWeatherEffect(double distance)
        {
            // 降低天气效应的影响
            double rainEffect = CalculateRainAttenuation(distance, IR_WAVELENGTH) * 1.2;  // 从1.5改为1.2
            double snowEffect = CalculateSnowAttenuation(distance, IR_WAVELENGTH) * 1.2;  // 从1.5改为1.2
            double fogEffect = CalculateFogAttenuation(distance) * 1.5;                   // 从2.0改为1.5
            double dustEffect = CalculateDustAttenuation(distance) * 1.5;                 // 从2.0改为1.5

            return Math.Exp(-2.0 * (rainEffect + snowEffect + fogEffect + dustEffect));   // 从3.0改为2.0
        }

        protected override double CalculateRainKCoefficient(double wavelength)
        {
            if (wavelength >= 8 && wavelength <= 12)
            {
                // 对于远红外波段（8-12μm）
                return 0.187;
            }
            else if (wavelength >= 3 && wavelength <= 5)
            {
                // 对于中红外波段（3-5μm）
                return 0.2656;   
            }
            return 0;
        }

        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            if (wavelength >= 8 && wavelength <= 12)
            {
                // 对于远红外波段（8-12μm）
                return 0.784;
            }
            else if (wavelength >= 3 && wavelength <= 5)
            {
                // 对于中红外波段（3-5μm）
                return 0.7978;
            }
            return 0;
        }

        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            return 0.365;
        }

        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            return 0.88;
        }

        private double CalculateFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double beta = 3.91 / Visibility * Math.Pow(IR_WAVELENGTH / 0.55, -q);
            return beta * pathLength * 1.0; // 从1.2改为1.0
        }
    }
}
