using CommunityToolkit.Mvvm.ComponentModel;
using TournamentManager.Core.Entities;

namespace TournamentManager.ViewModels.ViewModels
{
    public abstract class UserViewModel : ObservableObject
    {
        public User? User { get; set; }

        public event EventHandler<UserViewModel>? ChangeViewModel;

        protected void OnChangeViewModel(UserViewModel viewModel)
        {
            ChangeViewModel?.Invoke(this, viewModel);
        }
    }
}
