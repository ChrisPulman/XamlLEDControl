// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CP.XamlLEDControl.WPF.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            led.LedOnColors = new List<Color>(new[] { Colors.Green, Colors.Red, Colors.Green, Colors.Red });
            led.LedOffColors = new List<Color>(new[] { Colors.DarkRed, Colors.DarkGreen, Colors.DarkRed, Colors.DarkGreen });
            led1.LedOnColors = new List<Color>(new[] { Colors.Green });
            led1.LedOffColors = new List<Color>(new[] { Colors.DarkRed });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (chkBox.IsChecked == true)
            {
                led.IsTrue = !led.IsTrue;
            }
            else
            {
                led.IsTrue = false;
                led.ActiveLed++;
                if (led.ActiveLed > 4)
                {
                    led.ActiveLed = 0;
                }
            }

            led1.IsTrue = !led1.IsTrue;
        }
    }
}
