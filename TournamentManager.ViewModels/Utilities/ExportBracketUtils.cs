using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TournamentManager.Core.Entities;
using TournamentManager.Core.Enums;
using TournamentManager.ViewModels.Options;

namespace TournamentManager.ViewModels.Utilities;

internal static class ExportBracketUtils
{
    internal static Task<Image<Rgba32>> ExportBracketAsImageAsync(Tournament tournament, ExportBracketAsImageOptions options) => Task.Run(() =>
    {
        switch (tournament.Strategy)
        {
            case StrategyType.Spider:
                return ExportSpiderBracketAsImage(tournament, options);
            case StrategyType.Groups:
                throw new NotImplementedException("Groups bracket export is not implemented yet.");
            default:
                throw new ArgumentOutOfRangeException();
        }
    });

    private static Image<Rgba32> ExportSpiderBracketAsImage(Tournament tournament, ExportBracketAsImageOptions exportOptions)
    {
        int bracketSize = 2;
        int roundsCount = 1;
        while (bracketSize < tournament.Teams.Count)
        {
            bracketSize *= 2;
            roundsCount++;
        }

        int maxTeamNameLength = 0;
        foreach (var match in tournament.Matches)
        {
            if (match.FirstTeam.Name.Length > maxTeamNameLength)
            {
                maxTeamNameLength = match.FirstTeam.Name.Length;
            }
            if (match.SecondTeam != null && match.SecondTeam.Name.Length > maxTeamNameLength)
            {
                maxTeamNameLength = match.SecondTeam.Name.Length;
            }
        }

        BracketImageRenderingOptions internalOptions = SetUpInternalOptions(maxTeamNameLength, roundsCount, tournament.Matches, exportOptions);

        DrawSpiderBracketRoot(internalOptions, exportOptions);

        return internalOptions.Image;
    }

    private static BracketImageRenderingOptions SetUpInternalOptions(int maxTeamNameLength, int totalRounds, IEnumerable<Match> matches, ExportBracketAsImageOptions exportOptions)
    {
        if (!SystemFonts.TryGet(exportOptions.FontName, out FontFamily fontFamily))
        {
            throw new Exception($"Couldn't find font {exportOptions.FontName}");
        }

        Font font = fontFamily.CreateFont(exportOptions.FontSize, FontStyle.Regular);
        TextOptions textOptions = new TextOptions(font);

        string placeholderText = new string('X', maxTeamNameLength + 3);
        FontRectangle nameSize = TextMeasurer.MeasureSize(placeholderText, textOptions);

        double matchHeight = (2 * nameSize.Height + 4 * exportOptions.TextPadding + 3 * exportOptions.LinesThickness);
        double matchWidth = (nameSize.Width + 2 * exportOptions.TextPadding + 2 * exportOptions.LinesThickness);

        double imageHeight;
        if (totalRounds <= 1)
        {
            imageHeight = 2 * exportOptions.ImageHeightOffSet + matchHeight * totalRounds;
        }
        else
        {
            imageHeight = 2 * exportOptions.ImageHeightOffSet + matchHeight * Math.Pow(2, totalRounds - 2) + exportOptions.SpaceBetweenMatches * (Math.Pow(2, totalRounds - 2) - 1);
        }
        double imageWidth = 2 * exportOptions.ImageWidthOffSet + matchWidth * (totalRounds * 2 - 1) + exportOptions.SpaceBetweenRounds * (totalRounds * 2 - 2);

        Image<Rgba32> image = new Image<Rgba32>((int)Math.Ceiling(imageWidth), (int)Math.Ceiling(imageHeight));

        image.Mutate(x => x.BackgroundColor(exportOptions.BackgroundColor));

        double lineLength;
        if (totalRounds < 3)
        {
            lineLength = 0;
        }
        else
        {
            lineLength = (matchHeight + exportOptions.SpaceBetweenMatches) * Math.Pow(2, totalRounds - 3);
        }

        Size rectangleSize = new Size((int)matchWidth, (int)matchHeight);

        BracketImageRenderingOptions internalOptions = new BracketImageRenderingOptions(image, matches, font)
        {
            BoxSize = rectangleSize,
            MatchHeight = (int)matchHeight,
            MatchWidth = (int)matchWidth,
            LineLength = (int)lineLength,
            CurrentRound = totalRounds,
            CurrentMatchNumber = 1
        };

        return internalOptions;
    }

    private static void DrawSpiderBracketRoot(BracketImageRenderingOptions internalOptions, ExportBracketAsImageOptions exportOptions)
    {
        double roundHeightIdentation;
        if (internalOptions.CurrentRound < 3)
        {
            roundHeightIdentation = 0;
        }
        else
        {
            roundHeightIdentation = (internalOptions.MatchHeight + exportOptions.SpaceBetweenMatches) * Math.Pow(2, internalOptions.CurrentRound - 3) - internalOptions.MatchHeight * 0.5 - exportOptions.SpaceBetweenMatches * 0.5;
        }

        double pointX = exportOptions.ImageWidthOffSet + (internalOptions.MatchWidth + exportOptions.SpaceBetweenRounds) * (internalOptions.CurrentRound - 1);
        double pointY = exportOptions.ImageHeightOffSet + roundHeightIdentation;

        Point point = new Point((int)pointX, (int)pointY);

        DrawMatch(point, internalOptions, exportOptions);

        if (internalOptions.CurrentRound > 1)
        {
            point.Y = point.Y + internalOptions.MatchHeight / 2;

            Point leftSide = new Point(point.X - exportOptions.SpaceBetweenRounds, point.Y);
            internalOptions.Image.Mutate(x => x.DrawLine(
                exportOptions.LinesColor,
                exportOptions.LinesThickness,
                point,
                leftSide)
            );

            point.X = point.X + internalOptions.MatchWidth;
            Point rightSide = new Point(point.X + exportOptions.SpaceBetweenRounds, point.Y);
            internalOptions.Image.Mutate(x => x.DrawLine(
                exportOptions.LinesColor,
                exportOptions.LinesThickness,
                point,
                rightSide)
            );

            var leftOptions = (BracketImageRenderingOptions)internalOptions.Clone();
            leftOptions.CurrentRound -= 1;
            leftOptions.CurrentMatchNumber = 1;
            leftOptions.IsRightSide = false;

            var rightOptions = (BracketImageRenderingOptions)internalOptions.Clone();
            rightOptions.CurrentRound -= 1;
            rightOptions.CurrentMatchNumber = 2;
            rightOptions.IsRightSide = true;

            DrawBracketNode(leftSide, leftOptions, exportOptions);
            DrawBracketNode(rightSide, rightOptions, exportOptions);
        }
    }

    private static void DrawBracketNode(Point point, BracketImageRenderingOptions internalOptions, ExportBracketAsImageOptions exportOptions)
    {
        Point endOfBox;
        Point drawMatch;

        if (internalOptions.IsRightSide)
        {
            endOfBox = new Point(point.X + internalOptions.MatchWidth, point.Y);
            drawMatch = new Point(point.X, point.Y - internalOptions.MatchHeight / 2);
        }
        else
        {
            endOfBox = new Point(point.X - internalOptions.MatchWidth, point.Y);
            drawMatch = new Point(point.X - internalOptions.MatchWidth, point.Y - internalOptions.MatchHeight / 2);
        }

        DrawMatch(drawMatch, internalOptions, exportOptions);

        if (internalOptions.CurrentRound > 1)
        {
            DrawConnectingLine(endOfBox, internalOptions, exportOptions);
        }
    }

    private static void DrawConnectingLine(Point point, BracketImageRenderingOptions internalOptions, ExportBracketAsImageOptions exportOptions)
    {
        int moveBy;
        if (internalOptions.IsRightSide)
        {
            moveBy = exportOptions.SpaceBetweenRounds / 2;
        }
        else
        {
            moveBy = -exportOptions.SpaceBetweenRounds / 2;
        }

        Point crossRoads = new Point(point.X + moveBy, point.Y);
        internalOptions.Image.Mutate(x => x.DrawLine(
            exportOptions.LinesColor,
            exportOptions.LinesThickness,
            point,
            crossRoads)
        );

        Point upNode = new Point(crossRoads.X + moveBy, crossRoads.Y - internalOptions.LineLength / 2);
        internalOptions.Image.Mutate(x => x.DrawLine(
            exportOptions.LinesColor,
            exportOptions.LinesThickness,
            crossRoads,
            new Point(crossRoads.X, crossRoads.Y - internalOptions.LineLength / 2),
            upNode)
        );

        Point downNode = new Point(crossRoads.X + moveBy, crossRoads.Y + internalOptions.LineLength / 2);
        internalOptions.Image.Mutate(x => x.DrawLine(
            exportOptions.LinesColor,
            exportOptions.LinesThickness,
            crossRoads,
            new Point(crossRoads.X, crossRoads.Y + internalOptions.LineLength / 2),
            downNode)
        );

        var upOptions = (BracketImageRenderingOptions)internalOptions.Clone();
        upOptions.LineLength = internalOptions.LineLength / 2;
        upOptions.CurrentRound -= 1;
        upOptions.CurrentMatchNumber = internalOptions.CurrentMatchNumber * 2 - 1;

        var downOptions = (BracketImageRenderingOptions)internalOptions.Clone();
        downOptions.LineLength = internalOptions.LineLength / 2;
        downOptions.CurrentRound -= 1;
        downOptions.CurrentMatchNumber = internalOptions.CurrentMatchNumber * 2;

        DrawBracketNode(upNode, upOptions, exportOptions);
        DrawBracketNode(downNode, downOptions, exportOptions);
    }

    private static void DrawMatch(Point point, BracketImageRenderingOptions internalOptions, ExportBracketAsImageOptions exportOptions)
    {
        Match? match = internalOptions.Matches
            .Where(m => m.Round == internalOptions.CurrentRound && m.MatchNumber == internalOptions.CurrentMatchNumber)
            .FirstOrDefault();

        string firstTeamName = "";
        string secondTeamName = "";
        string firstTeamScore = "0";
        string secondTeamScore = "0";
        if (match != null)
        {
            firstTeamName = match.FirstTeam.Name;
            firstTeamScore = match.FirstTeamWins.ToString();

            if (match.SecondTeam != null)
            {
                secondTeamName = match.SecondTeam.Name;
                secondTeamScore = match.SecondTeamWins.ToString();
            }
        }

        internalOptions.Image.Mutate(x => x.Draw(
            exportOptions.LinesColor,
            exportOptions.LinesThickness,
            new Rectangle(point, internalOptions.BoxSize))
        );

        Point linePointStart = new Point(point.X, point.Y + internalOptions.MatchHeight / 2);
        Point linePointEnd = new Point(linePointStart.X + internalOptions.MatchWidth, linePointStart.Y);
        internalOptions.Image.Mutate(x => x.DrawLine(
            exportOptions.LinesColor,
            exportOptions.LinesThickness,
            linePointStart,
            linePointEnd)
        );

        Point textPoint = new Point(point.X + exportOptions.TextPadding, point.Y + exportOptions.TextPadding);
        internalOptions.Image.Mutate(x =>
            x.DrawText(
                firstTeamName,
                internalOptions.Font,
                exportOptions.TextColor,
                textPoint)
            .DrawText(
                firstTeamScore,
                internalOptions.Font,
                exportOptions.TextColor,
                new Point((int)(textPoint.X + internalOptions.MatchWidth - internalOptions.Font.Size - 2 * exportOptions.TextPadding), textPoint.Y))
        );

        textPoint.Y = textPoint.Y + internalOptions.MatchHeight / 2;
        internalOptions.Image.Mutate(x =>
            x.DrawText(
                secondTeamName,
                internalOptions.Font,
                exportOptions.TextColor,
                textPoint)
            .DrawText(
                secondTeamScore,
                internalOptions.Font,
                exportOptions.TextColor,
                new Point((int)(textPoint.X + internalOptions.MatchWidth - internalOptions.Font.Size - 2 * exportOptions.TextPadding), textPoint.Y))
        );
    }
}
