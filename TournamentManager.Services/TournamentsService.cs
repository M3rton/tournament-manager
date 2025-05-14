using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.ObjectModel;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.Core.Interfaces.Repositories;
using TournamentManager.Core.Interfaces.Services;
using TournamentManager.Core.Options;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace TournamentManager.Services;

internal class TournamentsService : ITournamentsService
{
    private readonly IUnitOfWork _unitOfWork;

    public TournamentsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateTournamentAsync(string tournamentName, StrategyType strategyType, int maxTeams, string? description, Player player)
    {
        if (!await CanCreateTournamentAsync(tournamentName) || player.Tournament != null)
        {
            return;
        }

        var tournament = new Tournament
        {
            Name = tournamentName,
            Strategy = strategyType,
            MaxTeams = maxTeams,
            Description = description,
            Owner = player,
            Teams = new ObservableCollection<Team>(),
            Matches = new ObservableCollection<Match>()

        };
        player.Tournament = tournament;

        _unitOfWork.PlayersRepository.Update(player);
        await _unitOfWork.TournamentsRepository.AddAsync(tournament);

        await _unitOfWork.SaveAsync();
    }

    public async Task<bool> CanCreateTournamentAsync(string tournamentName)
    {
        return (await _unitOfWork.TournamentsRepository.GetAsync(t => t.Name == tournamentName)).FirstOrDefault() == null;
    }

    public async Task AddTeamAsync(Tournament tournament, string teamName)
    {
        Team? team = (await _unitOfWork.TeamsRepository.GetAsync(t => t.Name == teamName)).FirstOrDefault();

        if (team != null)
        {
            if (tournament.Teams.Contains(team))
            {
                return;
            }

            team.Tournaments.Add(tournament);
            tournament.Teams.Add(team);

            _unitOfWork.TeamsRepository.Update(team);
            _unitOfWork.TournamentsRepository.Update(tournament);

            await _unitOfWork.SaveAsync();
        }
    }

    public async Task SaveWinnerAsync(Tournament tournament, Team team)
    {
        tournament.Winner = team;

        _unitOfWork.TournamentsRepository.Update(tournament);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<Match>> GenerateBracketAsync(Tournament tournament)
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                return await GenerateSpiderBracketAsync(tournament);
            case StrategyType.Groups:
                return await GenerateGroupsBracketAsync(tournament);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<IEnumerable<Match>> GenerateSpiderBracketAsync(Tournament tournament)
    {
        async Task<IEnumerable<Match>> CreateMatches(int roundSize, List<Team> teams, int currentRound)
        {
            var newMatches = new List<Match>();
            int matchNumber = 1;

            for (int i = 0; i < roundSize; i++)
            {
                Match match = new Match
                {
                    Round = currentRound,
                    MatchNumber = matchNumber,
                    FirstTeam = teams[i],
                    SecondTeam = (roundSize * 2 - 1 - i) >= teams.Count ? null : teams[roundSize * 2 - 1 - i],
                    WinnerTeam = (roundSize * 2 - 1 - i) >= teams.Count ? teams[i] : null,
                    IsFinished = (roundSize * 2 - 1 - i) >= teams.Count ? true : false
                };

                if (!match.IsFinished)
                {
                    newMatches.Add(match);
                }

                await _unitOfWork.MatchesRepository.AddAsync(match);

                teams[i].Matches.Add(match);
                _unitOfWork.TeamsRepository.Update(teams[i]);

                if ((roundSize * 2 - 1 - i) < teams.Count)
                {
                    teams[roundSize * 2 - 1 - i].Matches.Add(match);
                    _unitOfWork.TeamsRepository.Update(teams[roundSize * 2 - 1 - i]);
                }

                tournament.Matches.Add(match);
                _unitOfWork.TournamentsRepository.Update(tournament);

                await _unitOfWork.SaveAsync();

                matchNumber++;
            }

            return newMatches;
        }

        if (tournament.Matches.Count == 0)
        {
            int teamsCount = tournament.Teams.Count;

            if (teamsCount < 2)
            {
                return Enumerable.Empty<Match>();
            }

            int bracketSize = 2;
            while (bracketSize < teamsCount)
            {
                bracketSize *= 2;
            }

            return await CreateMatches(bracketSize / 2, tournament.Teams.ToList(), 1);
        }
        else
        {
            List<Team> currentTeams = new List<Team>();

            int currentRound = tournament.Matches.Last().Round + 1;

            foreach (Match match in tournament.Matches)
            {
                if (!match.IsFinished)
                {
                    return Enumerable.Empty<Match>();
                }

                if (match.Round == currentRound - 1)
                {
                    currentTeams.Add(match.WinnerTeam!);
                }
            }

            if (currentTeams.Count <= 1)
            {
                return Enumerable.Empty<Match>();
            }

            return await CreateMatches(currentTeams.Count / 2, currentTeams, currentRound);
        }
    }

    private async Task<IEnumerable<Match>> GenerateGroupsBracketAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public async Task ExportTournament(Tournament tournament)
    {

    }

    public async Task ExportBracketAsImageAsync(Tournament tournament, BracketExportAsImageOptions options)
    {
        await DrawSpiderBracket(0, 0, 25, tournament.Matches, options);
    }

    private async Task DrawSpiderBracket(
        int offSetX,
        int offSetY,
        int maxMatchWidth,
        IEnumerable<Match> matchesInRounds,
        BracketExportAsImageOptions options)
    {
        if (!SystemFonts.TryGet(options.FontName, out FontFamily fontFamily))
        {
            throw new Exception($"Couldn't find font {options.FontName}");
        }

        Font font = fontFamily.CreateFont(options.FontSize, FontStyle.Regular);
        TextOptions textOptions = new TextOptions(font);

        string placeholderText = new string('A', maxMatchWidth + 5);
        FontRectangle textSize = TextMeasurer.MeasureSize(placeholderText, textOptions);

        double imageHeight = 2 * offSetX + (textSize.Height + 2 * options.TextPadding + options.SpaceBetweenMatches + 2 * options.LinesThickness) * Math.Pow(2, matchesInRounds.Count());
        double imageWidth = 2 * offSetY + (textSize.Width + 2 * options.TextPadding + options.SpaceBetweenRounds  + 2 * options.LinesThickness) * matchesInRounds.Count();

        using Image<Rgba32> image = new Image<Rgba32>((int)Math.Ceiling(imageWidth), (int)Math.Ceiling(imageHeight));

        image.Mutate(x => x.BackgroundColor(new Color(Rgba32.ParseHex("#FFFFFFFF"))));

        foreach (var match in matchesInRounds)
        {
            double roundHeightIdentation = 0;
            double matchHeight = (textSize.Height + 2 * options.TextPadding + 2 * options.LinesThickness);
            double matchWidth = (textSize.Width + 2 * options.TextPadding + 2 * options.LinesThickness);

            double matchIdentation = options.SpaceBetweenMatches * Math.Pow(2, match.Round - 1) * match.MatchNumber;

            if (match.Round > 1)
            {
                roundHeightIdentation = (matchHeight + options.SpaceBetweenRounds) * Math.Pow(2, match.Round - 2) + matchHeight * 0.5 - options.SpaceBetweenRounds * 0.5;
            }

            double pointX = offSetX + matchHeight * match.MatchNumber + options.SpaceBetweenMatches * roundHeightIdentation + matchIdentation;
            double pointY = offSetY + (matchWidth + options.SpaceBetweenRounds) * (match.Round - 1);
            Point point = new Point((int)pointX, (int) pointY);

            string firstTeamName = match.FirstTeam.Name;
            string secondTeamName = match.SecondTeam == null ? "" : match.SecondTeam!.Name;

            DrawMatch(image, point, firstTeamName, secondTeamName, font, options, (int) matchHeight, (int) matchWidth);


            SizeF rectSize = new SizeF((int)matchWidth, (int)matchHeight);

            image.Mutate(x => x.Draw(new Color(Rgba32.ParseHex("#000000FF")), options.LinesThickness, new RectangleF(point, rectSize)));

            image.Mutate(x => x.DrawText(
                firstTeamName,
                font,
                new Color(Rgba32.ParseHex("#000000FF")),
                point)
            );

            break;
        }

        await image.SaveAsBmpAsync("C:\\Users\\Marek\\Downloads\\Temp\\test.bmp");
    }

    private async Task DrawMatch(Image<Rgba32> image, PointF point, string firstTeamName, string secondTeamName, Font font, BracketExportAsImageOptions options, int matchHeight, int matchWidth)
    {
        image.Mutate(x => x.DrawText(
            firstTeamName,
            font,
            new Color(Rgba32.ParseHex("#000000FF")),
            point)
        );


        image.Mutate(x => x.Draw(new Color(Rgba32.ParseHex("#000000FF")), 3, new RectangleF(point, rectSize)));
    }

    private void Test()
    {
        //foreach (var (matches, i) in matchesInRounds.Select((matches, i) => (matches, i)))
        //{
        //    foreach (var (match, j) in matches.Select((match, j) => (match, j)))
        //    {
        //        PointF point = new PointF(offSetX + (textSize.Height + options.SpaceBetweenMatches + 2 * options.LinesThickness) * j + ,);

        //        string firstTeamName;
        //        string secondTeamName;

        //        if (i < matches.Count())
        //        {
        //            firstTeamName = matches[i].FirstTeam.Name;
        //            secondTeamName = matches[i].SecondTeam == null ? "" : matches[i].SecondTeam!.Name;
        //        }
        //        else
        //        {
        //            firstTeamName = "";
        //            secondTeamName = "";
        //        }

        //        DrawMatch(image, point, firstTeamName, secondTeamName, options);
        //    }
        //}
    }
}
