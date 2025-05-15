using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TournamentManager.ViewModels.Options;

public class ExportBracketAsImageOptions
{
    public int ImageWidthOffSet { get; set; } = 30;
    public int ImageHeightOffSet { get; set; } = 30;
    public int FontSize { get; set; } = 12;
    public string FontName { get; set; } = "Arial";
    public int SpaceBetweenRounds { get; set; } = 30;
    public int SpaceBetweenMatches { get; set; } = 15;
    public int TextPadding { get; set; } = 5;
    public Color TextColor { get; set; } = new Color(Rgba32.ParseHex("#000000FF"));
    public Color LinesColor { get; set; } = new Color(Rgba32.ParseHex("#000000FF"));
    public Color BackgroundColor { get; set; } = new Color(Rgba32.ParseHex("#FFFFFFFF"));
    public int LinesThickness { get; set; } = 3;

    public ExportBracketAsImageOptions()
    {
    }
}
