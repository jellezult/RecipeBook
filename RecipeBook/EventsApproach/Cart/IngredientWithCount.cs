using RecipeBook.Common;

namespace RecipeBook.EventsApproach.Cart;

public readonly record struct IngredientWithCount(Ingredient Ingredient, int Count);
