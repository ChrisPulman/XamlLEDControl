// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross;
using CrissCross.WPF.UI.Appearance;
using ReactiveUI;

namespace CP.XamlLEDControl.WPF.TestApp;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        SystemThemeWatcher.Watch(this);
        InitializeComponent();
        this.WhenActivated(d => this.NavigateToView<MainViewModel>());
    }
}
