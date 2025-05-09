using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace TournamentManager.DataAccess.Data;

internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TournamentDbContext>
{
    public TournamentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TournamentDbContext>();
        var connectionString = "Data Source=DESKTOP-L4MMGR0\\SQLEXPRESS;Integrated Security=True;" +
            "Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

        builder.UseSqlServer(connectionString);

        return new TournamentDbContext(builder.Options);
    }
}
