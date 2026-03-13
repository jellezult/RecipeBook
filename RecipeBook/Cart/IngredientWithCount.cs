using RecipeBook.Recipes;

namespace RecipeBook.Cart;

public readonly record struct IngredientWithCount(Ingredient Ingredient, int Count);
