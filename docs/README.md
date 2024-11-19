# 大气透过率计算库

## 项目简介

大气透过率计算库是一个用于模拟和计算各种电磁波（包括激光、红外线、毫米波和紫外线）在不同大气条件下透过率的工具。本项目考虑了多种天气因素和大气湍流的影响，为光学和通信系统的设计与分析提供了有力支持。

## 功能特性

- 支持多种电磁波类型：激光、红外线、毫米波和紫外线
  - 激光 (1.06μm)
  - 红外 (3-12μm)
  - 紫外 (0.2-0.4μm)
  - 毫米波 (94GHz)
- 考虑多种天气条件：晴天、雨天、雪天、雾天、沙尘天气
- 模拟大气湍流对光传输的影响
- 计算烟幕对透过率的影响
- 灵活配置传输距离和天气参数
- 提供详细的计算过程和结果输出

## 安装说明

1. 确保您的系统已安装 .NET 7 或更高版本。
2. 克隆此仓库到本地机器：   ```
   git clone https://github.com/11429339/atmospheric-transmittance-model.git ```
3. 进入项目目录：   ```
   cd atmospheric-transmittance-model ```
4. 编译项目：   ```
   dotnet build ```

## 快速开始

```csharp
// 创建天气条件
var weather = new WeatherCondition(
    type: WeatherType.晴天, // 天气类型
    temperature: 20, // 温度(℃)
    relativeHumidity: 0.45, // 相对湿度
    visibility: 23000, // 能见度(米)
    precipitation: 0 // 降雨量(mm/h)
);

// 计算激光透过率
double distance = 1000; // 1000米
double transmittance = AtmosphericTransmittanceCalculator.CalcLaser(weather, distance);
Console.WriteLine($"激光透过率: {transmittance:F4}");
```

## 项目结构

- `AtmosphericTransmittanceCalculator.cs`: 主要的计算接口
- `TransmittanceModel.cs`: 透过率模型的基类
- `LaserTransmittanceModel.cs`: 激光透过率模型
- `IRTransmittanceModel.cs`: 红外线透过率模型
- `MillimeterWaveTransmittanceModel.cs`: 毫米波透过率模型
- `UVTransmittanceModel.cs`: 紫外线透过率模型
- `AtmosphericTurbulenceModel.cs`: 大气湍流模型

## 版本历史

- 1.0.0 (2024-10-13): 初始版本发布

## 作者

田建勇

## 许可证

本项目采用 MIT 许可证。详情请见 [LICENSE](LICENSE) 文件。

## 联系方式

如有任何问题或建议，请通过以下方式联系我们：

- 电子邮件：<11429339@qq.com>
- 项目 Issues：<https://github.com/11429339/atmospheric-transmittance-model/issues>

## 致谢

感谢所有为本项目做出贡献的开发者和研究人员。
