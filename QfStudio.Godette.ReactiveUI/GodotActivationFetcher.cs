using System.Reactive.Disposables;
using System.Reactive.Linq;
using Godot;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

/// <summary>
/// Maps Godot node lifecycle events to ReactiveUI <see cref="IActivatableView"/> activation semantics.
/// </summary>
/// <remarks>
/// <para>
/// Activation (<c>true</c>): node is in the scene tree <b>and</b> ready (all children initialized).
/// </para>
/// <para>
/// In Godot, <c>TreeEntered</c> fires before <c>Ready</c>, so children may not be initialized yet.
/// <c>TreeEntered</c> alone is not sufficient to determine activation.
/// </para>
/// <para>
/// Three paths emit <c>true</c>:
/// <list type="bullet">
///   <item><c>Ready</c> — first entry, all children initialized</item>
///   <item><c>TreeEntered</c> + <c>IsNodeReady()</c> — re-entry (node was previously ready)</item>
///   <item>Initial check <c>IsInsideTree() &amp;&amp; IsNodeReady()</c> — already in tree at subscription time</item>
/// </list>
/// </para>
/// <para>
/// Semantically equivalent to Avalonia's <c>AttachedToVisualTree</c> / <c>DetachedFromVisualTree</c>.
/// </para>
/// </remarks>
public class GodotActivationFetcher : IActivationForViewFetcher
{
    public int GetAffinityForView(Type view)
    {
        return typeof(Node).IsAssignableFrom(view) ? 10 : 0;
    }

    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        var node = (Node)view;
        return Observable.Create<bool>(observer =>
        {
            // Ready: first entry, all children initialized
            void OnReady() => observer.OnNext(true);

            // TreeEntered: on re-entry, only activate if previously ready
            void OnTreeEntered()
            {
                if (node.IsNodeReady())
                    observer.OnNext(true);
            }

            // TreeExited: node left the scene tree
            void OnTreeExited() => observer.OnNext(false);

            node.Ready += OnReady;
            node.TreeEntered += OnTreeEntered;
            node.TreeExited += OnTreeExited;

            // Initial check: already in tree and ready at subscription time
            if (node.IsInsideTree() && node.IsNodeReady())
                observer.OnNext(true);

            return Disposable.Create(() =>
            {
                node.Ready -= OnReady;
                node.TreeEntered -= OnTreeEntered;
                node.TreeExited -= OnTreeExited;
            });
        });
    }
}
