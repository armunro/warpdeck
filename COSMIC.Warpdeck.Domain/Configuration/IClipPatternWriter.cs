using COSMIC.Warpdeck.Domain.Clipboard;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IClipPatternWriter
    {
        void WritePatterns(List<ClipPattern> patterns);
    }
}