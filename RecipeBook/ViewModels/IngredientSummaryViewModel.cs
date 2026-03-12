namespace RecipeBook.ViewModels;

public class IngredientSummaryViewModel
{
    public IngredientSummaryViewModel(string ingredientName, int totalCount)
    {
        IngredientName = ingredientName;
        TotalCount = totalCount;
    }

    public string IngredientName { get; }

    public int TotalCount { get; }
}
