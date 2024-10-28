/*
 * 版本历史：
 * 1.0.0 (2024-10-13): 初始版本，实现紫外线在大气中的传输特性计算。作者：田建勇
 * 1.1.0 (2024-10-18): 改进版本，采用光谱模型法计算紫外线透过率。作者：田建勇
 */

namespace AirTransmission
{
    /// <summary>
    /// 紫外线传输模型类，用于计算紫外线在大气中的传输特性
    /// </summary>
    public class UVTransmittanceModel : TransmittanceModel
    {
        // 光谱分段参数
        private const double MIN_WAVELENGTH = 0.2;  // 紫外波段最小波长（微米）
        private const double MAX_WAVELENGTH = 0.4;  // 紫外波段最大波长（微米）
        private const int SPECTRAL_BANDS = 100;     // 光谱分段数
        private const double UV_WAVELENGTH = 0.308; // 主要计算波长（微米）

        public UVTransmittanceModel(WeatherCondition weather) : base(weather) { }

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
            // 计算各种衰减机制
            double rayleighScattering = CalculateRayleighScattering(wavelength);
            double mieScattering = CalculateMieScattering(wavelength);
            double molecularAbsorption = CalculateMolecularAbsorption(wavelength);

            // 返回总衰减系数（km^-1）
            return rayleighScattering + mieScattering + molecularAbsorption;
        }

        private double CalculateRayleighScattering(double wavelength)
        {
            // Rayleigh散射系数计算（km^-1）
            double wavelengthMicrons = wavelength;
            return 0.008735 * Math.Pow(wavelengthMicrons, -4.08) * (Pressure / 1013.25) * (288.15 / Temperature);
        }

        private double CalculateMieScattering(double wavelength)
        {
            // Mie散射系数计算（km^-1）
            double beta = 3.91 / Visibility;
            double alpha = 1.2 * Math.Pow(wavelength / 0.55, -1.3);
            return beta * alpha * (AerosolDensity / STANDARD_AEROSOL_DENSITY);
        }

        private double CalculateMolecularAbsorption(double wavelength)
        {
            // 分子吸收系数计算（km^-1）
            double o2Absorption = CalculateO2Absorption(wavelength);
            double o3Absorption = CalculateO3Absorption(wavelength);
            
            return o2Absorption + o3Absorption;
        }

        private double CalculateO2Absorption(double wavelength)
        {
            // O₂吸收系数（km^-1）
            if (wavelength < 0.25)
                return 0.5;
            return 0.1;
        }

        private double CalculateO3Absorption(double wavelength)
        {
            // O₃吸收系数（km^-1）
            if (wavelength >= 0.2 && wavelength <= 0.3)
                return 0.8;
            return 0.2;
        }

        private double GetSpectralWeight(double wavelength)
        {
            // 调整光谱权重分布
            if (wavelength >= 0.28 && wavelength <= 0.32)
                return 1.0;
            else if (wavelength > 0.32 && wavelength <= 0.38)
                return 0.6;
            return 0.3;
        }

        private double CalculateWeatherEffect(double distance)
        {
            // 增加天气效应的影响
            double rainEffect = CalculateRainAttenuation(distance, UV_WAVELENGTH) * 0.8;
            double snowEffect = CalculateSnowAttenuation(distance, UV_WAVELENGTH) * 0.8;
            double fogEffect = CalculateFogAttenuation(distance) * 0.6;
            double dustEffect = CalculateDustAttenuation(distance) * 0.6;

            return Math.Exp(-rainEffect - snowEffect - fogEffect - dustEffect);
        }

        protected override double CalculateRainKCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.4715;
        }

        protected override double CalculateRainAlphaCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.6296;
        }

        protected override double CalculateSnowKCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.5225;
        }

        protected override double CalculateSnowAlphaCoefficient(double wavelength)
        {
            // 对于UV波段 (308 nm)
            return 0.7937;
        }

        private double CalculateFogAttenuation(double pathLength)
        {
            if (!IsFoggy)
                return 0;

            double q = 0.585 * Math.Pow(Visibility, 1.0/3.0);
            double beta = 3.91 / Visibility * Math.Pow(UV_WAVELENGTH / 0.55, -q);
            return beta * pathLength * 0.2;
        }
    }
}
