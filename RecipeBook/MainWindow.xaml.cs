using System.Windows;
using RecipeBook.Cart;
using RecipeBook.Recipes;
using RecipeBookModel = RecipeBook.Recipes.RecipeBook;

namespace RecipeBook;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Composition root: create models and wire up ViewModels
        var recipeBook = new RecipeBookModel();
        var groceryCart = new GroceryCart();

        RecipeBookView.DataContext = new RecipeBookViewModel(recipeBook, groceryCart);
        GroceryCartView.DataContext = new GroceryCartViewModel(groceryCart);
    }
}
