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

    private RelayCommand? addRecipeCommand;
    private RelayCommand<RecipeViewModel>? removeRecipeCommand;
    private RelayCommand<RecipeViewModel>? addToCartCommand;

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

    public RelayCommand AddRecipeCommand =>
        this.addRecipeCommand ??= new RelayCommand(
            () =>
            {
                this.recipeBook.AddRecipe(this.newRecipeName);
                NewRecipeName = string.Empty;
            },
            () => !string.IsNullOrWhiteSpace(this.newRecipeName));

    public RelayCommand<RecipeViewModel> RemoveRecipeCommand =>
        this.removeRecipeCommand ??= new RelayCommand<RecipeViewModel>(
            vm => this.recipeBook.RemoveRecipe(vm.GetModel()));

    public RelayCommand<RecipeViewModel> AddToCartCommand =>
        this.addToCartCommand ??= new RelayCommand<RecipeViewModel>(
            vm => this.groceryCart.AddOrIncrement(vm.GetModel()));

    public RecipeBookViewModel(Models.RecipeBook recipeBook, GroceryCart groceryCart)
    {
        this.recipeBook = recipeBook;
        this.groceryCart = groceryCart;

        // Transform SourceList<Recipe> -> ReadOnlyObservableCollection<RecipeViewModel>
        // .DisposeMany() ensures RecipeViewModels are disposed when recipes are removed
        recipeBook.ObserveRecipes()
            .Transform(r => new RecipeViewModel(r))
            .ObserveOnDispatcher()
            .Bind(out this.recipes)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(this.disposables);
    }

    public void Dispose() => this.disposables.Dispose();
}
