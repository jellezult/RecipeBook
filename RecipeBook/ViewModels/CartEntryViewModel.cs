using System.Reactive.Disposables;
using System.Reactive.Linq;
using RecipeBook.Models;

namespace RecipeBook.ViewModels;

public class CartEntryViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly CartEntry entry;
    private readonly GroceryCart cart;

    private int count;

    private RelayCommand? incrementCommand;
    private RelayCommand? decrementCommand;
    private RelayCommand? removeCommand;

    public string RecipeName => this.entry.Recipe.Name;
    public Guid Id => this.entry.Id;

    public int Count
    {
        get => this.count;
        private set => Set(ref this.count, value);
    }

    public RelayCommand IncrementCommand =>
        this.incrementCommand ??= new RelayCommand(() => this.cart.AddOrIncrement(this.entry.Recipe));

    public RelayCommand DecrementCommand =>
        this.decrementCommand ??= new RelayCommand(() => this.cart.Decrement(this.entry));

    public RelayCommand RemoveCommand =>
        this.removeCommand ??= new RelayCommand(() => this.cart.Remove(this.entry));

    public CartEntryViewModel(CartEntry entry, GroceryCart cart)
    {
        this.entry = entry;
        this.cart = cart;
        this.count = entry.Count;

        // Sync Count property whenever the model's count changes
        entry.ObserveCount()
            .Subscribe(c => Count = c)
            .DisposeWith(this.disposables);
    }

    public CartEntry GetModel() => this.entry;

    public void Dispose() => this.disposables.Dispose();
}
