namespace RecipeBook.Models;

public class CartEntry : NotifyObject
{
    private int count = 1;

    public Guid Id => this.Recipe.Id;
    public Recipe Recipe { get; }

    public CartEntry(Recipe recipe)
    {
        this.Recipe = recipe;
    }

    public int Count
    {
        get => count;
        set => Set(ref count, value);
    }
}
