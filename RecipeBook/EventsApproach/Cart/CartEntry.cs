using RecipeBook.Common;
using RecipeBook.EventsApproach.Recipes;

namespace RecipeBook.EventsApproach.Cart;

public class CartEntry : NotifyObject
{
    private int count;

    public CartEntry(Recipe recipe, int count)
    {
        Recipe = recipe;
        this.count = count;
    }

    public Recipe Recipe { get; }

    public int Count
    {
        get => count;
        set => Set(ref count, value);
    }
}
