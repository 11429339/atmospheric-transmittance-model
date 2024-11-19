# 大气透过率计算库 API 文档

## 主要类和接口

### AtmosphericTransmittanceCalculator

静态类，提供各种电磁波透过率的计算方法。

#### 方法

##### CalcLaser

```csharp
public static double CalcLaser(WeatherCondition weather, double distance)
```

计算激光（1.06μm）在给定天气条件和距离下的大气透过率。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
- 返回值：0-1之间的透过率值

##### CalcLaserWithSmoke

```csharp
public static double CalcLaserWithSmoke(WeatherCondition weather, double distance, double smokeConcentration, double smokeThickness)
```

计算有烟雾条件下的激光透过率。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
  - smokeConcentration: 烟雾浓度（g/m³）
  - smokeThickness: 烟雾厚度（米）
- 返回值：0-1之间的透过率值

##### CalcTurbulenceEffect

```csharp
public static double CalcTurbulenceEffect(WeatherCondition weather, double distance)
```

计算湍流效应对激光透过率的影响。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
- 返回值：0-1之间的透过率值

##### CalcIR

```csharp
public static double CalcIR(WeatherCondition weather, double distance)
```

计算红外线（3-12μm）透过率。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
- 返回值：0-1之间的透过率值

##### CalcUV

```csharp
public static double CalcUV(WeatherCondition weather, double distance)
```

计算紫外线（0.2-0.4μm）透过率。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
- 返回值：0-1之间的透过率值

##### CalcMillimeterWave

```csharp
public static double CalcMillimeterWave(WeatherCondition weather, double distance)
```

计算毫米波（94GHz）透过率。

- 参数：
  - weather: 天气条件
  - distance: 传输距离（米）
- 返回值：0-1之间的透过率值

##### CalculateSmokeScreenTransmittance

```csharp
public static double CalculateSmokeScreenTransmittance(double smokeConcentration, double smokeThickness)
```

计算烟幕对电磁波的透过率。

- 参数：
  - smokeConcentration: 烟雾浓度（g/m³）
  - smokeThickness: 烟雾厚度（米）
- 返回值：0-1之间的透过率值

##### CalculateReceivedRadiationSinglePath

```csharp
public static double CalculateReceivedRadiationSinglePath(double transmittance, double targetRadiation, double receiverDistance)
```

计算单程传输后接收到的辐射功率。

- 参数：
  - transmittance: 大气透过率
  - targetRadiation: 目标辐射（W/Sr）
  - receiverDistance: 接收器距离（米）
- 返回值：接收到的辐射功率（W/Sr）

##### CalculateReceivedRadiation

```csharp
public static double CalculateReceivedRadiation(double transmittance, double laserEnergy, double pulseWidth, double targetDistance, double receiverDistance, double targetReflectivity)
```

计算双程传输后接收到的辐射功率。

- 参数：
  - transmittance: 大气透过率
  - laserEnergy: 激光能量（焦耳）
  - pulseWidth: 脉冲宽度（纳秒）
  - targetDistance: 目标距离（米）
  - receiverDistance: 接收器距离（米）
  - targetReflectivity: 目标反射率
- 返回值：接收到的辐射功率（瓦特）

### WeatherCondition

表示大气环境条件的类。

#### 构造函数

```csharp
public WeatherCondition(
    WeatherType type,
    double temperature,
    double relativeHumidity,
    double visibility,
    double? precipitation = null,
    double co2Concentration = 415)
```

- 参数：
  - type: 天气类型
  - temperature: 温度（摄氏度）
  - relativeHumidity: 相对湿度（0-1）
  - visibility: 能见度（米）
  - precipitation: 降水量（mm/h），可选
  - co2Concentration: 二氧化碳浓度（ppm），默认415

#### 属性

- `Type`: 天气类型（WeatherType 枚举）
- `Temperature`: 温度（摄氏度）
- `RelativeHumidity`: 相对湿度（0-1）
- `Visibility`: 能见度（米）
- `Precipitation`: 降水量（mm/h）
- `CO2Concentration`: 二氧化碳浓度（ppm）

### WeatherType

天气类型枚举。

```csharp
public enum WeatherType
{
    晴天, // 晴朗天气
    雨天, // 雨天
    雪天, // 雪天
    雾天, // 雾天
    沙尘 // 沙尘天气
}
```

## 使用示例

### 基本使用

```csharp
// 创建天气条件
var weather = new WeatherCondition(
    type: WeatherType.晴天,
    temperature: 20, // 20℃
    relativeHumidity: 0.45, // 45%
    visibility: 23000 // 23km
);
// 计算1000米距离的激光透过率
double transmittance = AtmosphericTransmittanceCalculator.CalcLaser(weather, 1000);
```

### 计算有烟雾条件下的透过率

```csharp
// 计算有烟雾条件下的透过率
double smokeTransmittance = AtmosphericTransmittanceCalculator.CalcLaserWithSmoke(
    weather,
    distance: 1000, // 1000米
    smokeConcentration: 0.1, // 0.1 g/m³
    smokeThickness: 100 // 100米
);
```

## 注意事项

1. 所有透过率计算方法返回值范围都在0到1之间
2. 距离单位统一使用米（m）
3. 温度使用摄氏度（℃）
4. 相对湿度使用0-1的小数表示
5. 能见度使用米（m）表示
6. 降水量使用毫米/小时（mm/h）表示

## 性能考虑

- 所有计算方法都是线程安全的
- 计算过程中会考虑多种大气效应，包括分子散射、气溶胶散射、大气湍流等
- 对于大量计算，建议复用 WeatherCondition 对象以提高性能
