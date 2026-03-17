using CommunityToolkit.Mvvm.Input;
using RecipeBook.Common;
using RecipeBook.EventsApproach.Recipes;
using System.ComponentModel;

namespace RecipeBook.EventsApproach.Cart;

public class CartEntryViewModel : NotifyObject, IDisposable
{
    private readonly CartEntry entry;
    private readonly GroceryCart cart;

    public CartEntryViewModel(CartEntry entry, GroceryCart cart)
    {
        this.entry = entry;
        this.cart = cart;
        entry.PropertyChanged += OnEntryPropertyChanged;
    }

    public RelayCommand IncrementCommand => new(Increment);

    public RelayCommand DecrementCommand => new(Decrement);

    public RelayCommand RemoveCommand => new(Remove);

    public CartEntry Entry => entry;

    public Recipe Recipe => entry.Recipe;

    public int Count => entry.Count;

    public void Dispose()
    {
        entry.PropertyChanged -= OnEntryPropertyChanged;
    }

    private void OnEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CartEntry.Count))
            OnPropertyChanged(nameof(Count));
    }

    private void Increment() => cart.AddOrIncrement(entry.Recipe);

    private void Decrement() => cart.Decrement(entry);

    private void Remove() => cart.Remove(entry);
}
