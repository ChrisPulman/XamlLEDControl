// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ReactiveUI;
using Splat;

namespace CP.XamlLEDControl.WPF.TestApp.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    public partial class MainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            led.LedOnColors = new List<Color>([Colors.Green, Colors.Red, Colors.Green, Colors.Red]);
            led.LedOffColors = new List<Color>([Colors.DarkRed, Colors.DarkGreen, Colors.DarkRed, Colors.DarkGreen]);

            ViewModel ??= Locator.Current.GetService<MainViewModel>();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, x => x.IsTrue, x => x.led1.IsTrue);
                this.Bind(ViewModel, x => x.ClusterIsTrue, x => x.led.IsTrue);
                this.Bind(ViewModel, x => x.ActiveLed, x => x.led.ActiveLed);
                this.Bind(ViewModel, x => x.IsChecked, x => x.chkBox.IsChecked);
                this.BindCommand(ViewModel, x => x.ToggleCommand, x => x.ToggleCommand);
            });
        }
    }
}
