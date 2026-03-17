using CommunityToolkit.Mvvm.Input;
using RecipeBook.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RecipeBook.EventsApproach.Recipes;

public class RecipeViewModel : NotifyObject, IDisposable
{
    private static readonly IReadOnlyList<Ingredient> AllIngredients = Enum.GetValues<Ingredient>().ToList();

    private readonly Recipe recipe;
    private readonly ObservableCollection<Ingredient> ingredients = new();

    private Ingredient? selectedIngredientToAdd;
    private RelayCommand? addIngredientCommand;

    public RecipeViewModel(Recipe recipe)
    {
        this.recipe = recipe;

        foreach (var ingredient in recipe.Ingredients)
            ingredients.Add(ingredient);

        recipe.Ingredients.CollectionChanged += OnModelIngredientsChanged;
    }

    public RelayCommand AddIngredientCommand => addIngredientCommand ??= new(AddIngredient, CanAddIngredient);

    public RelayCommand<Ingredient> RemoveIngredientCommand => new(RemoveIngredient);

    public Recipe Recipe => recipe;

    public ObservableCollection<Ingredient> Ingredients => ingredients;

    public Ingredient[] AvailableIngredients => AllIngredients.Where(i => !ingredients.Contains(i)).ToArray();

    public Ingredient? SelectedIngredientToAdd
    {
        get => selectedIngredientToAdd;
        set
        {
            Set(ref selectedIngredientToAdd, value);
            addIngredientCommand?.NotifyCanExecuteChanged();
        }
    }

    public void Dispose()
    {
        recipe.Ingredients.CollectionChanged -= OnModelIngredientsChanged;
    }

    private void OnModelIngredientsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AppExtensions.InvokeUI(() =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
                foreach (Ingredient i in e.NewItems)
                    ingredients.Add(i);

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
                foreach (Ingredient i in e.OldItems)
                    ingredients.Remove(i);

            OnPropertyChanged(nameof(AvailableIngredients));
        });
    }

    private bool CanAddIngredient() => SelectedIngredientToAdd.HasValue;

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
