using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;

namespace RecipeBook.Common;

public static class AppExtensions
{
    public static Dispatcher Dispatcher { get; } = Application.Current.Dispatcher;

    /// <summary>
    /// Gets the dispatcher synchronization context, which we can use to observe on.
    /// </summary>
    /// <remarks>
    /// If <see cref="Dispatcher"/> is null (which is the case for unit tests),
    /// it uses <see cref="Dispatcher.CurrentDispatcher"/> instead.
    /// </remarks>
    public static DispatcherSynchronizationContext DispatcherSynchronizationContext { get; } = new(Dispatcher);

    /// <summary>
    /// Marshals observable notifications to the UI dispatcher thread by observing on the
    /// current synchronization context. Must be called from the UI thread.
    /// </summary>
    public static IObservable<T> ObserveOnDispatcher<T>(this IObservable<T> source)
        => source.ObserveOn(DispatcherSynchronizationContext);

    public static bool ShouldDispatch()
        => Dispatcher.CheckAccess() is false;

    /// <summary>
    /// Invokes an action on the UI thread.
    /// If already on the UI thread, it just invokes the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="priority">The priority.</param>
    public static void InvokeUI(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        if (ShouldDispatch())
        {
            // Invoke on dispatcher
            Dispatcher.Invoke(action, priority);
        }
        else
        {
            // Already on UI thread
            action();
        }
    }

    /// <summary>
    /// Adds the disposable to a CompositeDisposable for managed subscription lifetime.
    /// </summary>
    public static T DisposeWith<T>(this T disposable, CompositeDisposable composite)
        where T : IDisposable
    {
        composite.Add(disposable);
        return disposable;
    }
}
