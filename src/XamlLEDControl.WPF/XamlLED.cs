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
            new PropertyMetadata(Orientation.Horizontal, LedOrientationChanged));

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
                DefaultValue = new List<Color>([Colors.Green]),
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
                DefaultValue = new List<Color>([Colors.Red]),
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
            new PropertyMetadata(0.4, OffOpacityChanged));

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

    /// <summary>
    /// The default on color property.
    /// </summary>
    public static readonly DependencyProperty DefaultOnColorProperty =
        DependencyProperty.Register(
            nameof(DefaultOnColor),
            typeof(Color),
            typeof(XamlLED),
            new FrameworkPropertyMetadata
            {
                DefaultValue = Colors.Green,
                PropertyChangedCallback = LedOnColorsChanged,
            });

    /// <summary>
    /// The default off color property.
    /// </summary>
    public static readonly DependencyProperty DefaultOffColorProperty =
        DependencyProperty.Register(
            nameof(DefaultOffColor),
            typeof(Color),
            typeof(XamlLED),
            new FrameworkPropertyMetadata
            {
                DefaultValue = Colors.Red,
                PropertyChangedCallback = LedOffColorsChanged,
            });

    /// <summary>
    /// The is square property.
    /// </summary>
    public static readonly DependencyProperty IsSquareProperty =
        DependencyProperty.Register(
            nameof(IsSquare),
            typeof(bool),
            typeof(XamlLED),
            new PropertyMetadata(false, IsSquareChanged));

    /// <summary>
    /// Animation duration in seconds for fade on/off of cluster LEDs. Default is 0.1 seconds.
    /// </summary>
    public static readonly DependencyProperty AnimationDurationSecondsProperty =
        DependencyProperty.Register(
            nameof(AnimationDurationSeconds),
            typeof(double),
            typeof(XamlLED),
            new PropertyMetadata(0.1));

    /// <summary>
    /// Spacing (margin) around each LED element.
    /// </summary>
    public static readonly DependencyProperty LedSpacingProperty =
        DependencyProperty.Register(
            nameof(LedSpacing),
            typeof(double),
            typeof(XamlLED),
            new PropertyMetadata(2d, LedSpacingChanged));

    private readonly List<Shape> _leds = [];
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
        _LedText.FontFamily = FontFamily;
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
    /// Gets or sets a value indicating whether this instance is square.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is square; otherwise, <c>false</c>.
    /// </value>
    [Description("Gets or sets a value indicating whether the LED is square or round. Default is false.")]
    [Category("Layout")]
    public bool IsSquare
    {
        get => (bool)GetValue(IsSquareProperty);
        set => SetValue(IsSquareProperty, value);
    }

    /// <summary>
    /// Gets or sets the default color of the on.
    /// </summary>
    /// <value>
    /// The default color of the on.
    /// </value>
    public Color DefaultOnColor
    {
        get => (Color)GetValue(DefaultOnColorProperty);
        set
        {
            SetValue(DefaultOnColorProperty, value);
            LedOnColors = new List<Color>([value]);
        }
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
    /// Gets or sets the default color of the off.
    /// </summary>
    /// <value>
    /// The default color of the off.
    /// </value>
    public Color DefaultOffColor
    {
        get => (Color)GetValue(DefaultOffColorProperty);
        set
        {
            SetValue(DefaultOffColorProperty, value);
            LedOffColors = new List<Color>([value]);
        }
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
    /// Gets or sets the spacing (margin) around each LED.
    /// </summary>
    [Description("Gets or sets the spacing (margin) around each LED.")]
    [Category("Layout")]
    public double LedSpacing
    {
        get => (double)GetValue(LedSpacingProperty);
        set => SetValue(LedSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the animation duration for cluster LED fade animations in seconds. Default is 0.1 seconds.
    /// </summary>
    [Description("Gets or sets animation duration for cluster LED fade animations in seconds. Default is 0.1 seconds.")]
    [Category("Appearance")]
    public double AnimationDurationSeconds
    {
        get => (double)GetValue(AnimationDurationSecondsProperty);
        set => SetValue(AnimationDurationSecondsProperty, value);
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

    /// <summary>
    /// Called when a dependency property changes on this control to mirror common properties to the inner TextBlock.
    /// </summary>
    /// <param name="e">The change event args.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ForegroundProperty)
        {
            _LedText.Foreground = (Brush)e.NewValue;
        }
        else if (e.Property == FontSizeProperty)
        {
            _LedText.FontSize = (double)e.NewValue;
        }
        else if (e.Property == FontWeightProperty)
        {
            _LedText.FontWeight = (FontWeight)e.NewValue;
        }
        else if (e.Property == FontFamilyProperty)
        {
            _LedText.FontFamily = (FontFamily)e.NewValue;
        }
        else if (e.Property == FontStyleProperty)
        {
            _LedText.FontStyle = (FontStyle)e.NewValue;
        }
    }

    private static void LedSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && led._leds.Count > 0)
        {
            var margin = new Thickness((double)e.NewValue);
            foreach (var sh in led._leds)
            {
                sh.Margin = margin;
            }
        }
    }

    private static void LedOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            led._ledStackPanel.Orientation = (Orientation)e.NewValue;
        }
    }

    private static void LedSizeChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            led.LoadLeds(d, null!);
        }
    }

    private static void TextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            var text = e.NewValue?.ToString() ?? string.Empty;
            led._LedText!.Text = text;
        }
    }

    private static void TextPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is TextPosition position)
        {
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
                    Grid.SetRow(led._LedText, 2);
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
                    Grid.SetColumn(led._LedText, 2);
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

    private static void OffOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            // Re-apply current state to update opacities
            if (led.ActiveLed > -1)
            {
                led.Off();
                led.On();
            }
            else if (led.LedOnColors.Count == led.LedOffColors.Count)
            {
                // No change for single LED mode; ensure visuals reflect current state
                if (led.IsTrue)
                {
                    led.On();
                }
                else
                {
                    led.Off();
                }
            }
        }
    }

    private static void IsSquareChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            led.LoadLeds(d, null!);
        }
    }

    private static void LedOnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            if (e.NewValue is List<Color>)
            {
                led.LoadLeds(d, null!);
            }
            else if (e.NewValue is Color color)
            {
                led.LedOnColors = [color];
                led.LoadLeds(d, null!);
            }
        }
    }

    private static void LedOffColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led)
        {
            if (e.NewValue is List<Color>)
            {
                led.LoadLeds(d, null!);
            }
            else if (e.NewValue is Color color)
            {
                led.LedOffColors = [color];
                led.LoadLeds(d, null!);
            }
        }
    }

    private static void ActiveLedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED led && e.NewValue is int)
        {
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
        _LedText.FontFamily = FontFamily;

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

            if (IsSquare)
            {
                var rectangle = new Rectangle
                {
                    Name = $"rectangles{_leds.Count}",
                    Height = LEDSize,
                    Width = LEDSize,
                    Margin = new Thickness(LedSpacing),
                    Fill = GetLedColor(color),
                    Stroke = srgb,
                    StrokeThickness = LEDSize / 20.0,
                    RadiusX = LEDSize / 10.0,
                    RadiusY = LEDSize / 10.0
                };
                rectangle.Fill.Opacity = OffOpacity;

                _ledStackPanel.Children.Add(rectangle);
                _leds.Add(rectangle);
            }
            else
            {
                var ellipse = new Ellipse
                {
                    Name = $"ellipses{_leds.Count}",
                    Height = LEDSize > 4 ? LEDSize - 4 : LEDSize,
                    Width = LEDSize > 4 ? LEDSize - 4 : LEDSize,
                    Margin = new Thickness(LedSpacing),
                    Style = null,
                    StrokeThickness = LEDSize / 20.0,
                    Fill = GetLedColor(color),
                    Stroke = srgb
                };
                ellipse.Fill.Opacity = OffOpacity;

                _ledStackPanel.Children.Add(ellipse);
                _leds.Add(ellipse);
            }
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
                Duration = new Duration(TimeSpan.FromSeconds(AnimationDurationSeconds)),
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
                Duration = new Duration(TimeSpan.FromSeconds(AnimationDurationSeconds)),
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
