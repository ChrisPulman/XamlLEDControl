// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using CrissCross;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace CP.XamlLEDControl.WPF;

/// <summary>
/// MainViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public partial class MainViewModel : RxObject
{
    [Reactive]
    private bool _isTrue;
    [Reactive]
    private bool _clusterIsTrue;
    [Reactive]
    private bool _isChecked;
    [Reactive]
    private int _activeLed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() =>
        this.BuildComplete(() => Observable.Interval(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => Toggle())
            .DisposeWith(Disposables));

    /// <summary>
    /// Gets the toggle command.
    /// </summary>
    /// <value>
    /// The toggle command.
    /// </value>
    [ReactiveCommand]
    private void Toggle()
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
    }
}
