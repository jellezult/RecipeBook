using RecipeBook.EventsApproach.Recipes;
using System.Collections.ObjectModel;

namespace RecipeBook.EventsApproach.Cart;

public class GroceryCart
{
    private readonly ObservableCollection<CartEntry> entries = new();

    public ObservableCollection<CartEntry> Entries => entries;

    public void AddOrIncrement(Recipe recipe)
    {
        var entry = entries.FirstOrDefault(e => e.Recipe.Id == recipe.Id);
        if (entry is null)
            entries.Add(new CartEntry(recipe, 1));
        else
            entry.Count++;
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

    public IReadOnlyList<IngredientWithCount> ComputeIngredientCounts()
    {
        return entries
            .SelectMany(entry => entry.Recipe.CurrentIngredients.Select(i => new IngredientWithCount(i, entry.Count)))
            .GroupBy(x => x.Ingredient)
            .Select(group => new IngredientWithCount(group.Key, group.Sum(x => x.Count)))
            .ToArray();
    }
}
