using System.Reactive.Subjects;

namespace RecipeBook.Models;

public class CartEntry
{
    private readonly Subject<int> countSubject = new();
    private int count = 1;

    public Guid Id => this.Recipe.Id;
    public Recipe Recipe { get; }

    public int Count
    {
        get => this.count;
        set
        {
            this.count = value;
            this.countSubject.OnNext(value);
        }
    }

    public IObservable<int> ObserveCount() => this.countSubject;

    public CartEntry(Recipe recipe)
    {
        this.Recipe = recipe;
    }
}
