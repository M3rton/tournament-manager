using Microsoft.Extensions.DependencyInjection;
using TournamentManager.ViewModels.Factories;
using TournamentManager.ViewModels.Interfaces;
using TournamentManager.ViewModels.ViewModels;

namespace TournamentManager.ViewModels;

public static class ViewModelServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddFormFactory<MainWindowViewModel>();
        services.AddFormFactory<LoginWindowViewModel>();
        services.AddFormFactory<LoginViewModel>();
        services.AddFormFactory<RegisterViewModel>();
        services.AddFormFactory<PopUpWindowViewModel>();
        services.AddFormFactory<UpcomingTournamentsViewModel>();
        services.AddFormFactory<MyTournamentViewModel>();
        services.AddFormFactory<MyAccountViewModel>();
        services.AddFormFactory<MyTeamViewModel>();
        services.AddFormFactory<CreateTeamViewModel>();
        services.AddFormFactory<CreateTournamentViewModel>();

        return services;
    }

    private static IServiceCollection AddFormFactory<T>(this IServiceCollection services) where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(x => () => x.GetService<T>()!);
        services.AddSingleton<IViewModelFactory<T>, ViewModelFactory<T>>();

        return services;
    }
}
