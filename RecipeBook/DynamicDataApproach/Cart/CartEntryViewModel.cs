using CommunityToolkit.Mvvm.Input;
using RecipeBook.Common;
using RecipeBook.DynamicDataApproach.Recipes;
using System.Reactive.Disposables;

namespace RecipeBook.DynamicDataApproach.Cart;

public class CartEntryViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly CartEntry entry;
    private readonly GroceryCart cart;

    public CartEntryViewModel(CartEntry entry, GroceryCart cart)
    {
        this.entry = entry;
        this.cart = cart;
    }

    public RelayCommand IncrementCommand => new(Increment);

    public RelayCommand DecrementCommand => new(Decrement);

    public RelayCommand RemoveCommand => new(Remove);

    public CartEntry Entry => entry;

    public Recipe Recipe => entry.Recipe;

    public void Dispose() => disposables.Dispose();

    private void Increment() => cart.AddOrIncrement(entry.Recipe);

    private void Decrement() => cart.Decrement(entry);

    private void Remove() => cart.Remove(entry);
}
