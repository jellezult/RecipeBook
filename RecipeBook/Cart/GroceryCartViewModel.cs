using DynamicData;
using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.Cart;

public class GroceryCartViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly GroceryCart cart;

    private readonly ReadOnlyObservableCollection<CartEntryViewModel> cartEntries;
    private IngredientSummaryViewModel[] ingredientsSummary = [];

    public GroceryCartViewModel(GroceryCart cart)
    {
        this.cart = cart;

        // Transform SourceCache<CartEntry,Guid> -> ReadOnlyObservableCollection<CartEntryViewModel>
        cart.ObserveEntries()
            .Transform(entry => new CartEntryViewModel(entry, cart))
            .ObserveOnDispatcher()
            .Bind(out cartEntries)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposables);

        // Recompute when cart entries are added or removed
        cart.ObserveEntries()
            .Subscribe(_ => RecomputeIngredientsSummary())
            .DisposeWith(disposables);

        // Recompute when any cart entry's count changes.
        cart.ObserveEntries()
            .WhenPropertyChanged(entry => entry.Count)
            .Subscribe(_ => RecomputeIngredientsSummary())
            .DisposeWith(disposables);

        // MergeMany: dynamically subscribes/unsubscribes to each recipes's ObserveIngredients
        // as entries enter and leave the SourceCache.
        // Skip(1) skips the initial state replay from Connect() — we only want actual changes.
        cart.ObserveEntries()
            .MergeMany(entry => entry.Recipe.ObserveIngredients().Skip(1).Select(_ => Unit.Default))
            .Subscribe(_ => RecomputeIngredientsSummary())
            .DisposeWith(disposables);
    }

    public ReadOnlyObservableCollection<CartEntryViewModel> CartEntries => cartEntries;

    public IngredientSummaryViewModel[] IngredientsSummary
    {
        get => ingredientsSummary;
        private set => Set(ref ingredientsSummary, value);
    }

    public void Dispose() => disposables.Dispose();

    private void RecomputeIngredientsSummary()
    {
        var summary = cart.CurrentEntries
            .SelectMany(entry => entry.Recipe.IngredientsSnapshot
                .Select(ingredient => (ingredient, entry.Count)))
            .GroupBy(x => x.ingredient)
            .Select(g => new IngredientSummaryViewModel(
                g.Key.ToString(),
                g.Sum(x => x.Count)))
            .OrderBy(s => s.IngredientName)
            .ToArray();

        IngredientsSummary = summary;
    }
}
