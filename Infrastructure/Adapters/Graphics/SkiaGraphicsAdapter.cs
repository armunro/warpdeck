using System.Collections.Concurrent;
using System.IO;
using OpenMacroBoard.SDK;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Graphics;

public class SkiaGraphicsAdapter : IGraphicsAdapter, IDisposable
{
    private readonly SKTypeface _typeface;
    private int _width = 96;
    private int _height = 96;
    private readonly ConcurrentDictionary<string, SKPicture?> _pictureCache = new(StringComparer.OrdinalIgnoreCase);
    private string? _iconRoot;

    public int KeyWidth => _width;
    public int KeyHeight => _height;

    public void SetKeySize(int width, int height)
    {
        _width = Math.Clamp(width, 32, 512);
        _height = Math.Clamp(height, 32, 512);
    }

    public string? IconRoot
    {
        get => _iconRoot;
        set => SetIconRoot(value);
    }

    public void SetIconRoot(string? iconRoot)
    {
        if (!string.Equals(_iconRoot, iconRoot, StringComparison.OrdinalIgnoreCase))
        {
            _iconRoot = iconRoot;
            foreach (var picture in _pictureCache.Values)
            {
                picture?.Dispose();
            }
            _pictureCache.Clear();
        }
    }

    private static readonly string[] SubDirs = new[]
    {
        "solid",
        "brands",
        "regular",
        "duotone",
        "light",
        "thin",
        "sharp-solid",
        "sharp-regular",
        "sharp-light",
        "sharp-thin"
    };

    public SkiaGraphicsAdapter()
    {
        _typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal);
    }

    public KeyBitmap DrawCaption(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string text)
    {
        return DrawKey(r, g, b, fontR, fontG, fontB, text, null, null, false, null);
    }

    public KeyBitmap DrawKey(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath = null, string? theme = null, bool isPressed = false, string? font = null)
    {
        int targetWidth = _width;
        int targetHeight = _height;

        const int renderScale = 2;
        int renderWidth = targetWidth * renderScale;
        int renderHeight = targetHeight * renderScale;

        using var highResSurface = SKSurface.Create(new SKImageInfo(renderWidth, renderHeight, SKColorType.Bgra8888, SKAlphaType.Premul));
        var canvas = highResSurface.Canvas;
        canvas.Scale(renderScale);

        if (string.Equals(theme, "korry", StringComparison.OrdinalIgnoreCase))
        {
            DrawKorryTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed, font);
        }
        else if (string.Equals(theme, "cyber", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "neon", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "hud", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "scifi", StringComparison.OrdinalIgnoreCase))
        {
            DrawCyberTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed, font);
        }
        else if (string.Equals(theme, "synthwave", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "retro", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "outrun", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "synth", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "arcade", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "80s", StringComparison.OrdinalIgnoreCase))
        {
            DrawSynthwaveTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed, font);
        }
        else
        {
            DrawDefaultTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed, font);
        }

        using var highResImage = highResSurface.Snapshot();
        
        using var finalSurface = SKSurface.Create(new SKImageInfo(targetWidth, targetHeight, SKColorType.Bgra8888, SKAlphaType.Premul));
        using var filterPaint = new SKPaint
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        finalSurface.Canvas.DrawImage(highResImage, new SKRect(0, 0, targetWidth, targetHeight), filterPaint);

        using var bitmap = new SKBitmap(targetWidth, targetHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        finalSurface.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0);

        byte[] bgr24 = new byte[targetWidth * targetHeight * 3];
        for (int row = 0; row < targetHeight; row++)
        {
            for (int col = 0; col < targetWidth; col++)
            {
                var color = bitmap.GetPixel(col, row);
                int index = (row * targetWidth + col) * 3;
                bgr24[index] = color.Blue;
                bgr24[index + 1] = color.Green;
                bgr24[index + 2] = color.Red;
            }
        }

        return KeyBitmap.Create.FromBgr24Array(targetWidth, targetHeight, bgr24);
    }

    private SKTypeface GetTypeface(string? fontName, SKFontStyleWeight defaultWeight = SKFontStyleWeight.Bold)
    {
        if (!string.IsNullOrWhiteSpace(fontName) && !string.Equals(fontName, "(Default)", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var tf = SKTypeface.FromFamilyName(fontName, defaultWeight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
                if (tf != null && tf.FamilyName.Length > 0) return tf;
            }
            catch
            {
                // fallback
            }
        }
        return _typeface;
    }

    private void DrawDefaultTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed, string? font = null)
    {
        // Professional Pro-Deck / Dark Glassmorphic Modern UI Theme:
        // - Dark sleek graphite/slate background with subtle vertical gradient and specular top sheen
        // - Category/accent color used as a stylish accent glow, subtle top/bottom accent bar or perimeter border, and tinted icon/caption
        // - High readability, clean professional typography with subtle shadow drop
        // - Tactile mechanical depression effect when pressed

        canvas.Clear(SKColors.Black);

        float s = _width / 72f;
        float cornerRadius = 10f * s;
        
        SKRect buttonRect;
        float yOffset = isPressed ? 2f * s : 0f;
        float iconScaleFactor = isPressed ? 0.92f : 1.0f;

        if (isPressed)
        {
            buttonRect = new SKRect(2f * s, 3f * s, _width - 2f * s, _height - 1f * s);
        }
        else
        {
            buttonRect = new SKRect(0.5f, 0.5f, _width - 0.5f, _height - 0.5f);
        }

        // Determine accent color
        bool hasAccent = (r > 30 || g > 30 || b > 30);
        var accentColor = hasAccent ? new SKColor(r, g, b) : new SKColor(fontR, fontG, fontB);
        if (accentColor.Red < 40 && accentColor.Green < 40 && accentColor.Blue < 40)
        {
            accentColor = new SKColor(220, 225, 230);
        }

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(buttonRect, cornerRadius, cornerRadius), SKClipOperation.Intersect, antialias: true);

        // 1. Sleek dark gradient background - deep pitch obsidian/charcoal
        byte baseTop = isPressed ? (byte)8 : (byte)14;
        byte baseBottom = isPressed ? (byte)3 : (byte)6;
        
        SKColor topGradColor = new SKColor(
            (byte)Math.Clamp(baseTop + (accentColor.Red * 0.035f), 0, 255),
            (byte)Math.Clamp(baseTop + (accentColor.Green * 0.035f), 0, 255),
            (byte)Math.Clamp(baseTop + (accentColor.Blue * 0.035f), 0, 255)
        );
        SKColor bottomGradColor = new SKColor(
            (byte)Math.Clamp(baseBottom + (accentColor.Red * 0.01f), 0, 255),
            (byte)Math.Clamp(baseBottom + (accentColor.Green * 0.01f), 0, 255),
            (byte)Math.Clamp(baseBottom + (accentColor.Blue * 0.01f), 0, 255)
        );

        using (var bgShader = SKShader.CreateLinearGradient(
            new SKPoint(0, buttonRect.Top),
            new SKPoint(0, buttonRect.Bottom),
            new[] { topGradColor, bottomGradColor },
            null,
            SKShaderTileMode.Clamp))
        using (var bgPaint = new SKPaint
        {
            Shader = bgShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, bgPaint);
        }

        // 2. Subtle radial ambient glow matching accent color
        using (var glowShader = SKShader.CreateRadialGradient(
            new SKPoint(buttonRect.MidX, buttonRect.Top + (buttonRect.Height * 0.35f)),
            buttonRect.Width * 0.65f,
            new[] { accentColor.WithAlpha(isPressed ? (byte)25 : (byte)15), accentColor.WithAlpha(0) },
            null,
            SKShaderTileMode.Clamp))
        using (var glowPaint = new SKPaint
        {
            Shader = glowShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, glowPaint);
        }

        // 3. Subtle top accent highlight bar
        if (hasAccent)
        {
            using var barPaint = new SKPaint
            {
                Color = accentColor.WithAlpha(isPressed ? (byte)220 : (byte)180),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(new SKRect(buttonRect.Left + 8f * s, buttonRect.Top + 2.5f * s, buttonRect.Right - 8f * s, buttonRect.Top + 5f * s), 1.5f * s, 1.5f * s, barPaint);
        }

        // 4. Subtle top glass specular reflection / bevel
        using (var sheenShader = SKShader.CreateLinearGradient(
            new SKPoint(0, buttonRect.Top),
            new SKPoint(0, buttonRect.Top + 14f * s),
            new[] { SKColors.White.WithAlpha(isPressed ? (byte)8 : (byte)18), SKColors.White.WithAlpha(0) },
            null,
            SKShaderTileMode.Clamp))
        using (var sheenPaint = new SKPaint
        {
            Shader = sheenShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(new SKRect(buttonRect.Left, buttonRect.Top, buttonRect.Right, buttonRect.Top + 14f * s), sheenPaint);
        }

        // If pressed, draw top inner shadow for depth inside the key well
        if (isPressed)
        {
            using var shadowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 140),
                IsAntialias = true
            };
            canvas.DrawRect(new SKRect(buttonRect.Left, buttonRect.Top, buttonRect.Right, buttonRect.Top + 6f * s), shadowPaint);
        }

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        SKColor textColor = (fontR == 0 && fontG == 0 && fontB == 0) 
            ? new SKColor(240, 242, 245) 
            : new SKColor(fontR, fontG, fontB);

        SKColor iconColor = hasAccent ? accentColor : textColor;

        if (hasPicture && hasText)
        {
            float iconSize = 28f * s * iconScaleFactor;
            float iconTop = 8f * s + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, iconColor.Red, iconColor.Green, iconColor.Blue);

            using var fontTypeface = GetTypeface(font, SKFontStyleWeight.Bold);
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 12f * s,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 160),
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 12f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, _width - (6f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = (36f * s + yOffset) + (32f * s - totalHeight) / 2f + textPaint.TextSize;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, textStartY + (1f * s), shadowPaint);
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            float iconSize = 42f * s * iconScaleFactor;
            float iconTop = (_height - iconSize) / 2f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, iconColor.Red, iconColor.Green, iconColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            using var fontTypeface = GetTypeface(font, SKFontStyleWeight.Bold);
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 14f * s,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 160),
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 14f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, _width - (6f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (_height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y + (1f * s), shadowPaint);
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        // 5. Outer subtle border / bevel stroke for sleek hardware finish
        using var borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f * s,
            Color = hasAccent 
                ? accentColor.WithAlpha(isPressed ? (byte)100 : (byte)50) 
                : new SKColor(255, 255, 255, isPressed ? (byte)40 : (byte)25),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(buttonRect.Left + 0.5f, buttonRect.Top + 0.5f, buttonRect.Right - 0.5f, buttonRect.Bottom - 0.5f), cornerRadius, cornerRadius, borderPaint);

        canvas.Restore();
    }

    private void DrawKorryTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed, string? font = null)
    {
        // KORRY 389 / aviation switch style:
        canvas.Clear(SKColors.Black);

        float s = _width / 72f;
        float outerCornerRadius = 10f * s;
        float innerCornerRadius = 8f * s;
        float litBorderCornerRadius = 7.5f * s;

        // Outer Bezel / Housing
        using var bezelPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f * s,
            Color = isPressed ? new SKColor(20, 22, 26) : new SKColor(28, 30, 34),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(1f * s, 1f * s, _width - (1f * s), _height - (1f * s)), outerCornerRadius, outerCornerRadius, bezelPaint);

        // Inner recessed button face - deep matte cockpit black
        var innerRect = isPressed 
            ? new SKRect(4f * s, 5.5f * s, _width - (4f * s), _height - (2.5f * s)) 
            : new SKRect(3f * s, 3f * s, _width - (3f * s), _height - (3f * s));

        using var innerFacePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = isPressed ? new SKColor(2, 2, 3) : new SKColor(5, 5, 6),
            IsAntialias = true
        };
        canvas.DrawRoundRect(innerRect, innerCornerRadius, innerCornerRadius, innerFacePaint);

        // If pressed, draw dark top shadow inside bezel to simulate deep mechanical recess
        if (isPressed)
        {
            using var recessShadowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 220),
                IsAntialias = true
            };
            canvas.DrawRect(new SKRect(2f * s, 2f * s, _width - (2f * s), 6f * s), recessShadowPaint);
        }

        bool hasAccent = (r > 30 || g > 30 || b > 30);
        SKColor litColor;
        if (hasAccent)
        {
            litColor = new SKColor(r, g, b);
        }
        else if (fontR > 40 || fontG > 40 || fontB > 40)
        {
            litColor = new SKColor(fontR, fontG, fontB);
        }
        else
        {
            litColor = new SKColor(255, 255, 255);
        }

        float yOffset = isPressed ? 2f * s : 0f;
        float iconScaleFactor = isPressed ? 0.90f : 1.0f;
        float dividerY = (_height / 2f) + yOffset;

        // Korry horizontal divider line (classic split legend) - darker muted tint
        SKColor dividerColor = isPressed
            ? new SKColor((byte)(litColor.Red * 0.5f), (byte)(litColor.Green * 0.5f), (byte)(litColor.Blue * 0.5f))
            : new SKColor((byte)(litColor.Red * 0.3f), (byte)(litColor.Green * 0.3f), (byte)(litColor.Blue * 0.3f));

        using var dividerPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 1.5f * s : 1.2f * s,
            Color = dividerColor,
            IsAntialias = true
        };
        canvas.DrawLine(innerRect.Left + (1f * s), dividerY, innerRect.Right - (1f * s), dividerY, dividerPaint);

        // Top and bottom annunciator cell backgrounds - subtle backlighting
        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(innerRect, innerCornerRadius, innerCornerRadius), SKClipOperation.Intersect, antialias: true);

        byte cellAlpha = isPressed ? (byte)20 : (byte)8;
        using var cellGlowPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = litColor.WithAlpha(cellAlpha),
            IsAntialias = true
        };
        canvas.DrawRect(new SKRect(innerRect.Left, innerRect.Top, innerRect.Right, dividerY - (1f * s)), cellGlowPaint);
        canvas.DrawRect(new SKRect(innerRect.Left, dividerY + (1f * s), innerRect.Right, innerRect.Bottom), cellGlowPaint);

        if (isPressed)
        {
            using var topLensShadow = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 110),
                IsAntialias = true
            };
            canvas.DrawRect(new SKRect(innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Top + (4f * s)), topLensShadow);
        }

        canvas.Restore();

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        if (hasPicture && hasText)
        {
            float iconSize = 24f * s * iconScaleFactor;
            float iconTop = 6f * s + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, litColor.Red, litColor.Green, litColor.Blue);

            using var fontTypeface = GetTypeface(font, SKFontStyleWeight.Bold);
            using var textPaint = new SKPaint
            {
                Color = litColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 11.5f * s
            };

            string korryText = text!.ToUpperInvariant();
            var lines = WrapText(korryText, textPaint, _width - (8f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = dividerY + ((innerRect.Bottom - dividerY) - totalHeight) / 2f + textPaint.TextSize - (1f * s);

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            float iconSize = 36f * s * iconScaleFactor;
            float iconTop = (_height - iconSize) / 2f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, litColor.Red, litColor.Green, litColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            using var fontTypeface = GetTypeface(font, SKFontStyleWeight.Bold);
            using var textPaint = new SKPaint
            {
                Color = litColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 13f * s
            };

            string korryText = text!.ToUpperInvariant();
            var lines = WrapText(korryText, textPaint, _width - (8f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (_height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        // Inner glowing border indicator highlight
        using var litBorderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 1.5f * s : 1f * s,
            Color = litColor.WithAlpha(isPressed ? (byte)120 : (byte)70),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(innerRect.Left + 0.5f, innerRect.Top + 0.5f, innerRect.Right - 0.5f, innerRect.Bottom - 0.5f), litBorderCornerRadius, litBorderCornerRadius, litBorderPaint);
    }

    private void DrawCyberTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed, string? font = null)
    {
        // Cyberpunk / Sci-Fi Tactical HUD Theme:
        canvas.Clear(SKColors.Black);

        float s = _width / 72f;
        float cornerRadius = 8f * s;
        float yOffset = isPressed ? 1.5f * s : 0f;
        float iconScaleFactor = isPressed ? 0.94f : 1.0f;

        SKRect buttonRect = isPressed
            ? new SKRect(2f * s, 2.5f * s, _width - (2f * s), _height - (0.5f * s))
            : new SKRect(0.5f, 0.5f, _width - 0.5f, _height - 0.5f);

        bool hasAccent = (r > 30 || g > 30 || b > 30);
        var accentColor = hasAccent ? new SKColor(r, g, b) : new SKColor(fontR, fontG, fontB);
        if (accentColor.Red < 40 && accentColor.Green < 40 && accentColor.Blue < 40)
        {
            accentColor = new SKColor(0, 229, 255); // Default to Electric Cyan
        }

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(buttonRect, cornerRadius, cornerRadius), SKClipOperation.Intersect, antialias: true);

        // 1. Tech Obsidian Background - deep pitch tech surface
        byte bgTop = isPressed ? (byte)6 : (byte)8;
        byte bgBottom = isPressed ? (byte)1 : (byte)3;
        SKColor topGrad = new SKColor(
            (byte)Math.Clamp(bgTop + (accentColor.Red * 0.02f), 0, 255),
            (byte)Math.Clamp(bgTop + (accentColor.Green * 0.02f), 0, 255),
            (byte)Math.Clamp(bgTop + (accentColor.Blue * 0.02f), 0, 255)
        );
        SKColor bottomGrad = new SKColor(
            (byte)Math.Clamp(bgBottom + (accentColor.Red * 0.008f), 0, 255),
            (byte)Math.Clamp(bgBottom + (accentColor.Green * 0.008f), 0, 255),
            (byte)Math.Clamp(bgBottom + (accentColor.Blue * 0.008f), 0, 255)
        );

        using (var bgShader = SKShader.CreateLinearGradient(
            new SKPoint(0, buttonRect.Top),
            new SKPoint(0, buttonRect.Bottom),
            new[] { topGrad, bottomGrad },
            null,
            SKShaderTileMode.Clamp))
        using (var bgPaint = new SKPaint
        {
            Shader = bgShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, bgPaint);
        }

        // 2. High-tech scanlines - subtle low-intensity grid
        using (var scanlinePaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 6),
            StrokeWidth = 0.5f * s,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke
        })
        {
            for (float y = buttonRect.Top + (2f * s); y < buttonRect.Bottom - (1f * s); y += (3f * s))
            {
                canvas.DrawLine(buttonRect.Left, y, buttonRect.Right, y, scanlinePaint);
            }
        }

        // 3. Cyber Neon Ambient Bloom
        using (var glowShader = SKShader.CreateRadialGradient(
            new SKPoint(buttonRect.MidX, buttonRect.MidY),
            buttonRect.Width * 0.65f,
            new[] { accentColor.WithAlpha(isPressed ? (byte)45 : (byte)22), accentColor.WithAlpha(0) },
            null,
            SKShaderTileMode.Clamp))
        using (var glowPaint = new SKPaint
        {
            Shader = glowShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, glowPaint);
        }

        // 4. Subtle Outer Cyber Grid / Border Frame
        using (var framePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f * s,
            Color = accentColor.WithAlpha(isPressed ? (byte)160 : (byte)45),
            IsAntialias = true
        })
        {
            canvas.DrawRoundRect(new SKRect(buttonRect.Left + 0.5f, buttonRect.Top + 0.5f, buttonRect.Right - 0.5f, buttonRect.Bottom - 0.5f), cornerRadius, cornerRadius, framePaint);
        }

        // 5. Tactical HUD Corner Brackets
        float pad = 3f * s;
        float bLeft = buttonRect.Left + pad;
        float bTop = buttonRect.Top + pad;
        float bRight = buttonRect.Right - pad;
        float bBottom = buttonRect.Bottom - pad;
        float bracketLen = 8f * s;

        using (var bracketPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 2.0f * s : 1.5f * s,
            Color = accentColor.WithAlpha(isPressed ? (byte)255 : (byte)230),
            StrokeCap = SKStrokeCap.Square,
            IsAntialias = true
        })
        {
            // Top-Left bracket
            canvas.DrawLine(bLeft, bTop, bLeft + bracketLen, bTop, bracketPaint);
            canvas.DrawLine(bLeft, bTop, bLeft, bTop + bracketLen, bracketPaint);

            // Top-Right bracket
            canvas.DrawLine(bRight, bTop, bRight - bracketLen, bTop, bracketPaint);
            canvas.DrawLine(bRight, bTop, bRight, bTop + bracketLen, bracketPaint);

            // Bottom-Left bracket
            canvas.DrawLine(bLeft, bBottom, bLeft + bracketLen, bBottom, bracketPaint);
            canvas.DrawLine(bLeft, bBottom, bLeft, bBottom - bracketLen, bracketPaint);

            // Bottom-Right bracket
            canvas.DrawLine(bRight, bBottom, bRight - bracketLen, bBottom, bracketPaint);
            canvas.DrawLine(bRight, bBottom, bRight, bBottom - bracketLen, bracketPaint);
        }

        // 6. Mini Tactical Status Ticks
        using (var ledPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accentColor.WithAlpha(isPressed ? (byte)255 : (byte)180),
            IsAntialias = true
        })
        {
            float dotY = bTop + (2.5f * s);
            float dotStartX = bRight - bracketLen - (12f * s);
            for (int i = 0; i < 3; i++)
            {
                canvas.DrawRect(new SKRect(dotStartX + i * (4.5f * s), dotY, dotStartX + i * (4.5f * s) + (2f * s), dotY + (2f * s)), ledPaint);
            }
        }

        // 7. Mini Tech Slash Marks in bottom left
        using (var tickPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f * s,
            Color = accentColor.WithAlpha(isPressed ? (byte)200 : (byte)120),
            IsAntialias = true
        })
        {
            float slashX = bLeft + bracketLen + (2f * s);
            float slashY = bBottom - (2f * s);
            canvas.DrawLine(slashX, slashY, slashX + (2f * s), slashY - (4f * s), tickPaint);
            canvas.DrawLine(slashX + (3.5f * s), slashY, slashX + (5.5f * s), slashY - (4f * s), tickPaint);
        }

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        using var fontTypeface = !string.IsNullOrWhiteSpace(font) && !string.Equals(font, "(Default)", StringComparison.OrdinalIgnoreCase) 
            ? GetTypeface(font, SKFontStyleWeight.Bold) 
            : (SKTypeface.FromFamilyName("Consolas", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright) ?? _typeface);
        
        SKColor textColor = new SKColor(245, 250, 255);
        SKColor glowColor = accentColor.WithAlpha(180);

        if (hasPicture && hasText)
        {
            float iconSize = 27f * s * iconScaleFactor;
            float iconTop = 8.5f * s + yOffset;

            // Draw glowing neon icon background bloom
            DrawPictureScaledAndTinted(canvas, picture!, iconSize + (2f * s), iconTop - (1f * s), (byte)(accentColor.Red / 2), (byte)(accentColor.Green / 2), (byte)(accentColor.Blue / 2));
            // Draw crisp neon icon
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, accentColor.Red, accentColor.Green, accentColor.Blue);

            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 11.5f * s,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = glowColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 11.5f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!.ToUpperInvariant(), textPaint, _width - (10f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = (36f * s + yOffset) + (31f * s - totalHeight) / 2f + textPaint.TextSize;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, textStartY + (1f * s), shadowPaint);
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            float iconSize = 42f * s * iconScaleFactor;
            float iconTop = (_height - iconSize) / 2f + yOffset;

            DrawPictureScaledAndTinted(canvas, picture!, iconSize + (2f * s), iconTop - (1f * s), (byte)(accentColor.Red / 2), (byte)(accentColor.Green / 2), (byte)(accentColor.Blue / 2));
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, accentColor.Red, accentColor.Green, accentColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 13f * s,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = glowColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 13f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!.ToUpperInvariant(), textPaint, _width - (10f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (_height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y, shadowPaint);
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        canvas.Restore();
    }

    private void DrawSynthwaveTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed, string? font = null)
    {
        // 80s Synthwave / Outrun Retro-Futuristic Aesthetic:
        // - Deep midnight cosmic violet background
        // - Glowing retro sunset gradient aura and perspective wireframe horizon gridlines
        // - Duo-tone neon frame (Neon Magenta/Pink to Electric Cyan) with ambient neon bloom
        // - High-contrast glowing typography and dual-tone backlit vector icons
        // - Kinetic laser overdrive response when pressed

        canvas.Clear(SKColors.Black);

        float s = _width / 72f;
        float cornerRadius = 9f * s;
        float yOffset = isPressed ? 1.5f * s : 0f;
        float iconScaleFactor = isPressed ? 0.93f : 1.0f;

        SKRect buttonRect = isPressed
            ? new SKRect(2f * s, 2.5f * s, _width - (2f * s), _height - (0.5f * s))
            : new SKRect(0.5f, 0.5f, _width - 0.5f, _height - 0.5f);

        bool hasAccent = (r > 30 || g > 30 || b > 30);
        var accentColor = hasAccent ? new SKColor(r, g, b) : new SKColor(fontR, fontG, fontB);
        if (accentColor.Red < 40 && accentColor.Green < 40 && accentColor.Blue < 40)
        {
            accentColor = new SKColor(255, 0, 127); // Default to Synthwave Neon Pink / Magenta (#FF007F)
        }

        SKColor neonPink = new SKColor(255, 0, 127); // #FF007F
        SKColor neonCyan = new SKColor(0, 229, 255); // #00E5FF
        SKColor neonViolet = new SKColor(121, 40, 202); // #7928CA

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(buttonRect, cornerRadius, cornerRadius), SKClipOperation.Intersect, antialias: true);

        // 1. Cosmic Midnight Violet Base
        byte bgTop = isPressed ? (byte)12 : (byte)16;
        byte bgBottom = isPressed ? (byte)3 : (byte)5;
        SKColor topGrad = new SKColor(
            (byte)Math.Clamp(bgTop + (accentColor.Red * 0.04f), 0, 255),
            (byte)Math.Clamp((bgTop / 2) + (accentColor.Green * 0.02f), 0, 255),
            (byte)Math.Clamp(bgTop * 2 + (accentColor.Blue * 0.05f), 0, 255)
        );
        SKColor bottomGrad = new SKColor(
            (byte)Math.Clamp(bgBottom + (accentColor.Red * 0.01f), 0, 255),
            (byte)Math.Clamp(bgBottom + (accentColor.Green * 0.01f), 0, 255),
            (byte)Math.Clamp(bgBottom + 6 + (accentColor.Blue * 0.02f), 0, 255)
        );

        using (var bgShader = SKShader.CreateLinearGradient(
            new SKPoint(0, buttonRect.Top),
            new SKPoint(0, buttonRect.Bottom),
            new[] { topGrad, bottomGrad },
            null,
            SKShaderTileMode.Clamp))
        using (var bgPaint = new SKPaint
        {
            Shader = bgShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, bgPaint);
        }

        // 2. Synthwave Sunset Horizon Glow (Upper / Center)
        using (var sunShader = SKShader.CreateRadialGradient(
            new SKPoint(buttonRect.MidX, buttonRect.Top + buttonRect.Height * 0.25f),
            buttonRect.Width * 0.65f,
            new[] { neonPink.WithAlpha(isPressed ? (byte)75 : (byte)45), neonViolet.WithAlpha(isPressed ? (byte)30 : (byte)15), SKColors.Transparent },
            new[] { 0f, 0.6f, 1f },
            SKShaderTileMode.Clamp))
        using (var sunPaint = new SKPaint
        {
            Shader = sunShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(buttonRect, sunPaint);
        }

        // 3. 80s 3D Perspective Ground Grid (Bottom portion)
        float horizonY = buttonRect.Top + buttonRect.Height * 0.52f;
        using (var gridPaint = new SKPaint
        {
            Color = neonCyan.WithAlpha(isPressed ? (byte)80 : (byte)40),
            StrokeWidth = 0.8f * s,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        })
        {
            // Horizontal perspective lines (spaced further apart towards the bottom)
            float[] gridOffsets = { 3f * s, 8f * s, 15f * s, 24f * s, 34f * s };
            foreach (var off in gridOffsets)
            {
                float y = horizonY + off;
                if (y < buttonRect.Bottom)
                {
                    canvas.DrawLine(buttonRect.Left, y, buttonRect.Right, y, gridPaint);
                }
            }

            // Radiating vertical perspective lines
            float centerX = buttonRect.MidX;
            float[] spreadMultipliers = { -2.2f, -1.3f, -0.5f, 0.5f, 1.3f, 2.2f };
            foreach (var mult in spreadMultipliers)
            {
                float startX = centerX + (mult * 3f * s);
                float endX = centerX + (mult * 20f * s);
                canvas.DrawLine(startX, horizonY, endX, buttonRect.Bottom, gridPaint);
            }
        }

        // Horizon Neon Laser Line
        using (var horizonPaint = new SKPaint
        {
            Color = neonPink.WithAlpha(isPressed ? (byte)220 : (byte)130),
            StrokeWidth = 1.2f * s,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        })
        {
            canvas.DrawLine(buttonRect.Left, horizonY, buttonRect.Right, horizonY, horizonPaint);
        }

        // 4. Duo-Tone Neon Bevel & Border Frame
        SKColor frameColorA = hasAccent ? accentColor : neonPink;
        SKColor frameColorB = neonCyan;
        using (var borderShader = SKShader.CreateLinearGradient(
            new SKPoint(buttonRect.Left, buttonRect.Top),
            new SKPoint(buttonRect.Right, buttonRect.Bottom),
            new[] { frameColorA.WithAlpha(isPressed ? (byte)240 : (byte)160), frameColorB.WithAlpha(isPressed ? (byte)200 : (byte)120) },
            null,
            SKShaderTileMode.Clamp))
        using (var borderPaint = new SKPaint
        {
            Shader = borderShader,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 1.5f * s : 1.2f * s,
            IsAntialias = true
        })
        {
            canvas.DrawRoundRect(new SKRect(buttonRect.Left + 0.6f, buttonRect.Top + 0.6f, buttonRect.Right - 0.6f, buttonRect.Bottom - 0.6f), cornerRadius, cornerRadius, borderPaint);
        }

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        using var fontTypeface = !string.IsNullOrWhiteSpace(font) && !string.Equals(font, "(Default)", StringComparison.OrdinalIgnoreCase)
            ? GetTypeface(font, SKFontStyleWeight.Bold)
            : (SKTypeface.FromFamilyName("Trebuchet MS", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright) ?? _typeface);

        SKColor primaryLitColor = hasAccent ? accentColor : neonCyan;
        SKColor secondaryGlowColor = neonPink.WithAlpha(190);
        SKColor textColor = new SKColor(255, 250, 255);

        if (hasPicture && hasText)
        {
            float iconSize = 27f * s * iconScaleFactor;
            float iconTop = 8.5f * s + yOffset;

            // Multi-pass neon bloom for icon
            DrawPictureScaledAndTinted(canvas, picture!, iconSize + (3f * s), iconTop - (1.5f * s), neonPink.Red, neonPink.Green, neonPink.Blue);
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, primaryLitColor.Red, primaryLitColor.Green, primaryLitColor.Blue);

            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 11.5f * s,
                FakeBoldText = true
            };

            using var glowTextPaint = new SKPaint
            {
                Color = secondaryGlowColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 11.5f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, _width - (10f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = (36f * s + yOffset) + (31f * s - totalHeight) / 2f + textPaint.TextSize;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, textStartY + (1.2f * s), glowTextPaint);
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            float iconSize = 42f * s * iconScaleFactor;
            float iconTop = (_height - iconSize) / 2f + yOffset;

            DrawPictureScaledAndTinted(canvas, picture!, iconSize + (3f * s), iconTop - (1.5f * s), neonPink.Red, neonPink.Green, neonPink.Blue);
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, primaryLitColor.Red, primaryLitColor.Green, primaryLitColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 13.5f * s,
                FakeBoldText = true
            };

            using var glowTextPaint = new SKPaint
            {
                Color = secondaryGlowColor,
                IsAntialias = true,
                SubpixelText = true,
                LcdRenderText = true,
                FilterQuality = SKFilterQuality.High,
                Typeface = fontTypeface,
                TextSize = 13.5f * s,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, _width - (10f * s));
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (_height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (_width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y + (1.2f * s), glowTextPaint);
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        canvas.Restore();
    }

    private void DrawPictureScaledAndTinted(SKCanvas canvas, SKPicture picture, float iconSize, float iconTop, byte fontR, byte fontG, byte fontB)
    {
        var bounds = picture.CullRect;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        float scale = Math.Min(iconSize / bounds.Width, iconSize / bounds.Height);
        float scaledW = bounds.Width * scale;
        float scaledH = bounds.Height * scale;

        float dx = (_width - scaledW) / 2f - bounds.Left * scale;
        float dy = iconTop + (iconSize - scaledH) / 2f - bounds.Top * scale;

        using var iconPaint = new SKPaint
        {
            ColorFilter = SKColorFilter.CreateBlendMode(new SKColor(fontR, fontG, fontB), SKBlendMode.SrcIn),
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High,
            SubpixelText = true
        };

        canvas.Save();
        canvas.Translate(dx, dy);
        canvas.Scale(scale);
        canvas.SaveLayer(bounds, iconPaint);
        canvas.DrawPicture(picture);
        canvas.Restore();
        canvas.Restore();
    }

    private SKPicture? LoadSvgPicture(string? iconPath)
    {
        if (string.IsNullOrWhiteSpace(iconPath)) return null;

        var resolved = ResolveIconPath(iconPath);
        if (resolved != null)
        {
            var picture = _pictureCache.GetOrAdd(resolved, path =>
            {
                try
                {
                    var svg = new SkiaSharp.Extended.Svg.SKSvg();
                    svg.Load(path);
                    return svg.Picture;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to load SVG icon '{path}': {ex.Message}");
                    return null;
                }
            });

            if (picture != null) return picture;
        }

        // Icon path was specified but could not be resolved or loaded.
        return GetMissingIconPicture();
    }

    private static readonly Lazy<SKPicture> MissingIconPictureLazy = new(CreateMissingIconPicture);

    private static SKPicture GetMissingIconPicture() => MissingIconPictureLazy.Value;

    private static SKPicture CreateMissingIconPicture()
    {
        using var recorder = new SKPictureRecorder();
        var canvas = recorder.BeginRecording(new SKRect(0, 0, 512, 512));

        using var strokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 32f,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            Color = SKColors.White,
            IsAntialias = true
        };

        using var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White,
            IsAntialias = true
        };

        // 1. Outer rounded image frame
        canvas.DrawRoundRect(new SKRect(56, 56, 456, 456), 40, 40, strokePaint);

        // 2. Sun circle in upper right
        canvas.DrawCircle(350, 165, 34, fillPaint);

        // 3. Mountain peaks
        using (var mountainPath = new SKPath())
        {
            mountainPath.MoveTo(90, 410);
            mountainPath.LineTo(200, 270);
            mountainPath.LineTo(285, 370);
            mountainPath.LineTo(345, 310);
            mountainPath.LineTo(422, 410);
            mountainPath.Close();
            canvas.DrawPath(mountainPath, fillPaint);
        }

        // 4. Diagonal warning slash across frame
        using var slashPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 36f,
            StrokeCap = SKStrokeCap.Round,
            Color = SKColors.White,
            IsAntialias = true
        };
        canvas.DrawLine(80, 80, 432, 432, slashPaint);

        return recorder.EndRecording();
    }

    private string? ResolveIconPath(string iconPath)
    {
        if (File.Exists(iconPath)) return iconPath;

        if (!string.IsNullOrWhiteSpace(_iconRoot) && Directory.Exists(_iconRoot))
        {
            var match = TryFindInDir(_iconRoot, iconPath);
            if (match != null) return match;
        }

        var appBaseDir = AppContext.BaseDirectory;
        var appMatch = TryFindInDir(appBaseDir, iconPath);
        if (appMatch != null) return appMatch;

        return null;
    }

    private string? TryFindInDir(string baseDir, string iconPath)
    {
        var direct = Path.Combine(baseDir, iconPath);
        if (File.Exists(direct)) return direct;
        if (!direct.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) && File.Exists(direct + ".svg")) return direct + ".svg";

        var fileName = Path.GetFileName(iconPath);
        if (!fileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            fileName += ".svg";

        foreach (var sub in SubDirs)
        {
            var testPath = Path.Combine(baseDir, sub, fileName);
            if (File.Exists(testPath)) return testPath;
        }

        return null;
    }

    private List<string> WrapText(string text, SKPaint paint, float maxWidth)
    {
        var result = new List<string>();
        var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var rawLine in lines)
        {
            var words = rawLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) continue;

            string currentLine = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                string testLine = currentLine + " " + words[i];
                if (paint.MeasureText(testLine) <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    result.Add(currentLine);
                    currentLine = words[i];
                }
            }
            result.Add(currentLine);
        }

        return result.Count > 0 ? result : new List<string> { text };
    }

    public void Dispose()
    {
        _typeface.Dispose();
        foreach (var picture in _pictureCache.Values)
        {
            picture?.Dispose();
        }
        _pictureCache.Clear();
    }
}
