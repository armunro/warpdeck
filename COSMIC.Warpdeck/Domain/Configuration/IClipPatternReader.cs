using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IClipPatternReader
    {
        List<ClipPattern> ReadPatterns();
    }
}