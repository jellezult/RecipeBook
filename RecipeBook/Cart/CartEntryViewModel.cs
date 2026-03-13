using CommunityToolkit.Mvvm.Input;
using RecipeBook.Common;
using RecipeBook.Recipes;
using System.Reactive.Disposables;

namespace RecipeBook.Cart;

public class CartEntryViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly CartEntry entry;
    private readonly GroceryCart cart;

    private RelayCommand? incrementCommand;
    private RelayCommand? decrementCommand;
    private RelayCommand? removeCommand;

    public CartEntryViewModel(CartEntry entry, GroceryCart cart)
    {
        this.entry = entry;
        this.cart = cart;
    }

    public RelayCommand IncrementCommand => incrementCommand ??= new(Increment);

    public RelayCommand DecrementCommand => decrementCommand ??= new(Decrement);

    public RelayCommand RemoveCommand => removeCommand ??= new(Remove);

    public CartEntry Entry => entry;

    public Recipe Recipe => entry.Recipe;

    public void Dispose() => disposables.Dispose();

    private void Increment() => cart.AddOrIncrement(entry.Recipe);

    private void Decrement() => cart.Decrement(entry);

    private void Remove() => cart.Remove(entry);
}
