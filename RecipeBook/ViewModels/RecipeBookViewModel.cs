using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using RecipeBook.Models;

namespace RecipeBook.ViewModels;

public class RecipeBookViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly Models.RecipeBook recipeBook;
    private readonly GroceryCart groceryCart;

    private ReadOnlyObservableCollection<RecipeViewModel> recipes = null!;
    private RecipeViewModel? selectedRecipe;
    private string newRecipeName = string.Empty;

    public ReadOnlyObservableCollection<RecipeViewModel> Recipes => this.recipes;

    public RecipeViewModel? SelectedRecipe
    {
        get => this.selectedRecipe;
        set => Set(ref this.selectedRecipe, value);
    }

    public string NewRecipeName
    {
        get => this.newRecipeName;
        set => Set(ref this.newRecipeName, value);
    }

    public RelayCommand AddRecipeCommand { get; }
    public RelayCommand<RecipeViewModel> RemoveRecipeCommand { get; }
    public RelayCommand<RecipeViewModel> AddToCartCommand { get; }

    public RecipeBookViewModel(Models.RecipeBook recipeBook, GroceryCart groceryCart)
    {
        this.recipeBook = recipeBook;
        this.groceryCart = groceryCart;

        AddRecipeCommand = new RelayCommand(
            () =>
            {
                this.recipeBook.AddRecipe(this.newRecipeName);
                NewRecipeName = string.Empty;
            },
            () => !string.IsNullOrWhiteSpace(this.newRecipeName));

        RemoveRecipeCommand = new RelayCommand<RecipeViewModel>(
            vm => this.recipeBook.RemoveRecipe(vm.GetModel()));

        AddToCartCommand = new RelayCommand<RecipeViewModel>(
            vm => this.groceryCart.AddOrIncrement(vm.GetModel()));

        // Transform SourceList<Recipe> -> ReadOnlyObservableCollection<RecipeViewModel>
        // .DisposeMany() ensures RecipeViewModels are disposed when recipes are removed
        this.disposables.Add(
            recipeBook.RecipesChanges
                .Transform(r => new RecipeViewModel(r))
                .Bind(out this.recipes)
                .DisposeMany()
                .Subscribe());
    }

    public void Dispose() => this.disposables.Dispose();
}
