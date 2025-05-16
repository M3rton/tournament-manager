namespace TournamentManager.Core.Interfaces.Navigation;

public interface IWindowService
{
    void ShowWindow<TViewModel>(Action<TViewModel> setup, object? owner = null) where TViewModel : class;

    Task<string?> ShowSaveFileDialog(string? title = null, string? filter = null, string? defaultFileName = null);
    void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : class;
}
