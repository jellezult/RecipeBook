using DynamicData;

namespace RecipeBook.Recipes;

public class Recipe
{
    private readonly SourceList<Ingredient> ingredients = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public IObservable<IChangeSet<Ingredient>> ObserveIngredients() => ingredients.Connect();
    public IReadOnlyList<Ingredient> CurrentIngredients => ingredients.Items.ToList();

    public Recipe(string name)
    {
        Name = name;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (!ingredients.Items.Contains(ingredient))
            ingredients.Add(ingredient);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        ingredients.Remove(ingredient);
    }
}
