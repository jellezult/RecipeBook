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
            entry.Recipe.Ingredients.CollectionChanged += OnRecipeIngredientsChanged;
            entry.PropertyChanged += OnEntryCountChanged;
        }

        // Subscribe to top-level cart changes
        cart.Entries.CollectionChanged += OnCartEntriesChanged;

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
        cart.Entries.CollectionChanged -= OnCartEntriesChanged;
        foreach (var entry in cart.Entries)
        {
            entry.Recipe.Ingredients.CollectionChanged -= OnRecipeIngredientsChanged;
            entry.PropertyChanged -= OnEntryCountChanged;
        }
        foreach (var vm in cartEntries)
            vm.Dispose();
    }

    private void OnCartEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            foreach (CartEntry entry in e.NewItems)
            {
                cartEntries.Add(new CartEntryViewModel(entry, cart));
                // Subscribe to nested collections
                entry.Recipe.Ingredients.CollectionChanged += OnRecipeIngredientsChanged;
                entry.PropertyChanged += OnEntryCountChanged;
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
                entry.Recipe.Ingredients.CollectionChanged -= OnRecipeIngredientsChanged;
                entry.PropertyChanged -= OnEntryCountChanged;
            }

        RecomputeIngredientsSummary();
    }

    private void OnRecipeIngredientsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RecomputeIngredientsSummary();

    private void OnEntryCountChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartEntry.Count))
            RecomputeIngredientsSummary();
    }

    private void RecomputeIngredientsSummary()
    {
        IngredientCounts = cart.ComputeIngredientCounts()
            .Select(x => new IngredientCountViewModel(x.Ingredient.ToString(), x.Count))
            .ToArray();
    }
}