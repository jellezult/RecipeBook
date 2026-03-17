using RecipeBook.Common;
using System.Collections.ObjectModel;

namespace RecipeBook.EventsApproach.Recipes;

public class Recipe
{
    private readonly ObservableCollection<Ingredient> ingredients = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public ObservableCollection<Ingredient> Ingredients => ingredients;
    public IReadOnlyList<Ingredient> CurrentIngredients => ingredients;

    public Recipe(string name)
    {
        Name = name;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (!ingredients.Contains(ingredient))
            ingredients.Add(ingredient);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        ingredients.Remove(ingredient);
    }
}
