# Developer Docs

## Limitations for Godot

- In the current version of Godot, any C# class that inherits from `Godot.Node` is treated as a script resource. As a result, all such code files must be placed within the Godot project's source directory. This means that creating a third‑party library containing classes that derive from `Godot.Node` will not work.
- From the engine's perspective, every script resource must have a unique path within the source directory. Consequently, generic types that inherit from `Godot.Node` are not supported.

## ReactiveUI Integration

### Activation Semantics

Activation (`true`) means the node is in the scene tree **and** ready (all children initialized). This is semantically equivalent to Avalonia's `AttachedToVisualTree` / `DetachedFromVisualTree`.

Three paths emit `true`:
- `Ready` — first entry, all children initialized
- `TreeEntered` + `IsNodeReady()` — re-entry (node was previously ready)
- Initial check `IsInsideTree() && IsNodeReady()` — already in tree at subscription time
