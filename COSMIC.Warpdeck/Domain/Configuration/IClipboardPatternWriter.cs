using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IClipboardPatternWriter
    {
        void WritePatterns(List<ClipboardPattern> patterns);
    }
}