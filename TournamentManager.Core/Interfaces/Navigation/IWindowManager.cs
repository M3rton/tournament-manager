namespace TournamentManager.Core.Interfaces.Navigation;

public interface IWindowManager
{
    void ShowWindow<TViewModel>(Action<TViewModel> setup, object? owner = null) where TViewModel : class;
    void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : class;
}
