﻿using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;

namespace TournamentManager.Core.Interfaces.Services;

public interface ITournamentsService
{
    Task<bool> CanCreateTournamentAsync(string tournamentName);

    Task CreateTournamentAsync(
        string tournamentName,
        StrategyType strategyType,
        int maxTeams,
        string? description,
        Player player);

    Task AddTeamAsync(Tournament tournament, string teamName);

    Task SaveWinnerAsync(Tournament tournament, Team team);

    Task ExportTournamentAsync(Tournament tournament, StreamWriter streamWriter);
}
