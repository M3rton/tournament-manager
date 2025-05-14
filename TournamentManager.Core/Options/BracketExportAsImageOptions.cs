namespace TournamentManager.Core.Options;

public class BracketExportAsImageOptions
{
    public int FontSize { get; set; } = 12;
    public string FontName { get; set; } = "Arial";
    public int SpaceBetweenRounds { get; set; } = 20;
    public int SpaceBetweenMatches { get; set; } = 10;
    public int TextPadding { get; set; } = 5;
    public int TextColor { get; set; } = 0x000000;
    public int LinesColor { get; set; } = 0x000000;
    public int BackgroundColor { get; set; } = 0xFFFFFF;
    public int LinesThickness { get; set; } = 3;

    public BracketExportAsImageOptions()
    {
    }
}
