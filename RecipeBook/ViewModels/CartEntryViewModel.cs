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

    public string RecipeName => this.entry.Recipe.Name;
    public Guid Id => this.entry.Id;

    public int Count
    {
        get => this.count;
        private set => Set(ref this.count, value);
    }

    public RelayCommand IncrementCommand { get; }
    public RelayCommand DecrementCommand { get; }
    public RelayCommand RemoveCommand { get; }

    public CartEntryViewModel(CartEntry entry, GroceryCart cart)
    {
        this.entry = entry;
        this.cart = cart;
        this.count = entry.Count;

        IncrementCommand = new RelayCommand(() => this.cart.AddOrIncrement(this.entry.Recipe));
        DecrementCommand = new RelayCommand(() => this.cart.Decrement(this.entry));
        RemoveCommand = new RelayCommand(() => this.cart.Remove(this.entry));

        // Sync Count property whenever the model's count changes
        this.disposables.Add(
            entry.CountChanged
                .Subscribe(c => Count = c));
    }

    public CartEntry GetModel() => this.entry;

    public void Dispose() => this.disposables.Dispose();
}
