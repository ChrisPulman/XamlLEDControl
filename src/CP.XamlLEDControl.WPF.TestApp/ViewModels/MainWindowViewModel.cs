﻿// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CP.XamlLEDControl.WPF.TestApp.Views;
using CrissCross;
using ReactiveUI;
using Splat;

namespace CP.XamlLEDControl.WPF;

/// <summary>
/// MainWindowViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class MainWindowViewModel : RxObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());
        Locator.CurrentMutable.SetupComplete();
    }
}
