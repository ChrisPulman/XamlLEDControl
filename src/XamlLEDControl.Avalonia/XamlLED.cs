// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using ReactiveUI.Avalonia;

[assembly: XmlnsDefinition("https://github.com/ChrisPulman/XamlLEDControl.Avalonia", "CP.XamlLEDControl.Avalonia")]
[assembly: XmlnsPrefix("https://github.com/ChrisPulman/XamlLEDControl.Avalonia", "led")]

namespace CP.XamlLEDControl.Avalonia;

/// <summary>
/// XamlLED.
/// </summary>
public class XamlLED : ContentControl
{
    /// <summary>
    /// The led orientation property.
    /// </summary>
    public static readonly StyledProperty<Orientation> LedOrientationProperty =
        AvaloniaProperty.Register<XamlLED, Orientation>(
            nameof(LedOrientation),
            defaultValue: Orientation.Horizontal);

    /// <summary>
    /// The is true property.
    /// </summary>
    public static readonly StyledProperty<bool> IsTrueProperty =
        AvaloniaProperty.Register<XamlLED, bool>(
            nameof(IsTrue),
            defaultValue: false);

    /// <summary>
    /// The leds On property.
    /// </summary>
    public static readonly StyledProperty<List<Color>> LedOnColorsProperty =
        AvaloniaProperty.Register<XamlLED, List<Color>>(
            nameof(LedOnColors),
            defaultValue: new List<Color>([Colors.Green]),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The leds off property.
    /// </summary>
    public static readonly StyledProperty<List<Color>> LedOffColorsProperty =
        AvaloniaProperty.Register<XamlLED, List<Color>>(
            nameof(LedOffColors),
            defaultValue: new List<Color>([Colors.Red]),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The default on color property.
    /// </summary>
    public static readonly StyledProperty<Color> DefaultOnColorProperty =
        AvaloniaProperty.Register<XamlLED, Color>(
            nameof(DefaultOnColor),
            defaultValue: Colors.Green,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The default off color property.
    /// </summary>
    public static readonly StyledProperty<Color> DefaultOffColorProperty =
        AvaloniaProperty.Register<XamlLED, Color>(
            nameof(DefaultOffColor),
            defaultValue: Colors.Red,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The off opacity property.
    /// </summary>
    public static readonly StyledProperty<double> OffOpacityProperty =
        AvaloniaProperty.Register<XamlLED, double>(
            nameof(OffOpacity),
            defaultValue: 0.4);

    /// <summary>
    /// The active led property.
    /// </summary>
    public static readonly StyledProperty<int> ActiveLedProperty =
        AvaloniaProperty.Register<XamlLED, int>(
            nameof(ActiveLed),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// The text position property.
    /// </summary>
    public static readonly StyledProperty<TextPosition> TextPositionProperty =
        AvaloniaProperty.Register<XamlLED, TextPosition>(
            nameof(TextPosition),
            defaultValue: TextPosition.Center);

    /// <summary>
    /// The text property.
    /// </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<XamlLED, string>(
            nameof(Text),
            defaultValue: string.Empty);

    /// <summary>
    /// The led size property.
    /// </summary>
    public static readonly StyledProperty<double> LEDSizeProperty =
        AvaloniaProperty.Register<XamlLED, double>(
            nameof(LEDSize),
            defaultValue: 40D);

    /// <summary>
    /// The is true property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSquareProperty =
        AvaloniaProperty.Register<XamlLED, bool>(
            nameof(IsSquare),
            defaultValue: false);

    /// <summary>
    /// Gets or sets animation duration in seconds for fade on/off of cluster LEDs.
    /// </summary>
    public static readonly StyledProperty<double> AnimationDurationSecondsProperty =
        AvaloniaProperty.Register<XamlLED, double>(
            nameof(AnimationDurationSeconds),
            defaultValue: 0.1d);

    /// <summary>
    /// Gets or sets spacing (margin) around each LED element.
    /// </summary>
    public static readonly StyledProperty<double> LedSpacingProperty =
        AvaloniaProperty.Register<XamlLED, double>(
            nameof(LedSpacing),
            defaultValue: 2d);

    private readonly List<Shape> _leds = [];
    private readonly TextBlock _ledText = new();
    private readonly StackPanel _ledStackPanel = new();
    private readonly Grid _layoutRoot = new();

    static XamlLED()
    {
        LEDSizeProperty.Changed.Subscribe(OnLedSizeChanged);
        TextProperty.Changed.Subscribe(OnTextChanged);
        IsTrueProperty.Changed.Subscribe(OnIsTrueChanged);
        LedOnColorsProperty.Changed.Subscribe(OnLedOnColorsChanged);
        DefaultOnColorProperty.Changed.Subscribe(OnDefaultLedOnColorChanged);
        LedOffColorsProperty.Changed.Subscribe(OnLedOffColorsChanged);
        DefaultOffColorProperty.Changed.Subscribe(OnDefaultLedOffColorChanged);
        ActiveLedProperty.Changed.Subscribe(OnActiveLedChanged);
        TextPositionProperty.Changed.Subscribe(OnTextPositionChanged);
        LedOrientationProperty.Changed.Subscribe(OnOrientationChanged);
        IsSquareProperty.Changed.Subscribe(OnIsSquareChanged);
        OffOpacityProperty.Changed.Subscribe(OnOffOpacityChanged);
        LedSpacingProperty.Changed.Subscribe(OnLedSpacingChanged);
        ForegroundProperty.Changed.Subscribe(OnForegroundChanged);
        FontSizeProperty.Changed.Subscribe(OnFontSizeChanged);
        FontFamilyProperty.Changed.Subscribe(OnFontFamilyChanged);
        FontWeightProperty.Changed.Subscribe(OnFontWeightChanged);
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [Description("Set the Text of the control")]
    [Category("Layout")]
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets orientation of the leds.
    /// </summary>
    [Description("Gets or sets orientation of the leds")]
    [Category("Layout")]
    public Orientation LedOrientation
    {
        get => GetValue(LedOrientationProperty);
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
        get => GetValue(TextPositionProperty);
        set => SetValue(TextPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets colors of the leds in on mode. Amount of colors equal the amount of leds displayed.
    /// </summary>
    [Description("Gets or sets colors of the leds in On mode. Amount of colors equal the amount of leds displayed.")]
    [Category("Layout")]
    public List<Color> LedOnColors
    {
        get => GetValue(LedOnColorsProperty);
        set => SetValue(LedOnColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets colors of the leds in on mode. Amount of colors equal the amount of leds displayed.
    /// </summary>
    [Description("Gets or sets colors of the led in On mode.")]
    [Category("Layout")]
    public Color DefaultOnColor
    {
        get => GetValue(DefaultOnColorProperty);
        set => SetValue(DefaultOnColorProperty, value);
    }

    /// <summary>
    /// Gets or sets colors of the leds in off mode. Amount of colors equal the amount of leds displayed.
    /// </summary>
    [Description("Gets or sets colors of the led in On mode.")]
    [Category("Layout")]
    public Color DefaultOffColor
    {
        get => GetValue(DefaultOffColorProperty);
        set => SetValue(DefaultOffColorProperty, value);
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
        get => GetValue(LedOffColorsProperty);
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
        get => GetValue(LEDSizeProperty);
        set => SetValue(LEDSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing (margin) around each LED.
    /// </summary>
    [Description("Gets or sets the spacing (margin) around each LED.")]
    [Category("Layout")]
    public double LedSpacing
    {
        get => GetValue(LedSpacingProperty);
        set => SetValue(LedSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets animation duration for cluster LED fade animations in seconds.
    /// </summary>
    [Description("Gets or sets animation duration for cluster LED fade animations in seconds.")]
    [Category("Appearance")]
    public double AnimationDurationSeconds
    {
        get => GetValue(AnimationDurationSecondsProperty);
        set => SetValue(AnimationDurationSecondsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is true.
    /// </summary>
    /// <value><c>true</c> if this instance is true; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether this instance is true, only valid when a single LED is used")]
    [Category("Control")]
    public bool IsTrue
    {
        get => GetValue(IsTrueProperty);
        set => SetValue(IsTrueProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is true.
    /// </summary>
    /// <value><c>true</c> if this instance is true; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether the LED is square or round.Default is false.")]
    [Category("Appearance")]
    public bool IsSquare
    {
        get => GetValue(IsSquareProperty);
        set => SetValue(IsSquareProperty, value);
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
        get => GetValue(ActiveLedProperty);
        set => SetValue(ActiveLedProperty, value);
    }

    /// <summary>
    /// Gets or sets opacity of LED in off mode for cluster mode.
    /// </summary>
    [Description("Gets or sets opacity of LEDs in off mode, only used in multiple LED mode.")]
    [Category("Appearance")]
    public double OffOpacity
    {
        get => GetValue(OffOpacityProperty);
        set => SetValue(OffOpacityProperty, value);
    }

    /// <summary>
    /// Called when the LED control is loaded and lays out its internal content.
    /// </summary>
    /// <param name="e">The loaded event args.</param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        LoadLeds(this, null!);
    }

    /// <summary>
    /// Called when an <see cref="T:Avalonia.Controls.Presenters.ContentPresenter" /> is registered with the control.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>A bool.</returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter == null)
        {
            return false;
        }

        if (presenter?.Name == "PART_ContentPresenter" && presenter.Content == null)
        {
            presenter.Content = _layoutRoot;
            _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            _layoutRoot.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
            _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
            _layoutRoot.RowDefinitions.Add(new() { Height = GridLength.Auto });
            Grid.SetRow(_ledStackPanel, 1);
            Grid.SetColumn(_ledStackPanel, 1);
            Grid.SetColumnSpan(_ledStackPanel, 1);
            Grid.SetRowSpan(_ledStackPanel, 1);
            _layoutRoot.Children.Add(_ledStackPanel);

            _ledText.HorizontalAlignment = HorizontalAlignment.Center;
            _ledText.VerticalAlignment = VerticalAlignment.Center;
            _ledText.TextAlignment = TextAlignment.Center;
            _ledText.Text = Text;
            _ledText.Foreground = Foreground;
            _ledText.FontWeight = FontWeight;
            _ledText.FontSize = FontSize;
            _ledText.FontFamily = FontFamily;
            _ledText.Margin = new Thickness(5);

            Grid.SetRow(_ledText, 1);
            Grid.SetColumn(_ledText, 1);
            Grid.SetColumnSpan(_ledText, 1);
            Grid.SetRowSpan(_ledText, 1);
            _layoutRoot.Children.Add(_ledText);
        }

        return base.RegisterContentPresenter(presenter!);
    }

    private static void OnLedSizeChanged(AvaloniaPropertyChangedEventArgs<double> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LoadLeds(led, null!);
        }
    }

    private static void OnTextChanged(AvaloniaPropertyChangedEventArgs<string> args)
    {
        if (args.Sender is XamlLED led)
        {
            led._ledText!.Text = args.NewValue.Value;
        }
    }

    private static void OnForegroundChanged(AvaloniaPropertyChangedEventArgs<IBrush?> args)
    {
        if (args.Sender is XamlLED led && args.NewValue.HasValue)
        {
            led._ledText.Foreground = args.NewValue.Value;
        }
    }

    private static void OnFontSizeChanged(AvaloniaPropertyChangedEventArgs<double> args)
    {
        if (args.Sender is XamlLED led)
        {
            led._ledText.FontSize = args.NewValue.Value;
        }
    }

    private static void OnFontFamilyChanged(AvaloniaPropertyChangedEventArgs<FontFamily> args)
    {
        if (args.Sender is XamlLED led && args.NewValue.HasValue)
        {
            led._ledText.FontFamily = args.NewValue.Value;
        }
    }

    private static void OnFontWeightChanged(AvaloniaPropertyChangedEventArgs<FontWeight> args)
    {
        if (args.Sender is XamlLED led)
        {
            led._ledText.FontWeight = args.NewValue.Value;
        }
    }

    private static void OnOrientationChanged(AvaloniaPropertyChangedEventArgs<Orientation> args)
    {
        if (args.Sender is XamlLED led)
        {
            led._ledStackPanel.Orientation = args.NewValue.Value;
        }
    }

    private static void OnTextPositionChanged(AvaloniaPropertyChangedEventArgs<TextPosition> args)
    {
        if (args.Sender is XamlLED led)
        {
            switch (args.NewValue.Value)
            {
                case TextPosition.Top:
                    Grid.SetRow(led._ledText, 0);
                    Grid.SetColumn(led._ledText, 1);
                    Grid.SetColumnSpan(led._ledText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._ledText.VerticalAlignment = VerticalAlignment.Top;
                    led._ledText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._ledText.TextAlignment = TextAlignment.Center;
                    break;

                case TextPosition.Bottom:
                    Grid.SetRow(led._ledText, 2);
                    Grid.SetColumn(led._ledText, 1);
                    Grid.SetColumnSpan(led._ledText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._ledText.VerticalAlignment = VerticalAlignment.Bottom;
                    led._ledText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._ledText.TextAlignment = TextAlignment.Center;
                    break;

                case TextPosition.Left:
                    Grid.SetRow(led._ledText, 1);
                    Grid.SetColumn(led._ledText, 0);
                    Grid.SetColumnSpan(led._ledText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._ledText.VerticalAlignment = VerticalAlignment.Center;
                    led._ledText.HorizontalAlignment = HorizontalAlignment.Left;
                    led._ledText.TextAlignment = TextAlignment.Left;
                    break;

                case TextPosition.Right:
                    Grid.SetRow(led._ledText, 1);
                    Grid.SetColumn(led._ledText, 2);
                    Grid.SetColumnSpan(led._ledText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._ledText.VerticalAlignment = VerticalAlignment.Center;
                    led._ledText.HorizontalAlignment = HorizontalAlignment.Right;
                    led._ledText.TextAlignment = TextAlignment.Right;
                    break;

                case TextPosition.Center:
                    Grid.SetRow(led._ledText, 1);
                    Grid.SetColumn(led._ledText, 1);
                    Grid.SetColumnSpan(led._ledText, 1);
                    Grid.SetRow(led._ledStackPanel, 1);
                    Grid.SetColumn(led._ledStackPanel, 1);
                    led._ledText.VerticalAlignment = VerticalAlignment.Center;
                    led._ledText.HorizontalAlignment = HorizontalAlignment.Center;
                    led._ledText.TextAlignment = TextAlignment.Center;
                    break;
            }
        }
    }

    private static void OnIsTrueChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.Sender is XamlLED led && led.LedOnColors.Count == led.LedOffColors.Count)
        {
            led.ActiveLed = -1;
            if (args.NewValue.Value)
            {
                led.On();
            }
            else
            {
                led.Off();
            }
        }
    }

    private static void OnLedOnColorsChanged(AvaloniaPropertyChangedEventArgs<List<Color>> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LoadLeds(led, null!);
        }
    }

    private static void OnLedOffColorsChanged(AvaloniaPropertyChangedEventArgs<List<Color>> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LoadLeds(led, null!);
        }
    }

    private static void OnDefaultLedOnColorChanged(AvaloniaPropertyChangedEventArgs<Color> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LedOnColors = [args.NewValue.Value];
            led.LoadLeds(led, null!);
        }
    }

    private static void OnDefaultLedOffColorChanged(AvaloniaPropertyChangedEventArgs<Color> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LedOffColors = [args.NewValue.Value];
            led.LoadLeds(led, null!);
        }
    }

    private static void OnActiveLedChanged(AvaloniaPropertyChangedEventArgs<int> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.Off();
            led.On();
        }
    }

    private static void OnIsSquareChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.Sender is XamlLED led)
        {
            led.LoadLeds(led, null!);
        }
    }

    private static void OnOffOpacityChanged(AvaloniaPropertyChangedEventArgs<double> args)
    {
        if (args.Sender is XamlLED led)
        {
            if (led.ActiveLed > -1)
            {
                led.Off();
                led.On();
            }
            else if (led.LedOnColors.Count == led.LedOffColors.Count)
            {
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

    private static void OnLedSpacingChanged(AvaloniaPropertyChangedEventArgs<double> args)
    {
        if (args.Sender is XamlLED led)
        {
            foreach (var sh in led._leds)
            {
                sh.Margin = new Thickness(args.NewValue.Value);
            }
        }
    }

    private static RadialGradientBrush GetLedColor(in Color color) => new()
    {
        GradientOrigin = new RelativePoint(0.5d, 0.5d, RelativeUnit.Absolute),
        Center = new RelativePoint(0.5d, 0.5d, RelativeUnit.Absolute),
        RadiusX = new RelativeScalar(0.5d, RelativeUnit.Relative),
        RadiusY = new RelativeScalar(0.5d, RelativeUnit.Relative),
        GradientStops =
        [
            new GradientStop(Color.FromArgb(150, color.R, color.G, color.B), 0.1d),
            new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), 0.4d),
            new GradientStop(Color.FromArgb(255, color.R, color.G, color.B), 1.0d),
        ],
    };

#if NETSTANDARD2_0
    private static void OpacityAnimation(Visual visual, double from, double to, TimeSpan timeSpan)
#else
    private static void OpacityAnimation(Visual visual, double from, double to, in TimeSpan timeSpan)
#endif
    {
        var tickTime = timeSpan.TotalMilliseconds / 100;
        IDisposable? subscription = default;
        if (to > from)
        {
            var increment = (to - from) / 100;
            subscription = Observable.Interval(TimeSpan.FromMilliseconds(tickTime))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ =>
                {
                    if (visual.Opacity >= to)
                    {
                        visual.Opacity = to;
                        subscription?.Dispose();
                        return;
                    }

                    visual.Opacity += increment;
                });
        }
        else
        {
            var decrement = (from - to) / 100;
            subscription = Observable.Interval(TimeSpan.FromMilliseconds(tickTime))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ =>
                {
                    if (visual.Opacity <= to)
                    {
                        visual.Opacity = to;
                        subscription?.Dispose();
                        return;
                    }

                    visual.Opacity -= decrement;
                });
        }
    }

    private void LoadLeds(object? sender, EventArgs e)
    {
        _ledText.Text = Text;
        _ledText.Foreground = Foreground;
        _ledText.FontWeight = FontWeight;
        _ledText.FontSize = FontSize;
        _ledText.FontFamily = FontFamily;

        _ledStackPanel.Orientation = LedOrientation;
        _ledStackPanel.Children.Clear();
        _leds.Clear();

        foreach (var color in LedOnColors)
        {
            // Border for led
            var srgb = new RadialGradientBrush()
            {
                GradientOrigin = new RelativePoint(0.5d, 0.5d, RelativeUnit.Absolute),
                Center = new RelativePoint(0.5d, 0.5d, RelativeUnit.Absolute),
                RadiusX = new RelativeScalar(0.5d, RelativeUnit.Relative),
                RadiusY = new RelativeScalar(0.5d, RelativeUnit.Relative),
                GradientStops =
                [
                    new GradientStop(Color.FromArgb(255, 211, 211, 211), 0.8d),
                    new GradientStop(Color.FromArgb(255, 169, 169, 169), 0.9d),
                    new GradientStop(Color.FromArgb(255, 150, 150, 150), 0.95d),
                ]
            };

            if (IsSquare)
            {
                var rectangle = new Rectangle
                {
                    Name = $"leds{_leds.Count}",
                    Height = LEDSize,
                    Width = LEDSize,
                    Margin = new Thickness(LedSpacing),
                    StrokeThickness = LEDSize / 20.0,
                    Fill = GetLedColor(color),
                    Stroke = srgb,
                    Opacity = OffOpacity,
                    RadiusX = LEDSize / 10.0,
                    RadiusY = LEDSize / 10.0
                };

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
                    StrokeThickness = LEDSize / 20.0,
                    Fill = GetLedColor(color),
                    Stroke = srgb,
                    Opacity = OffOpacity
                };

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
                _leds[i].Opacity = 1d;
            }
        }
        else
        {
            for (var i = 0; i < _leds.Count; i++)
            {
                if ((ActiveLed - 1 == i) && (_leds[i].Opacity < 1.0))
                {
                    OpacityAnimation(_leds[i], OffOpacity, 1d, TimeSpan.FromSeconds(AnimationDurationSeconds));
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
                _leds[i].Opacity = 1d;
            }
        }
        else
        {
            for (var i = 0; i < _leds.Count; i++)
            {
                if ((ActiveLed - 1 != i) && (_leds[i].Opacity > OffOpacity))
                {
                    OpacityAnimation(_leds[i], 1d, OffOpacity, TimeSpan.FromSeconds(AnimationDurationSeconds));
                }
            }
        }
    }
}
