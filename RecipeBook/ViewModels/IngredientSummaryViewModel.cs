namespace RecipeBook.ViewModels;

public class IngredientSummaryViewModel
{
    public string IngredientName { get; }
    public int TotalCount { get; }

    public IngredientSummaryViewModel(string ingredientName, int totalCount)
    {
        IngredientName = ingredientName;
        TotalCount = totalCount;
    }
}
