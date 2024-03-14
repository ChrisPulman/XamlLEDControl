// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using CrissCross;
using ReactiveUI;

namespace CP.XamlLEDControl.WPF;

/// <summary>
/// MainViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class MainViewModel : RxObject
{
    private bool _isTrue;
    private bool _clusterIsTrue;
    private bool _isChecked;
    private int _activeLed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() =>
        this.BuildComplete(() => ToggleCommand = ReactiveCommand.Create(() =>
                {
                    IsTrue = !IsTrue;
                    if (IsChecked)
                    {
                        ClusterIsTrue = !ClusterIsTrue;
                    }
                    else
                    {
                        ClusterIsTrue = false;
                        ActiveLed++;
                        if (ActiveLed > 4)
                        {
                            ActiveLed = 0;
                        }
                    }
                }));

    /// <summary>
    /// Gets or sets a value indicating whether this instance is true.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is true; otherwise, <c>false</c>.
    /// </value>
    public bool IsTrue
    {
        get => _isTrue;
        set => this.RaiseAndSetIfChanged(ref _isTrue, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is true.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is true; otherwise, <c>false</c>.
    /// </value>
    public bool ClusterIsTrue
    {
        get => _clusterIsTrue;
        set => this.RaiseAndSetIfChanged(ref _clusterIsTrue, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }

    /// <summary>
    /// Gets or sets the active led.
    /// </summary>
    /// <value>
    /// The active led.
    /// </value>
    public int ActiveLed
    {
        get => _activeLed;
        set => this.RaiseAndSetIfChanged(ref _activeLed, value);
    }

    /// <summary>
    /// Gets the toggle command.
    /// </summary>
    /// <value>
    /// The toggle command.
    /// </value>
    public ReactiveCommand<Unit, Unit>? ToggleCommand { get; private set; }
}
