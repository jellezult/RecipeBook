using DynamicData;
using RecipeBook.Cart;
using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.Recipes;

public class RecipeBookViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly RecipeBook recipeBook;
    private readonly GroceryCart groceryCart;

    private readonly ReadOnlyObservableCollection<RecipeViewModel> recipes;
    private RecipeViewModel? selectedRecipe;
    private string newRecipeName = string.Empty;

    private RelayCommand? addRecipeCommand;
    private RelayCommand<RecipeViewModel>? removeRecipeCommand;
    private RelayCommand<RecipeViewModel>? addToCartCommand;

    public RecipeBookViewModel(RecipeBook recipeBook, GroceryCart groceryCart)
    {
        this.recipeBook = recipeBook;
        this.groceryCart = groceryCart;

        // Transform SourceList<Recipe> -> ReadOnlyObservableCollection<RecipeViewModel>
        // .DisposeMany() ensures RecipeViewModels are disposed when recipes are removed
        recipeBook.ObserveRecipes()
            .Transform(r => new RecipeViewModel(r))
            .ObserveOnDispatcher()
            .Bind(out recipes)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposables);
    }

    public ReadOnlyObservableCollection<RecipeViewModel> Recipes => recipes;

    public RecipeViewModel? SelectedRecipe
    {
        get => selectedRecipe;
        set => Set(ref selectedRecipe, value);
    }

    public string NewRecipeName
    {
        get => newRecipeName;
        set => Set(ref newRecipeName, value);
    }

    public RelayCommand AddRecipeCommand =>
        addRecipeCommand ??= new RelayCommand(
            () =>
            {
                recipeBook.AddRecipe(newRecipeName);
                NewRecipeName = string.Empty;
            },
            () => !string.IsNullOrWhiteSpace(newRecipeName));

    public RelayCommand<RecipeViewModel> RemoveRecipeCommand =>
        removeRecipeCommand ??= new RelayCommand<RecipeViewModel>(
            vm => recipeBook.RemoveRecipe(vm.Recipe));

    public RelayCommand<RecipeViewModel> AddToCartCommand =>
        addToCartCommand ??= new RelayCommand<RecipeViewModel>(
            vm => groceryCart.AddOrIncrement(vm.Recipe));

    public void Dispose() => disposables.Dispose();
}
