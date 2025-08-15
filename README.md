# XamlLEDControl

Beautiful, lightweight LED simulators for WPF and Avalonia. Each control can display a single LED with on/off state and optional text, or a cluster of LEDs with smooth fade animations. All key properties are fully dynamic at runtime: changes to fonts, sizes, colors, layout, orientation, and animation settings update the UI immediately.

- WPF package: XamlLEDControl.WPF
- Avalonia package: XamlLEDControl.Avalonia

NuGet
- WPF: ![NuGet Version](https://img.shields.io/nuget/v/XamlLEDControl.WPF) ![NuGet downloads](https://img.shields.io/nuget/dt/XamlLEDControl.WPF)
- Avalonia: ![NuGet Version](https://img.shields.io/nuget/v/XamlLEDControl.Avalonia) ![NuGet Downloads](https://img.shields.io/nuget/dt/XamlLEDControl.Avalonia)

Install
- dotnet add package XamlLEDControl.WPF
- dotnet add package XamlLEDControl.Avalonia

Target frameworks
- WPF control: .NET Framework 4.6.2, .NET 8, .NET 9
- Avalonia control: .NET Standard 2.0, .NET 8, .NET 9

XAML namespaces
- WPF: ```xmlns:led="https://github.com/ChrisPulman/XamlLEDControl.WPF"```
- Avalonia: ```xmlns:led="https://github.com/ChrisPulman/XamlLEDControl.Avalonia"```

Features
- Single LED or LED cluster
- Round or square LED (with subtle corner rounding)
- Per-LED gradient for a realistic look
- Label text with flexible positioning around LEDs
- Horizontal or vertical layout
- Runtime-dynamic updates to all properties (font, colors, sizes, spacing, animation)
- Smooth fade animations for cluster selection

Properties (common)
- Text: string – label text overlay/near the LEDs
- TextPosition: Top | Bottom | Left | Right | Center – where to place Text relative to LEDs
- LedOrientation: Horizontal | Vertical – direction of the LED stack
- LEDSize: double – LED visual size (default 40)
- LedSpacing: double – margin around each LED (default 2)
- IsSquare: bool – square or round LED (default false)
- DefaultOnColor: Color – used by single-LED mode
- DefaultOffColor: Color – used by single-LED mode
- LedOnColors: List<Color> – cluster on colors (one per LED)
- LedOffColors: List<Color> – cluster off colors (one per LED)
- IsTrue: bool – single-LED on/off state (used when ActiveLed == -1 and On/Off lists have same length)
- ActiveLed: int – 1-based active LED index for cluster mode (0 or -1 means none active)
- OffOpacity: double – opacity of non-active LEDs in cluster mode (default 0.4)
- AnimationDurationSeconds: double – fade animation duration for cluster mode (default 1.0)
- FontFamily, FontSize, FontWeight, Foreground – forwarded to the internal TextBlock

WPF usage
1) Add namespace
- xmlns:led="https://github.com/ChrisPulman/XamlLEDControl.WPF"

2) Single LED (XAML)
```xml
<led:XamlLED
    Text="Power"
    TextPosition="Right"
    LEDSize="36"
    DefaultOnColor="LimeGreen"
    DefaultOffColor="DarkGreen"
    IsTrue="True"
    FontFamily="Segoe UI"
    FontSize="14" />
```

3) Cluster (XAML + code-behind)
- XAML
```xml
<StackPanel>
    <led:XamlLED x:Name="Cluster"
                 Text="Status"
                 TextPosition="Top"
                 LedOrientation="Horizontal"
                 LEDSize="32"
                 LedSpacing="4"
                 OffOpacity="0.35"
                 AnimationDurationSeconds="0.5" />

    <Button Content="Next" Click="Next_Click" />
  </StackPanel>
```

- Code-behind (C#)
```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Cluster.LedOnColors = new List<Color> { Colors.Green, Colors.Red, Colors.Green, Colors.Red };
        Cluster.LedOffColors = new List<Color> { Colors.DarkGreen, Colors.DarkRed, Colors.DarkGreen, Colors.DarkRed };
        Cluster.ActiveLed = 1; // 1-based index
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        // cycle through LEDs 1..4, 0/-1 means none
        var next = Cluster.ActiveLed + 1;
        if (next > Cluster.LedOnColors.Count) next = 0;
        Cluster.ActiveLed = next;
    }
}
```

4) Dynamic updates at runtime
```csharp
Cluster.LEDSize = 48; // resizes LEDs live
Cluster.IsSquare = true; // switches to square LEDs
Cluster.OffOpacity = 0.2; // dims inactive LEDs more
Cluster.Text = "Network"; // updates label text
Cluster.TextPosition = TextPosition.Bottom; // moves label dynamically
```

Avalonia usage
1) Add namespace
- xmlns:led="https://github.com/ChrisPulman/XamlLEDControl.Avalonia"

2) Single LED (XAML)
```xml
<led:XamlLED
    Text="Power"
    TextPosition="Right"
    LEDSize="36"
    DefaultOnColor="LimeGreen"
    DefaultOffColor="DarkGreen"
    IsTrue="True"
    FontFamily="Segoe UI"
    FontSize="14" />
```

3) Cluster (XAML + code-behind)
- XAML
```xml
<StackPanel>
    <led:XamlLED x:Name="Cluster"
                 Text="Status"
                 TextPosition="Top"
                 LedOrientation="Horizontal"
                 LEDSize="32"
                 LedSpacing="4"
                 OffOpacity="0.35"
                 AnimationDurationSeconds="0.5" />

    <Button Content="Next" Click="Next_Click" />
  </StackPanel>
```

- Code-behind (C#)
```csharp
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        Cluster.LedOnColors = new List<Color> { Colors.Green, Colors.Red, Colors.Green, Colors.Red };
        Cluster.LedOffColors = new List<Color> { Colors.DarkGreen, Colors.DarkRed, Colors.DarkGreen, Colors.DarkRed };
        Cluster.ActiveLed = 1;
    }

    private void Next_Click(object? sender, RoutedEventArgs e)
    {
        var next = Cluster.ActiveLed + 1;
        if (next > Cluster.LedOnColors.Count) next = 0;
        Cluster.ActiveLed = next;
    }
}
```

4) Dynamic updates at runtime
```csharp
Cluster.LEDSize = 48;
Cluster.IsSquare = true;
Cluster.OffOpacity = 0.2;
Cluster.Text = "Network";
Cluster.TextPosition = TextPosition.Bottom;
```

MVVM bindings
- Single LED
```xml
<led:XamlLED Text="Connected"
               DefaultOnColor="LimeGreen"
               DefaultOffColor="DarkGreen"
               IsTrue="{Binding IsConnected}" />
```

- Cluster
```xml
<led:XamlLED LedOrientation="Horizontal"
               LedSpacing="6"
               OffOpacity="0.4"
               AnimationDurationSeconds="0.3"
               ActiveLed="{Binding ActiveIndex}" />
```

Notes
- ActiveLed is 1-based for active LED selection; set 0 or -1 for none.
- Single LED mode is used when ActiveLed == -1 and LedOnColors.Count == LedOffColors.Count. In this case IsTrue toggles all LEDs between DefaultOnColor and DefaultOffColor.
- To set per-LED colors from XAML directly, prefer binding to a ViewModel property of type List<Color>. Setting List<Color> literals is simpler in code-behind.

License
- MIT License (see LICENSE)
