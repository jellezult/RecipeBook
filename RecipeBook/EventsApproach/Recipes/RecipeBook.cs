using RecipeBook.Common;
using System.Collections.ObjectModel;

namespace RecipeBook.EventsApproach.Recipes;

public class RecipeBook
{
    private readonly ObservableCollection<Recipe> recipes = new();

    public ObservableCollection<Recipe> Recipes => recipes;

    public RecipeBook()
    {
        var carbonara = new Recipe("Pasta Carbonara");
        carbonara.AddIngredient(Ingredient.Pasta);
        carbonara.AddIngredient(Ingredient.Eggs);
        carbonara.AddIngredient(Ingredient.GuancialeBacon);
        carbonara.AddIngredient(Ingredient.PecorinoRomano);
        carbonara.AddIngredient(Ingredient.BlackPepper);

        var risotto = new Recipe("Risotto Funghi");
        risotto.AddIngredient(Ingredient.ArborioRice);
        risotto.AddIngredient(Ingredient.PorciniMushrooms);
        risotto.AddIngredient(Ingredient.ParmesanCheese);
        risotto.AddIngredient(Ingredient.Butter);
        risotto.AddIngredient(Ingredient.WhiteWine);
        risotto.AddIngredient(Ingredient.Onion);

        var curry = new Recipe("Curry Madras");
        curry.AddIngredient(Ingredient.ChickenBreast);
        curry.AddIngredient(Ingredient.CurryPaste);
        curry.AddIngredient(Ingredient.CoconutMilk);
        curry.AddIngredient(Ingredient.Onion);
        curry.AddIngredient(Ingredient.Garlic);
        curry.AddIngredient(Ingredient.Rice);
        curry.AddIngredient(Ingredient.Coriander);

        recipes.Add(carbonara);
        recipes.Add(risotto);
        recipes.Add(curry);
    }

    public void AddRecipe(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            recipes.Add(new Recipe(name));
    }

    public void RemoveRecipe(Recipe recipe)
    {
        recipes.Remove(recipe);
    }
}
