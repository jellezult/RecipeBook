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

    private RelayCommand? addIngredientCommand;
    private RelayCommand<Ingredient>? removeIngredientCommand;

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

    public RelayCommand AddIngredientCommand =>
        this.addIngredientCommand ??= new RelayCommand(
            () =>
            {
                if (this.selectedIngredientToAdd.HasValue)
                {
                    this.recipe.AddIngredient(this.selectedIngredientToAdd.Value);
                    SelectedIngredientToAdd = null;
                }
            },
            () => this.selectedIngredientToAdd.HasValue);

    public RelayCommand<Ingredient> RemoveIngredientCommand =>
        this.removeIngredientCommand ??= new RelayCommand<Ingredient>(
            ingredient => this.recipe.RemoveIngredient(ingredient));

    public RecipeViewModel(Recipe recipe)
    {
        this.recipe = recipe;

        recipe.ObserveIngredients()
            .ObserveOnDispatcher()
            .Bind(out this.ingredients)
            .Do(_ => OnPropertyChanged(nameof(AvailableIngredients)))
            .Subscribe()
            .DisposeWith(this.disposables);
    }

    public Recipe GetModel() => this.recipe;

    public void Dispose() => this.disposables.Dispose();
}
