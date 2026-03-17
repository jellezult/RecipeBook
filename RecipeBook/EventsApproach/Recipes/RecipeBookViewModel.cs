using CommunityToolkit.Mvvm.Input;
using RecipeBook.Common;
using RecipeBook.EventsApproach.Cart;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RecipeBook.EventsApproach.Recipes;

public class RecipeBookViewModel : NotifyObject, IDisposable
{
    private readonly RecipeBook recipeBook;
    private readonly GroceryCart groceryCart;

    private readonly ObservableCollection<RecipeViewModel> recipes = new();
    private RecipeViewModel? selectedRecipe;
    private string newRecipeName = string.Empty;

    private RelayCommand? addRecipeCommand;

    public RecipeBookViewModel(RecipeBook recipeBook, GroceryCart groceryCart)
    {
        this.recipeBook = recipeBook;
        this.groceryCart = groceryCart;

        // Seed ViewModels for existing recipes
        foreach (var recipe in recipeBook.Recipes)
            recipes.Add(new RecipeViewModel(recipe));

        // Subscribe to model collection changes — analogue of .Transform().Bind().DisposeMany()
        recipeBook.Recipes.CollectionChanged += OnModelRecipesChanged;
    }

    public RelayCommand AddRecipeCommand => addRecipeCommand ??= new(AddRecipe, CanAddRecipe);

    public RelayCommand<RecipeViewModel> RemoveRecipeCommand => new(RemoveRecipe);

    public RelayCommand<RecipeViewModel> AddToCartCommand => new(AddToCart);

    public ObservableCollection<RecipeViewModel> Recipes => recipes;

    public RecipeViewModel? SelectedRecipe
    {
        get => selectedRecipe;
        set => Set(ref selectedRecipe, value);
    }

    public string NewRecipeName
    {
        get => newRecipeName;
        set
        {
            Set(ref newRecipeName, value);
            addRecipeCommand?.NotifyCanExecuteChanged();
        }
    }

    public void Dispose()
    {
        recipeBook.Recipes.CollectionChanged -= OnModelRecipesChanged;
        foreach (var vm in recipes)
            vm.Dispose();
    }

    private void OnModelRecipesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            foreach (Recipe r in e.NewItems)
                recipes.Add(new RecipeViewModel(r));

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
            foreach (Recipe r in e.OldItems)
            {
                var vm = recipes.FirstOrDefault(v => v.Recipe == r);
                if (vm is not null)
                {
                    recipes.Remove(vm);
                    vm.Dispose();
                }
            }
    }

    private bool CanAddRecipe() => !string.IsNullOrWhiteSpace(NewRecipeName);

    private void AddRecipe()
    {
        recipeBook.AddRecipe(newRecipeName);
        NewRecipeName = string.Empty;
    }

    private void RemoveRecipe(RecipeViewModel? vm)
    {
        if (vm is null)
            return;

        recipeBook.RemoveRecipe(vm.Recipe);
    }

    private void AddToCart(RecipeViewModel? vm)
    {
        if (vm is null)
            return;

        groceryCart.AddOrIncrement(vm.Recipe);
    }
}
