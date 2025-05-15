using SixLabors.ImageSharp;

namespace TournamentManager.ViewModels.Options;

public class ExportBracketAsImageOptionsBuilder
{
    private ExportBracketAsImageOptions _options;

    public ExportBracketAsImageOptionsBuilder()
    {
        _options = new ExportBracketAsImageOptions();
    }

    public void Reset()
    {
        _options = new ExportBracketAsImageOptions();
    }

    public ExportBracketAsImageOptionsBuilder SetFontSize(int fontSize)
    {
        _options.FontSize = fontSize;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetFontName(string fontName)
    {
        _options.FontName = fontName;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetSpaceBetweenRounds(int spaceBetweenRounds)
    {
        _options.SpaceBetweenRounds = spaceBetweenRounds;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetSpaceBetweenMatches(int spaceBetweenMatches)
    {
        _options.SpaceBetweenRounds = spaceBetweenMatches;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetTextPadding(int textPadding)
    {
        _options.TextPadding = textPadding;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetTextColor(Color textColor)
    {
        _options.TextColor = textColor;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetLinesColor(Color linesColor)
    {
        _options.LinesColor = linesColor;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetBackgroundColor(Color backgroundColor)
    {
        _options.BackgroundColor = backgroundColor;
        return this;
    }

    public ExportBracketAsImageOptionsBuilder SetLinesThickness(int linesThickness)
    {
        _options.LinesThickness = linesThickness;
        return this;
    }

    public ExportBracketAsImageOptions Build()
    {
        var result = _options;

        Reset();

        return result;
    }
}
