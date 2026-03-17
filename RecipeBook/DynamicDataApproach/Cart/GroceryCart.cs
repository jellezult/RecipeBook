using DynamicData;
using DynamicData.Kernel;
using RecipeBook.DynamicDataApproach.Recipes;

namespace RecipeBook.DynamicDataApproach.Cart;

public class GroceryCart
{
    private readonly SourceCache<CartEntry, Guid> entries = new(e => e.Recipe.Id);

    public IObservable<IChangeSet<CartEntry, Guid>> ObserveEntries() => entries.Connect();

    public IEnumerable<CartEntry> CurrentEntries => entries.Items;

    public void AddOrIncrement(Recipe recipe)
    {
        CartEntry? entry = entries.Lookup(recipe.Id).ValueOrDefault();
        entry ??= new CartEntry(recipe, 0);

        entries.AddOrUpdate(entry with { Count = entry.Count + 1 });
    }

    public void Decrement(CartEntry entry)
    {
        if (entry.Count <= 1)
            Remove(entry);
        else
            entries.AddOrUpdate(entry with { Count = entry.Count - 1 });
    }

    public void Remove(CartEntry entry)
    {
        entries.Remove(entry);
    }

    public IReadOnlyList<IngredientWithCount> ComputeIngredientCounts()
    {
        return CurrentEntries
            .SelectMany(entry => entry.Recipe.CurrentIngredients.Select(i => new IngredientWithCount(i, entry.Count)))
            .GroupBy(x => x.Ingredient)
            .Select(group => new IngredientWithCount(group.Key, group.Sum(x => x.Count)))
            .ToArray();
    }
}
