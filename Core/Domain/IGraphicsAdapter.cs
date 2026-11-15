using OpenMacroBoard.SDK;

namespace Warpdeck.Core.Domain;

public interface IGraphicsAdapter
{
    KeyBitmap DrawCaption(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string text);
}
