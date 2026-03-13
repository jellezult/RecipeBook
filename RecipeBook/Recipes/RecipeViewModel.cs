using CommunityToolkit.Mvvm.Input;
using DynamicData;
using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.Recipes;

public class RecipeViewModel : NotifyObject, IDisposable
{
    private static readonly IReadOnlyList<Ingredient> AllIngredients = Enum.GetValues<Ingredient>().ToList();

    private readonly CompositeDisposable disposables = new();
    private readonly Recipe recipe;
    private readonly ReadOnlyObservableCollection<Ingredient> ingredients;

    private Ingredient? selectedIngredientToAdd;

    private RelayCommand? addIngredientCommand;
    private RelayCommand<Ingredient>? removeIngredientCommand;

    public RecipeViewModel(Recipe recipe)
    {
        this.recipe = recipe;

        recipe.ObserveIngredients()
            .ObserveOnDispatcher()
            .Bind(out ingredients)
            .Do(_ => OnPropertyChanged(nameof(AvailableIngredients)))
            .Subscribe()
            .DisposeWith(disposables);
    }

    public RelayCommand AddIngredientCommand => addIngredientCommand ??= new(AddIngredient, CanAddIngredient);

    public RelayCommand<Ingredient> RemoveIngredientCommand => removeIngredientCommand ??= new(RemoveIngredient);

    public Recipe Recipe => recipe;

    public ReadOnlyObservableCollection<Ingredient> Ingredients => ingredients;

    public Ingredient[] AvailableIngredients => AllIngredients.Where(i => !ingredients.Contains(i)).ToArray();

    public Ingredient? SelectedIngredientToAdd
    {
        get => selectedIngredientToAdd;
        set => Set(ref selectedIngredientToAdd, value);
    }

    public void Dispose() => disposables.Dispose();

    private bool CanAddIngredient() => selectedIngredientToAdd.HasValue;

    private void AddIngredient()
    {
        if (selectedIngredientToAdd is not Ingredient ingredient)
            return;

        recipe.AddIngredient(ingredient);
        SelectedIngredientToAdd = null;
    }

    private void RemoveIngredient(Ingredient ingredient)
    {
        recipe.RemoveIngredient(ingredient);
    }
}
