namespace TournamentManager.Core.Options;

public class BracketExportAsImageOptionsBuilder
{
    private BracketExportAsImageOptions _options;

    public BracketExportAsImageOptionsBuilder()
    {
        _options = new BracketExportAsImageOptions();
    }

    public void Reset()
    {
        _options = new BracketExportAsImageOptions();
    }

    public BracketExportAsImageOptionsBuilder SetFontSize(int fontSize)
    {
        _options.FontSize = fontSize;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetFontName(string fontName)
    {
        _options.FontName = fontName;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetSpaceBetweenRounds(int spaceBetweenRounds)
    {
        _options.SpaceBetweenRounds = spaceBetweenRounds;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetSpaceBetweenMatches(int spaceBetweenMatches)
    {
        _options.SpaceBetweenRounds = spaceBetweenMatches;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetTextPadding(int textPadding)
    {
        _options.TextPadding = textPadding;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetTextColor(int textColor)
    {
        _options.TextColor = textColor;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetLinesColor(int linesColor)
    {
        _options.LinesColor = linesColor;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetBackgroundColor(int backgroundColor)
    {
        _options.BackgroundColor = backgroundColor;
        return this;
    }

    public BracketExportAsImageOptionsBuilder SetLinesThickness(int linesThickness)
    {
        _options.LinesThickness = linesThickness;
        return this;
    }

    public BracketExportAsImageOptions Build()
    {
        var result = _options;

        Reset();

        return result;
    }
}
