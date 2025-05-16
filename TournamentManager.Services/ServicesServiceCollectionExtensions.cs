using Microsoft.Extensions.DependencyInjection;
using TournamentManager.Core.Interfaces.Services;

namespace TournamentManager.Services;

public static class ServicesServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITournamentsService, TournamentsService>();
        services.AddScoped<ITeamsService, TeamsService>();
        services.AddScoped<IPlayersService, PlayersService>();
        services.AddScoped<IMatchesService, MatchesService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IBracketGenerationService, BracketGenerationService>();

        return services;
    }
}
