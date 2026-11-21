using OpenMacroBoard.SDK;

namespace Warpdeck.Core.Domain;

public interface IGraphicsAdapter
{
    KeyBitmap DrawCaption(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string text);
    KeyBitmap DrawKey(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath = null, string? theme = null, bool isPressed = false, string? font = null);
    string? IconRoot { get; set; }
    void SetIconRoot(string? iconRoot);
    int KeyWidth { get; }
    int KeyHeight { get; }
    void SetKeySize(int width, int height);
}
