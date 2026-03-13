using RecipeBook.Recipes;

namespace RecipeBook.Cart;

public record CartEntry(Recipe Recipe, int Count);