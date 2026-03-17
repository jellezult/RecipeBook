using System.Windows;
using DD_RecipeBook = RecipeBook.DynamicDataApproach.Recipes.RecipeBook;
using DD_GroceryCart = RecipeBook.DynamicDataApproach.Cart.GroceryCart;
using DD_RecipeBookViewModel = RecipeBook.DynamicDataApproach.Recipes.RecipeBookViewModel;
using DD_GroceryCartViewModel = RecipeBook.DynamicDataApproach.Cart.GroceryCartViewModel;
using EV_RecipeBook = RecipeBook.EventsApproach.Recipes.RecipeBook;
using EV_GroceryCart = RecipeBook.EventsApproach.Cart.GroceryCart;
using EV_RecipeBookViewModel = RecipeBook.EventsApproach.Recipes.RecipeBookViewModel;
using EV_GroceryCartViewModel = RecipeBook.EventsApproach.Cart.GroceryCartViewModel;

namespace RecipeBook;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // DynamicData composition root
        var ddRecipeBook = new DD_RecipeBook();
        var ddCart = new DD_GroceryCart();
        DD_RecipeBookView.DataContext = new DD_RecipeBookViewModel(ddRecipeBook, ddCart);
        DD_GroceryCartView.DataContext = new DD_GroceryCartViewModel(ddCart);

        // Events composition root
        var evRecipeBook = new EV_RecipeBook();
        var evCart = new EV_GroceryCart();
        EV_RecipeBookView.DataContext = new EV_RecipeBookViewModel(evRecipeBook, evCart);
        EV_GroceryCartView.DataContext = new EV_GroceryCartViewModel(evCart);
    }
}
