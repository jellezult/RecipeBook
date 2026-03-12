using DynamicData;
using RecipeBook.Recipes;

namespace RecipeBook.Cart;

public class GroceryCart
{
    private readonly SourceCache<CartEntry, Guid> entries = new(e => e.Id);

    public IObservable<IChangeSet<CartEntry, Guid>> ObserveEntries() => entries.Connect();

    public IEnumerable<CartEntry> CurrentEntries => entries.Items;

    public void AddOrIncrement(Recipe recipe)
    {
        var existing = entries.Lookup(recipe.Id);
        if (existing.HasValue)
            existing.Value.Count++;
        else
            entries.AddOrUpdate(new CartEntry(recipe));
    }

    public void Decrement(CartEntry entry)
    {
        if (entry.Count <= 1)
            Remove(entry);
        else
            entry.Count--;
    }

    public void Remove(CartEntry entry)
    {
        entries.Remove(entry);
    }
}
