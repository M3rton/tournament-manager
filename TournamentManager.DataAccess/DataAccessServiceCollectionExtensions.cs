using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.DataAccess.Data;
using TournamentManager.DataAccess.Repositories;

namespace TournamentManager.DataAccess;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        string connectionString = "Data Source=DESKTOP-L4MMGR0\\SQLEXPRESS;Integrated Security=True;" +
            "Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

        services.AddEntityFrameworkProxies();

        services.AddDbContext<TournamentDbContext>(options =>
            options.UseLazyLoadingProxies().UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
