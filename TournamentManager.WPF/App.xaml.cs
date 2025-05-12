using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TournamentManager.Core.Interfaces.Navigation;
using TournamentManager.DataAccess;
using TournamentManager.Services;
using TournamentManager.ViewModels;
using TournamentManager.ViewModels.ViewModels;
using TournamentManager.WPF.Services;
using TournamentManager.WPF.Views;

namespace TournamentManager.WPF;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddRepositories();
        services.AddServices();
        services.AddViewModels();

        services.AddSingleton(CreateViewModelToViewMap());
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<IEventAggregator, EventAggregator>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
        windowManager.ShowWindow<LoginWindowViewModel>(_ => { });

        base.OnStartup(e);
    }

    private static Dictionary<Type, Type> CreateViewModelToViewMap()
    {
        return new Dictionary<Type, Type>
        {
            { typeof(MainWindowViewModel), typeof(MainWindow) },
            { typeof(LoginWindowViewModel), typeof(LoginWindow) },
            { typeof(PopUpWindowViewModel), typeof(PopUpWindow)},
        };
    }
}
