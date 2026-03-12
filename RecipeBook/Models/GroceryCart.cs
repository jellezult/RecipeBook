using DynamicData;

namespace RecipeBook.Models;

public class GroceryCart
{
    private readonly SourceCache<CartEntry, Guid> entries = new(e => e.Id);

    public IObservable<IChangeSet<CartEntry, Guid>> ObserveEntries() => this.entries.Connect();

    public void AddOrIncrement(Recipe recipe)
    {
        var existing = this.entries.Lookup(recipe.Id);
        if (existing.HasValue)
            existing.Value.Count++;
        else
            this.entries.AddOrUpdate(new CartEntry(recipe));
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
        this.entries.Remove(entry);
    }
}
