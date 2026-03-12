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
    private readonly GroceryCart cart;

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
        this.cart = cart;

        // Transform SourceCache<CartEntry,Guid> -> ReadOnlyObservableCollection<CartEntryViewModel>
        this.disposables.Add(
            cart.EntriesChanges
                .Transform(entry => new CartEntryViewModel(entry, cart))
                .Bind(out this.cartEntries)
                .DisposeMany()
                .Subscribe());

        // Recompute aggregated ingredients when recipes are added or removed from cart
        this.disposables.Add(
            cart.EntriesChanges
                .Subscribe(_ => RecomputeAggregation()));

        // Key RX pattern: MergeMany dynamically subscribes/unsubscribes to each entry's
        // CountChanged observable as entries enter and leave the SourceCache.
        // This triggers reaggregation whenever any cart entry's count changes.
        this.disposables.Add(
            cart.EntriesChanges
                .MergeMany(entry => entry.CountChanged.Select(_ => Unit.Default))
                .Subscribe(_ => RecomputeAggregation()));
    }

    private void RecomputeAggregation()
    {
        // For each cart entry, expand its recipe's ingredients, weighted by count
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
