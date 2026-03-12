using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using RecipeBook.Models;

namespace RecipeBook.ViewModels;

public class GroceryCartViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    private ReadOnlyObservableCollection<CartEntryViewModel> cartEntries = null!;
    private List<IngredientSummaryViewModel> aggregatedIngredients = new();

    public ReadOnlyObservableCollection<CartEntryViewModel> CartEntries => this.cartEntries;

    public List<IngredientSummaryViewModel> AggregatedIngredients
    {
        get => this.aggregatedIngredients;
        private set => Set(ref this.aggregatedIngredients, value);
    }

    public GroceryCartViewModel(GroceryCart cart)
    {
        // Transform SourceCache<CartEntry,Guid> -> ReadOnlyObservableCollection<CartEntryViewModel>
        cart.ObserveEntries()
            .Transform(entry => new CartEntryViewModel(entry, cart))
            .ObserveOnDispatcher()
            .Bind(out this.cartEntries)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(this.disposables);

        // Recompute when cart entries are added or removed
        cart.ObserveEntries()
            .Subscribe(_ => RecomputeAggregation())
            .DisposeWith(this.disposables);

        // MergeMany: dynamically subscribes/unsubscribes to each entry's ObserveCount()
        // as entries enter and leave the SourceCache.
        // Triggers reaggregation whenever any cart entry's count changes.
        cart.ObserveEntries()
            .MergeMany(entry => entry.ObserveCount().Select(_ => Unit.Default))
            .Subscribe(_ => RecomputeAggregation())
            .DisposeWith(this.disposables);

        // Bug fix: also recompute when any cart recipe's ingredients change.
        // Skip(1) skips the initial state replay from Connect() — we only want actual changes.
        cart.ObserveEntries()
            .MergeMany(entry => entry.Recipe.ObserveIngredients().Skip(1).Select(_ => Unit.Default))
            .Subscribe(_ => RecomputeAggregation())
            .DisposeWith(this.disposables);
    }

    private void RecomputeAggregation()
    {
        // For each cart entry, expand its recipe's ingredients weighted by count
        var summary = this.cartEntries
            .SelectMany(vm => vm.GetModel().Recipe.IngredientsSnapshot
                .Select(ingredient => (ingredient, vm.GetModel().Count)))
            .GroupBy(x => x.ingredient)
            .Select(g => new IngredientSummaryViewModel(
                g.Key.ToString(),
                g.Sum(x => x.Count)))
            .OrderBy(s => s.IngredientName)
            .ToList();

        AggregatedIngredients = summary;
    }

    public void Dispose() => this.disposables.Dispose();
}
