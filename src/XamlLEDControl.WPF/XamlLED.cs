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
        DependencyProperty.Register(nameof(LedOrientation), typeof(Orientation), typeof(XamlLED), new PropertyMetadata(Orientation.Horizontal));

    /// <summary>
    /// The is true property.
    /// </summary>
    public static readonly DependencyProperty IsTrueProperty =
        DependencyProperty.Register(nameof(IsTrue), typeof(bool), typeof(XamlLED), new PropertyMetadata(false, IsTruePropertyChanged));

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
        DependencyProperty.Register(nameof(OffOpacity), typeof(double), typeof(XamlLED), new PropertyMetadata(0.4));

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
                DefaultValue = 0,
                PropertyChangedCallback = ActiveLedChanged
            });

    private readonly List<Ellipse> _leds = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="XamlLED"/> class.
    /// </summary>
    public XamlLED() => Loaded += LoadLeds;

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
    /// Gets or sets opacity of led in off mode.
    /// </summary>
    [Description("Gets or sets opacity of leds in off mode.")]
    [Category("Appearance")]
    public double OffOpacity
    {
        get => (double)GetValue(OffOpacityProperty);
        set => SetValue(OffOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets ative index of the leds
    /// Value 0 = nothing active
    /// Value 1 = first led active
    /// Value 2 = second led active etc.
    /// </summary>
    [Description("Gets or sets a value indicating which instance is On, only valid when multiple On LED's are used")]
    [Category("Control")]
    public int ActiveLed
    {
        get => (int)GetValue(ActiveLedProperty);
        set => SetValue(ActiveLedProperty, value);
    }

    private static void IsTruePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED c && e.NewValue is bool isTrue)
        {
            if (c.LedOnColors.Count == 1 && c.LedOffColors.Count == 1)
            {
                c.IsTrue = isTrue;
                if (isTrue)
                {
                    c.On();
                }
                else
                {
                    c.Off();
                }
            }
        }
    }

    private static void LedOnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED c && e.NewValue is List<Color> colors)
        {
            c.LedOnColors = colors;
            c.LoadLeds(d, null!);
        }
    }

    private static void LedOffColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED c && e.NewValue is List<Color> colors)
        {
            c.LedOffColors = colors;
            c.LoadLeds(d, null!);
        }
    }

    /// <summary>
    /// Property changed callback for the active LED index.
    /// </summary>
    /// <param name="d">The DependencyObject.</param>
    /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
    private static void ActiveLedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlLED c && e.NewValue is int activeLed)
        {
            c.ActiveLed = activeLed;
            c.Off();
            c.On();
        }
    }

    private static RadialGradientBrush GetLedColor(Color color) => new(new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(150, color.R, color.G, color.B), 0.1d),
                new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), 0.4d),
                new GradientStop(Color.FromArgb(255, color.R, color.G, color.B), 1.0d),
            })
    {
        GradientOrigin = new Point(0.5d, 0.5d),
        Center = new Point(0.5d, 0.5d),
        RadiusX = 0.5d,
        RadiusY = 0.5d
    };

    /// <summary>
    /// Load led into the panel.
    /// </summary>
    /// <param name="sender">Object sender.</param>
    /// <param name="e">Routed event atguments.</param>
    private void LoadLeds(object sender, RoutedEventArgs e)
    {
        var panel = new StackPanel();
        Content = panel;
        panel.Orientation = LedOrientation;
        panel.Children.Clear();
        _leds.Clear();
        double size;

        if (LedOnColors.Count == 0)
        {
            LedOnColors.Add(Colors.Green);
            LedOffColors.Add(Colors.Red);
        }

        if (LedOrientation == Orientation.Horizontal)
        {
            size = Height;
        }
        else
        {
            size = Width;
        }

        // Give it some size if forgotten to define width or height in combination with orientation
        if (size.Equals(double.NaN) && (Parent is FrameworkElement parent) && (LedOnColors.Count != 0))
        {
            if (!double.IsNaN(parent.ActualWidth))
            {
                size = parent.ActualWidth / LedOnColors.Count;
            }
            else if (!double.IsNaN(parent.ActualHeight))
            {
                size = parent.ActualHeight / LedOnColors.Count;
            }
        }

        foreach (var color in LedOnColors)
        {
            var ellipse = new Ellipse
            {
                Name = $"ellipses{_leds.Count}",
                Height = size > 4 ? size - 4 : size,
                Width = size > 4 ? size - 4 : size,
                Margin = new Thickness(2),
                Style = null
            };

            // Border for led
            var srgb = new RadialGradientBrush(new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(255, 211, 211, 211), 0.8d),
                new GradientStop(Color.FromArgb(255, 169, 169, 169), 0.9d),
                new GradientStop(Color.FromArgb(255, 150, 150, 150), 0.95d),
            });

            ellipse.StrokeThickness = size / 20.0;

            srgb.GradientOrigin = new Point(0.5d, 0.5d);
            srgb.Center = new Point(0.5d, 0.5d);
            srgb.RadiusX = 0.5d;
            srgb.RadiusY = 0.5d;
            ellipse.Stroke = srgb;

            // Color of led
            var rgb = GetLedColor(color);
            ellipse.Fill = rgb;
            ellipse.Fill.Opacity = OffOpacity;
            panel.Children.Add(ellipse);
            _leds.Add(ellipse);
        }

        Off();
    }

    /// <summary>
    /// Switch on the active led.
    /// </summary>
    private void On()
    {
        if (LedOffColors.Count == LedOnColors.Count)
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

    /// <summary>
    /// Switch off all but the active led.
    /// </summary>
    private void Off()
    {
        if (LedOffColors.Count == LedOnColors.Count)
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
