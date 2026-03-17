using RecipeBook.DynamicDataApproach.Recipes;

namespace RecipeBook.DynamicDataApproach.Cart;

public record CartEntry(Recipe Recipe, int Count);
