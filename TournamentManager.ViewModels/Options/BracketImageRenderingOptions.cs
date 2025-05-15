using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TournamentManager.Core.Entities;

namespace TournamentManager.ViewModels.Options;

internal class BracketImageRenderingOptions : ICloneable
{
    public Image<Rgba32> Image { get; set; }
    public Font Font { get; set; }
    public IEnumerable<Match> Matches { get; set; }
    public Size BoxSize { get; set; }
    public int MatchHeight { get; set; }
    public int MatchWidth { get; set; }
    public int LineLength { get; set; }
    public bool IsRightSide { get; set; }
    public int CurrentRound { get; set; }
    public int CurrentMatchNumber { get; set; }

    private BracketImageRenderingOptions(BracketImageRenderingOptions clone)
    {
        Image = clone.Image;
        Font = clone.Font;
        Matches = clone.Matches;
        BoxSize = clone.BoxSize;
        MatchHeight = clone.MatchHeight;
        MatchWidth = clone.MatchWidth;
        LineLength = clone.LineLength;
        IsRightSide = clone.IsRightSide;
        CurrentRound = clone.CurrentRound;
        CurrentMatchNumber = clone.CurrentMatchNumber;
    }

    internal BracketImageRenderingOptions(Image<Rgba32> image, IEnumerable<Match> matches, Font font)
    {
        Image = image;
        Matches = matches;
        Font = font;
    }

    public object Clone()
    {
        return new BracketImageRenderingOptions(this);
    }
}
