using DynamicData;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RecipeBook.Recipes;

public class RecipeViewModel : NotifyObject, IDisposable
{
    private static readonly IReadOnlyList<Ingredient> AllIngredients =
        Enum.GetValues<Ingredient>().ToList();

    private readonly CompositeDisposable disposables = new();
    private readonly Recipe recipe;

    private ReadOnlyObservableCollection<Ingredient> ingredients = null!;
    private Ingredient? selectedIngredientToAdd;

    private RelayCommand? addIngredientCommand;
    private RelayCommand<Ingredient>? removeIngredientCommand;

    public Guid Id => recipe.Id;
    public string Name => recipe.Name;

    public ReadOnlyObservableCollection<Ingredient> Ingredients => ingredients;

    public IEnumerable<Ingredient> AvailableIngredients =>
        AllIngredients.Where(i => !ingredients.Contains(i)).ToList();

    public Ingredient? SelectedIngredientToAdd
    {
        get => selectedIngredientToAdd;
        set => Set(ref selectedIngredientToAdd, value);
    }

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

    public RelayCommand AddIngredientCommand =>
        addIngredientCommand ??= new RelayCommand(
            () =>
            {
                if (selectedIngredientToAdd.HasValue)
                {
                    recipe.AddIngredient(selectedIngredientToAdd.Value);
                    SelectedIngredientToAdd = null;
                }
            },
            () => selectedIngredientToAdd.HasValue);

    public RelayCommand<Ingredient> RemoveIngredientCommand =>
        removeIngredientCommand ??= new RelayCommand<Ingredient>(
            ingredient => recipe.RemoveIngredient(ingredient));

    public Recipe Recipe => recipe;

    public void Dispose() => disposables.Dispose();
}
