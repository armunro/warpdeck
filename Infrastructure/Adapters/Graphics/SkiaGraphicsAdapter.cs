using OpenMacroBoard.SDK;
using SkiaSharp;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Graphics;

public class SkiaGraphicsAdapter : IGraphicsAdapter, IDisposable
{
    private readonly SKTypeface _typeface;
    private const int Width = 72;
    private const int Height = 72;

    public SkiaGraphicsAdapter()
    {
        _typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal);
    }

    public KeyBitmap DrawCaption(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string text)
    {
        using var surface = SKSurface.Create(new SKImageInfo(Width, Height, SKColorType.Bgra8888));
        var canvas = surface.Canvas;

        // Use the passed background color
        canvas.Clear(new SKColor(r, g, b));

        using var paint = new SKPaint
        {
            Color = new SKColor(fontR, fontG, fontB),
            IsAntialias = true,
            Typeface = _typeface,
            TextSize = 12
        };

        // Text wrapping logic
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            if (paint.MeasureText(testLine) > Width - 4)
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }
        lines.Add(currentLine);

        // Calculate vertical start position to center text
        float lineHeight = paint.FontSpacing;
        float totalHeight = lines.Count * lineHeight;
        float y = (Height - totalHeight) / 2 + paint.TextSize; // TextSize adjustment for baseline

        foreach (var line in lines)
        {
            float x = (Width - paint.MeasureText(line)) / 2;
            canvas.DrawText(line, x, y, paint);
            y += lineHeight;
        }

        using var bitmap = new SKBitmap(Width, Height);
        surface.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0);

        byte[] bgr24 = new byte[Width * Height * 3];
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                var color = bitmap.GetPixel(col, row);
                int index = (row * Width + col) * 3;
                bgr24[index] = color.Blue;
                bgr24[index + 1] = color.Green;
                bgr24[index + 2] = color.Red;
            }
        }

        return KeyBitmap.Create.FromBgr24Array(Width, Height, bgr24);
    }

    public void Dispose()
    {
        _typeface.Dispose();
    }
}
