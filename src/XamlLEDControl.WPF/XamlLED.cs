// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: XmlnsDefinition("https://github.com/ChrisPulman/XamlLEDControl.WPF", "CP.XamlLEDControl.WPF")]
[assembly: XmlnsPrefix("https://github.com/ChrisPulman/XamlLEDControl.WPF", "led")]

namespace CP.XamlLEDControl.WPF;

/// <summary>
/// XamlLED.
/// </summary>
/// <seealso cref="ContentControl" />
public class XamlLED : ContentControl
{
    /// <summary>
    /// The led orientation property.
    /// </summary>
    public static readonly DependencyProperty LedOrientationProperty =
        DependencyProperty.Register(
            nameof(LedOrientation),
            typeof(Orientation),
            typeof(XamlLED),
            new PropertyMetadata(Orientation.Horizontal));

    /// <summary>
    /// The is true property.
    /// </summary>
    public static readonly DependencyProperty IsTrueProperty =
        DependencyProperty.Register(
            nameof(IsTrue),
            typeof(bool),
            typeof(XamlLED),
            new PropertyMetadata(false, IsTruePropertyChanged));

    /// <summary>
    /// The leds On property.
    /// </summary>
    public static readonly DependencyProperty LedOnColorsProperty =
        DependencyProperty.Register(
            nameof(LedOnColors),
            typeof(List<Color>),
            typeof(XamlLED),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultValue = new List<Color>(),
                PropertyChangedCallback = LedOnColorsChanged,
            });

    /// <summary>
    /// The leds off property.
    /// </summary>
    public static readonly DependencyProperty LedOffColorsProperty =
        DependencyProperty.Register(
            nameof(LedOffColors),
            typeof(List<Color>),
            typeof(XamlLED),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultValue = new List<Color>(),
                PropertyChangedCallback = LedOffColorsChanged,
            });

    /// <summary>
    /// The off opacity property.
    /// </summary>
    public static readonly DependencyProperty OffOpacityProperty =
        DependencyProperty.Register(
            nameof(OffOpacity),
            typeof(double),
            typeof(XamlLED),
            new PropertyMetadata(0.4));

    /// <summary>
    /// The active led property.
    /// </summary>
    public static readonly DependencyProperty ActiveLedProperty =
        DependencyProperty.Register(
            nameof(ActiveLed),
            typeof(int),
            typeof(XamlLED),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultValue = -1,
                PropertyChangedCallback = ActiveLedChanged
            });

    /// <summary>
    /// The text position property.
    /// </summary>
    public static readonly DependencyProperty TextPositionProperty =
        DependencyProperty.Register(
            nameof(TextPosition),
            typeof(TextPosition),
            typeof(XamlLED),
            new PropertyMetadata(TextPosition.Center, TextPositionChanged));

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(XamlLED),
            new PropertyMetadata(string.Empty, TextChange));

    /// <summary>
    /// The led size property.
    /// </summary>
    public static readonly DependencyProperty LEDSizeProperty =
        DependencyProperty.Register(
            nameof(LEDSize),
            typeof(double),
            typeof(XamlLED),
            new PropertyMetadata(40d, LedSizeChange));

    private readonly List<Ellipse> _leds = new();
    private readonly TextBlock _LedText = new();
    private readonly StackPanel _ledStackPanel = new();
    private readonly Grid _layoutRoot = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="XamlLED"/> class.
    /// </summary>
    public XamlLED()
    {
        _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
        _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
        _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
        Content = _layoutRoot;
        Grid.SetRow(_ledStackPanel, 1);
        Grid.SetColumn(_ledStackPanel, 1);
        Grid.SetColumnSpan(_ledStackPanel, 1);
        Grid.SetRowSpan(_ledStackPanel, 1);
        _layoutRoot.Children.Add(_ledStackPanel);

        _LedText.HorizontalAlignment = HorizontalAlignment.Center;
        _LedText.VerticalAlignment = VerticalAlignment.Center;
        _LedText.TextAlignment = TextAlignment.Center;
        _LedText.Text = Text;
        _LedText.Foreground = Foreground;
        _LedText.FontWeight = FontWeight;
        _LedText.FontSize = FontSize;
        _LedText.Margin = new Thickness(5);

        Grid.SetRow(_LedText, 1);
        Grid.SetColumn(_LedText, 1);
        Grid.SetColumnSpan(_LedText, 1);
        Grid.SetRowSpan(_LedText, 1);
        _layoutRoot.Children.Add(_LedText);
        Loaded += LoadLeds;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [Description("Set the Text of the control")]
    [Category("Layout")]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets orientation of the leds.
    /// </summary>
    [Description("Gets or sets orientation of the leds")]
    [Category("Layout")]
    public Orientation LedOrientation
    {
        get => (Orientation)GetValue(LedOrientationProperty);
        set => SetValue(LedOrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the text position.
    /// </summary>
    /// <value>The text position.</value>
    [Description("Set the Text Position of the control")]
    [Category("Layout")]
    public TextPosition TextPosition
    {
        get => (TextPosition)GetValue(TextPositionProperty);
        set => SetValue(TextPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets colors of the leds in on mode. Amount of colors equal the amount of leds displayed.
    /// </summary>
    [Description("Gets or sets colors of the leds in On mode. Amount of colors equal the amount of leds displayed.")]
    [Category("Layout")]
    public List<Color> LedOnColors
    {
        get => (List<Color>)GetValue(LedOnColorsProperty);
        set => SetValue(LedOnColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the leds off.
    /// </summary>
    /// <value>
    /// The leds off.
    /// </value>
    [Description("Gets or sets colors of the leds in Off mode. Amount of On colors is equal the amount of leds displayed.")]
    [Category("Layout")]
    public List<Color> LedOffColors
    {
        get => (List<Color>)GetValue(LedOffColorsProperty);
        set => SetValue(LedOffColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the led.
    /// </summary>
    /// <value>The size of the led.</value>
    [Description("Set the LED size of the control, default is 40")]
    [Category("Layout")]
    public double LEDSize
    {
        get => (double)GetValue(LEDSizeProperty);
        set => SetValue(LEDSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets opacity of led in off mode.
    /// </summary>
    [Description("Gets or sets opacity of leds in off mode, only used in multiple Led mode.")]
    [Category("Appearance")]
    public double OffOpacity
    {
        get => (double)GetValue(OffOpacityProperty);
        set => SetValue(OffOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is true.
    /// </summary>
    /// <value><c>true</c> if this instance is true; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether this instance is true, only valid when a single LED is used")]
    [Category("Control")]
    public bool IsTrue
    {
        get => (bool)GetValue(IsTrueProperty);
        set => SetValue(IsTrueProperty, value);
    }

    /// <summary>
    /// Gets or sets ative index of the leds
    /// Value 0 = nothing active
    /// Value 1 = first led active
    /// Value 2 = second led active etc.
    /// </summary>
    [Description("Gets or sets a value indicating which instance is On, only used in multiple Led mode")]
    [Category("Control")]
    public int ActiveLed
    {
        get => (int)GetValue(ActiveLedProperty);
        set => SetValue(ActiveLedProperty, value);
    }

    private static void LedSizeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            led.LEDSize = (double)e.NewValue;
            led.LoadLeds(d, null!);
        }
    }

    private static void TextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            var text = e.NewValue.ToString();
            led.Text = text!;
            led._LedText!.Text = text;
        }
    }

    private static void TextPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is TextPosition position)
        {
            led.TextPosition = position;
            switch (position)
            {
                case TextPosition.Top:
                    Grid.SetRow(led._LedText, 0);
                    Grid.SetColumn(led._LedText, 1);
                    Grid.SetColumnSpan(led._LedText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._LedText.VerticalAlignment = VerticalAlignment.Top;
                    led._LedText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._LedText.TextAlignment = TextAlignment.Center;
                    break;

                case TextPosition.Bottom:
                    Grid.SetRow(led._LedText, 3);
                    Grid.SetColumn(led._LedText, 1);
                    Grid.SetColumnSpan(led._LedText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._LedText.VerticalAlignment = VerticalAlignment.Bottom;
                    led._LedText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._LedText.TextAlignment = TextAlignment.Center;
                    break;

                case TextPosition.Left:
                    Grid.SetRow(led._LedText, 1);
                    Grid.SetColumn(led._LedText, 0);
                    Grid.SetColumnSpan(led._LedText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._LedText.VerticalAlignment = VerticalAlignment.Center;
                    led._LedText.HorizontalAlignment = HorizontalAlignment.Left;
                    led._LedText.TextAlignment = TextAlignment.Left;
                    break;

                case TextPosition.Right:
                    Grid.SetRow(led._LedText, 1);
                    Grid.SetColumn(led._LedText, 3);
                    Grid.SetColumnSpan(led._LedText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._LedText.VerticalAlignment = VerticalAlignment.Center;
                    led._LedText.HorizontalAlignment = HorizontalAlignment.Right;
                    led._LedText.TextAlignment = TextAlignment.Right;
                    break;

                case TextPosition.Center:
                    Grid.SetRow(led._LedText, 1);
                    Grid.SetColumn(led._LedText, 1);
                    Grid.SetColumnSpan(led._LedText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._LedText.VerticalAlignment = VerticalAlignment.Center;
                    led._LedText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._LedText.TextAlignment = TextAlignment.Center;
                    break;
            }
        }
    }

    private static void IsTruePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is bool isTrue && led.LedOnColors.Count == led.LedOffColors.Count)
        {
            led.ActiveLed = -1;
            led.IsTrue = isTrue;
            if (isTrue)
            {
                led.On();
            }
            else
            {
                led.Off();
            }
        }
    }

    private static void LedOnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is List<Color> colors)
        {
            led.LedOnColors = colors;
            led.LoadLeds(d, null!);
        }
    }

    private static void LedOffColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is List<Color> colors)
        {
            led.LedOffColors = colors;
            led.LoadLeds(d, null!);
        }
    }

    private static void ActiveLedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is int activeLed)
        {
            led.ActiveLed = activeLed;
            led.Off();
            led.On();
        }
    }

    private static RadialGradientBrush GetLedColor(Color color) => new(
            [
                new GradientStop(Color.FromArgb(150, color.R, color.G, color.B), 0.1d),
                new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), 0.4d),
                new GradientStop(Color.FromArgb(255, color.R, color.G, color.B), 1.0d),
            ])
    {
        GradientOrigin = new Point(0.5d, 0.5d),
        Center = new Point(0.5d, 0.5d),
        RadiusX = 0.5d,
        RadiusY = 0.5d
    };

    private void LoadLeds(object sender, RoutedEventArgs e)
    {
        _LedText.Text = Text;
        _LedText.Foreground = Foreground;
        _LedText.FontWeight = FontWeight;
        _LedText.FontSize = FontSize;

        _ledStackPanel.Orientation = LedOrientation;
        _ledStackPanel.Children.Clear();
        _leds.Clear();

        foreach (var color in LedOnColors)
        {
            // Border for led
            var srgb = new RadialGradientBrush(
            [
                new GradientStop(Color.FromArgb(255, 211, 211, 211), 0.8d),
                new GradientStop(Color.FromArgb(255, 169, 169, 169), 0.9d),
                new GradientStop(Color.FromArgb(255, 150, 150, 150), 0.95d),
            ])
            {
                GradientOrigin = new Point(0.5d, 0.5d),
                Center = new Point(0.5d, 0.5d),
                RadiusX = 0.5d,
                RadiusY = 0.5d
            };

            var ellipse = new Ellipse
            {
                Name = $"ellipses{_leds.Count}",
                Height = LEDSize > 4 ? LEDSize - 4 : LEDSize,
                Width = LEDSize > 4 ? LEDSize - 4 : LEDSize,
                Margin = new Thickness(2),
                Style = null,
                StrokeThickness = LEDSize / 20.0,
                Fill = GetLedColor(color),
                Stroke = srgb
            };
            ellipse.Fill.Opacity = OffOpacity;

            _ledStackPanel.Children.Add(ellipse);
            _leds.Add(ellipse);
        }

        if (ActiveLed == -1 && LedOnColors.Count == LedOffColors.Count)
        {
            if (IsTrue)
            {
                On();
            }
            else
            {
                Off();
            }
        }

        if (ActiveLed > -1)
        {
            On();
        }
    }

    private void On()
    {
        if (ActiveLed == -1 && LedOffColors.Count == LedOnColors.Count)
        {
            for (var i = 0; i < _leds.Count; i++)
            {
                _leds[i].Fill = GetLedColor(LedOnColors[i]);
            }
        }
        else
        {
            var animation = new DoubleAnimation
            {
                From = OffOpacity,
                To = 1.0d,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = false
            };

            for (var i = 0; i < _leds.Count; i++)
            {
                if ((ActiveLed - 1 == i) && (_leds[i].Fill.Opacity < 1.0))
                {
                    _leds[i].Fill.BeginAnimation(Brush.OpacityProperty, animation);
                }
            }
        }
    }

    private void Off()
    {
        if (ActiveLed == -1 && LedOffColors.Count == LedOnColors.Count)
        {
            for (var i = 0; i < _leds.Count; i++)
            {
                _leds[i].Fill = GetLedColor(LedOffColors[i]);
            }
        }
        else
        {
            var animation = new DoubleAnimation
            {
                From = 1.0d,
                To = OffOpacity,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = false
            };

            for (var i = 0; i < _leds.Count; i++)
            {
                if ((ActiveLed - 1 != i) && (_leds[i].Fill.Opacity > OffOpacity))
                {
                    _leds[i].Fill.BeginAnimation(Brush.OpacityProperty, animation);
                }
            }
        }
    }
}
