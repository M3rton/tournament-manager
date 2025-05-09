using TournamentManager.Core.Entities;

namespace TournamentManager.ViewModels.Interfaces;

public interface ILoginViewModel
{
    event EventHandler<ILoginViewModel>? ChangeViewModel;

    event EventHandler<User>? SignInSuccesful;
}
