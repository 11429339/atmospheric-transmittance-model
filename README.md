# 大气透过率计算器

## 项目简介

大气透过率计算器是一个用于模拟和计算各种电磁波（包括激光、红外线、毫米波和紫外线）在不同大气条件下透过率的工具。本项目考虑了多种天气因素和大气湍流的影响，为光学和通信系统的设计与分析提供了有力支持。

## 功能特性

- 支持多种电磁波类型：激光、红外线、毫米波和紫外线
- 考虑多种天气条件：晴天、雨天、雪天、雾天、沙尘天气
- 模拟大气湍流对光传输的影响
- 计算烟幕对透过率的影响
- 灵活配置传输距离和天气参数
- 提供详细的计算过程和结果输出

## 安装说明

1. 确保您的系统已安装 .NET Core 3.1 或更高版本。
2. 克隆此仓库到本地机器：   ```
   git clone https://github.com/11429339/atmospheric-transmittance-model.git   ```
3. 进入项目目录：   ```
   cd atmospheric-transmittance-model   ```
4. 编译项目：   ```
   dotnet build   ```

## 使用方法

1. 在项目根目录下运行程序：   ```
   dotnet run   ```
2. 程序将自动执行一系列预设的测试场景，包括不同天气条件和传输距离。
3. 查看控制台输出，了解各种条件下的透过率计算结果。

### 示例代码

如果您想在自己的程序中使用此计算器，可以参考Program.cs中的代码

## 项目结构

- `Program.cs`: 主程序入口，包含各种测试方法
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
