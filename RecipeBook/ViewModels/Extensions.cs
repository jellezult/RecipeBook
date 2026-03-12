using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.ViewModels;

public static class Extensions
{
    /// <summary>
    /// Marshals observable notifications to the UI dispatcher thread by observing on the
    /// current synchronization context. Must be called from the UI thread.
    /// </summary>
    public static IObservable<T> ObserveOnDispatcher<T>(this IObservable<T> source)
        => source.ObserveOn(SynchronizationContext.Current!);

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
