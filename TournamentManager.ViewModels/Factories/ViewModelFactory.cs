using TournamentManager.ViewModels.Interfaces;

namespace TournamentManager.ViewModels.Factories;

internal class ViewModelFactory<T> : IViewModelFactory<T>
{
    private readonly Func<T> _factory;

    public ViewModelFactory(Func<T> factory)
    {
        _factory = factory;
    }

    public T Create()
    {
        return _factory();
    }
}
