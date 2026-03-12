using DynamicData;

namespace RecipeBook.Models;

public class Recipe
{
    private readonly SourceList<Ingredient> ingredients = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public IObservable<IChangeSet<Ingredient>> IngredientsChanges => this.ingredients.Connect();
    public IReadOnlyList<Ingredient> IngredientsSnapshot => this.ingredients.Items.ToList();

    public Recipe(string name)
    {
        Name = name;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (!this.ingredients.Items.Contains(ingredient))
            this.ingredients.Add(ingredient);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        this.ingredients.Remove(ingredient);
    }
}
