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

    public RelayCommand IncrementCommand =>
        incrementCommand ??= new RelayCommand(() => cart.AddOrIncrement(entry.Recipe));

    public RelayCommand DecrementCommand =>
        decrementCommand ??= new RelayCommand(() => cart.Decrement(entry));

    public RelayCommand RemoveCommand =>
        removeCommand ??= new RelayCommand(() => cart.Remove(entry));

    public Recipe Recipe => entry.Recipe;

    public CartEntry Entry => entry;

    public void Dispose() => disposables.Dispose();
}
