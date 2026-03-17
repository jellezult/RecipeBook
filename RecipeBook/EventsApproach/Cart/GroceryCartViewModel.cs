using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RecipeBook.EventsApproach.Cart;

public class GroceryCartViewModel : NotifyObject, IDisposable
{
    private readonly GroceryCart cart;
    private readonly ObservableCollection<CartEntryViewModel> cartEntries = new();
    private IngredientCountViewModel[] ingredientCounts = [];

    public GroceryCartViewModel(GroceryCart cart)
    {
        this.cart = cart;

        // Seed ViewModels for existing entries and subscribe to nested collections
        foreach (var entry in cart.Entries)
        {
            cartEntries.Add(new CartEntryViewModel(entry, cart));
            entry.Recipe.Ingredients.CollectionChanged += OnModelIngredientsChanged;
            entry.PropertyChanged += OnModelEntryCountChanged;
        }

        // Subscribe to top-level cart changes
        cart.Entries.CollectionChanged += OnModelCartEntriesChanged;

        RecomputeIngredientsSummary();
    }

    public ObservableCollection<CartEntryViewModel> CartEntries => cartEntries;

    public IngredientCountViewModel[] IngredientCounts
    {
        get => ingredientCounts;
        private set => Set(ref ingredientCounts, value);
    }

    public void Dispose()
    {
        cart.Entries.CollectionChanged -= OnModelCartEntriesChanged;
        foreach (var entry in cart.Entries)
        {
            entry.Recipe.Ingredients.CollectionChanged -= OnModelIngredientsChanged;
            entry.PropertyChanged -= OnModelEntryCountChanged;
        }
        foreach (var vm in cartEntries)
            vm.Dispose();
    }

    private void OnModelCartEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AppExtensions.InvokeUI(() =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
                foreach (CartEntry entry in e.NewItems)
                {
                    cartEntries.Add(new CartEntryViewModel(entry, cart));
                    // Subscribe to nested collections
                    entry.Recipe.Ingredients.CollectionChanged += OnModelIngredientsChanged;
                    entry.PropertyChanged += OnModelEntryCountChanged;
                }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
                foreach (CartEntry entry in e.OldItems)
                {
                    var vm = cartEntries.FirstOrDefault(v => v.Entry == entry);
                    if (vm is not null)
                    {
                        cartEntries.Remove(vm);
                        vm.Dispose();
                    }
                    entry.Recipe.Ingredients.CollectionChanged -= OnModelIngredientsChanged;
                    entry.PropertyChanged -= OnModelEntryCountChanged;
                }

            RecomputeIngredientsSummary();
        });
    }

    private void OnModelIngredientsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => AppExtensions.InvokeUI(RecomputeIngredientsSummary);

    private void OnModelEntryCountChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartEntry.Count))
            AppExtensions.InvokeUI(RecomputeIngredientsSummary);
    }

    private void RecomputeIngredientsSummary()
    {
        IngredientCounts = cart.ComputeIngredientCounts()
            .Select(x => new IngredientCountViewModel(x.Ingredient.ToString(), x.Count))
            .ToArray();
    }
}