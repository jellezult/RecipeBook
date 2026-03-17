using DynamicData;
using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.DynamicDataApproach.Cart;

public class GroceryCartViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly GroceryCart cart;

    private readonly ReadOnlyObservableCollection<CartEntryViewModel> cartEntries;
    private IngredientCountViewModel[] ingredientCounts = [];

    public GroceryCartViewModel(GroceryCart cart)
    {
        this.cart = cart;

        // Observe the backend CartEntry collection
        // -> transform to ObservableCollection<CartEntryViewModel>
        cart.ObserveEntries()
            .Transform(entry => new CartEntryViewModel(entry, cart))
            .ObserveOnDispatcher() // dispatch to UI thread from this point on
            .Bind(out cartEntries) // bind to an ObservableCollection in this ViewModel
            .DisposeMany() // dispose any ViewModel entries when they get removed from the collection
            .Subscribe() // activates the subscription
            .DisposeWith(disposables); // useful pattern if we want to dispose all subscriptions later

        // Observe the backend CartEntry collection, and its nested ingredient lists
        // -> trigger a recomputation of the ingredients summary on each relevant change:
        //      -> Change of Recipes in Cart (=CartEntry collection) should trigger a recompute
        //      -> Change of Ingredient collection in a Recipe should trigger a recompute if that Recipe is in the Cart
        cart.ObserveEntries()
            .MergeManyChangeSets(entry => entry.Recipe.ObserveIngredients()) // respond to changes of nested collections
            .Throttle(TimeSpan.FromMilliseconds(50)) // batch rapid changes
            .ObserveOnDispatcher()
            .Subscribe(RecomputeIngredientsSummary) // subscribe + custom handler for changes
            .DisposeWith(disposables);
    }

    public ReadOnlyObservableCollection<CartEntryViewModel> CartEntries => cartEntries;

    public IngredientCountViewModel[] IngredientCounts
    {
        get => ingredientCounts;
        private set => Set(ref ingredientCounts, value);
    }

    public void Dispose() => disposables.Dispose();

    private void RecomputeIngredientsSummary(IChangeSet<Ingredient> set)
    {
        // In this case we don't use the change-set itself since we just do a full recomputation.
        // In alternative scenarios the change-set could be used for more custom fine-grained updating.

        IngredientCounts = cart.ComputeIngredientCounts()
            .Select(x => new IngredientCountViewModel(x.Ingredient.ToString(), x.Count))
            .ToArray();
    }
}
