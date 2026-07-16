# QfStudio.Godette

## QfStudio.Godette.ReactiveUI

ReactiveUI is an open-source, composable MVVM (Model-View-ViewModel) framework for .NET platforms.

Generally, it takes 4 steps to implement `ReactiveUI` for a custom platform.

- Implement `IScheduler` for UI thread.
- Implement `IActivationForViewFetcher` for view activation.
- Implement `ICreatesObservableForProperty` for property changed notification.
- Implement `ICommandBinderImplementation` for command binding.

QfStudio.Godette.ReactiveUI does the above job for Godot Engine.

### Usage

// TODO

### Notes

**Activation semantics**: A view is activated (`true`) when it is in the scene tree **and** ready (all children initialized). This is semantically equivalent to Avalonia's `AttachedToVisualTree` / `DetachedFromVisualTree`.

### Alternatives

- [R3](https://github.com/Cysharp/R3) Zero allocation and high performance Rx .NET alternative. If you prefer building app from ReactiveProperty pieces, or don't want to apply the full MVVM pattern, you can try R3.
