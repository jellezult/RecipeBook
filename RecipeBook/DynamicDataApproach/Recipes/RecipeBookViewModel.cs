using CommunityToolkit.Mvvm.Input;
using DynamicData;
using RecipeBook.Common;
using RecipeBook.DynamicDataApproach.Cart;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.DynamicDataApproach.Recipes;

public class RecipeBookViewModel : NotifyObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();
    private readonly RecipeBook recipeBook;
    private readonly GroceryCart groceryCart;

    private readonly ReadOnlyObservableCollection<RecipeViewModel> recipes;
    private RecipeViewModel? selectedRecipe;
    private string newRecipeName = string.Empty;

    private RelayCommand? addRecipeCommand;

    public RecipeBookViewModel(RecipeBook recipeBook, GroceryCart groceryCart)
    {
        this.recipeBook = recipeBook;
        this.groceryCart = groceryCart;

        // Observe the backend recipe list
        // -> goal here is simply to transform it to an ObservableCollection<RecipeViewModel>
        recipeBook.ObserveRecipes()
            .Transform(r => new RecipeViewModel(r))
            .ObserveOnDispatcher() // ensures we are on the UI thread from here on
            .Bind(out recipes) // updates the ObservableCollection when the backend list changes
            .DisposeMany() // ensures RecipeViewModels are disposed when recipes are removed
            .Subscribe() // activates the subscription
            .DisposeWith(disposables); // useful pattern if we want to dispose all subscriptions later
    }

    public RelayCommand AddRecipeCommand => addRecipeCommand ??= new(AddRecipe, CanAddRecipe);

    public RelayCommand<RecipeViewModel> RemoveRecipeCommand => new(RemoveRecipe);

    public RelayCommand<RecipeViewModel> AddToCartCommand => new(AddToCart);

    public ReadOnlyObservableCollection<RecipeViewModel> Recipes => recipes;

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

    public void Dispose() => disposables.Dispose();
}
