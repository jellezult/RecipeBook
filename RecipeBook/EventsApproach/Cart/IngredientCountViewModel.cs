namespace RecipeBook.EventsApproach.Cart;

public class IngredientCountViewModel
{
    public IngredientCountViewModel(string ingredientName, int totalCount)
    {
        IngredientName = ingredientName;
        TotalCount = totalCount;
    }

    public string IngredientName { get; }

    public int TotalCount { get; }
}
