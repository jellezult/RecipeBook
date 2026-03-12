using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using RecipeBook.Models;

namespace RecipeBook.ViewModels;

public class RecipeViewModel : NotifyObject, IDisposable
{
    private static readonly IReadOnlyList<Ingredient> AllIngredients =
        Enum.GetValues<Ingredient>().ToList();

    private readonly CompositeDisposable disposables = new();
    private readonly Recipe recipe;

    private ReadOnlyObservableCollection<Ingredient> ingredients = null!;
    private Ingredient? selectedIngredientToAdd;

    public Guid Id => this.recipe.Id;
    public string Name => this.recipe.Name;

    public ReadOnlyObservableCollection<Ingredient> Ingredients => this.ingredients;

    public IEnumerable<Ingredient> AvailableIngredients =>
        AllIngredients.Where(i => !this.ingredients.Contains(i)).ToList();

    public Ingredient? SelectedIngredientToAdd
    {
        get => this.selectedIngredientToAdd;
        set => Set(ref this.selectedIngredientToAdd, value);
    }

    public RelayCommand AddIngredientCommand { get; }
    public RelayCommand<Ingredient> RemoveIngredientCommand { get; }

    public RecipeViewModel(Recipe recipe)
    {
        this.recipe = recipe;

        AddIngredientCommand = new RelayCommand(
            () =>
            {
                if (this.selectedIngredientToAdd.HasValue)
                {
                    this.recipe.AddIngredient(this.selectedIngredientToAdd.Value);
                    SelectedIngredientToAdd = null;
                }
            },
            () => this.selectedIngredientToAdd.HasValue);

        RemoveIngredientCommand = new RelayCommand<Ingredient>(
            ingredient => this.recipe.RemoveIngredient(ingredient));

        // Bind ingredients list and notify AvailableIngredients whenever it changes.
        // All mutations happen on the UI thread (commands), so no ObserveOn is needed here.
        // In a multi-threaded scenario, add .ObserveOn(DispatcherScheduler.Current) before .Bind().
        this.disposables.Add(
            recipe.IngredientsChanges
                .Bind(out this.ingredients)
                .Do(_ => OnPropertyChanged(nameof(AvailableIngredients)))
                .Subscribe());
    }

    public Recipe GetModel() => this.recipe;

    public void Dispose() => this.disposables.Dispose();
}
