using System.Collections.Concurrent;
using OpenMacroBoard.SDK;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using Warpdeck.Core.Domain;

namespace Warpdeck.Infrastructure.Adapters.Graphics;

public class SkiaGraphicsAdapter : IGraphicsAdapter, IDisposable
{
    private readonly SKTypeface _typeface;
    private const int Width = 72;
    private const int Height = 72;
    private readonly ConcurrentDictionary<string, SKPicture?> _pictureCache = new(StringComparer.OrdinalIgnoreCase);
    private string? _iconRoot;

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
        return DrawKey(r, g, b, fontR, fontG, fontB, text, null, null, false);
    }

    public KeyBitmap DrawKey(byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath = null, string? theme = null, bool isPressed = false)
    {
        const int renderScale = 4;
        int renderWidth = Width * renderScale;
        int renderHeight = Height * renderScale;

        using var highResSurface = SKSurface.Create(new SKImageInfo(renderWidth, renderHeight, SKColorType.Bgra8888, SKAlphaType.Premul));
        var canvas = highResSurface.Canvas;
        canvas.Scale(renderScale);

        if (string.Equals(theme, "korry", StringComparison.OrdinalIgnoreCase))
        {
            DrawKorryTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed);
        }
        else if (string.Equals(theme, "cyber", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "neon", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "hud", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(theme, "scifi", StringComparison.OrdinalIgnoreCase))
        {
            DrawCyberTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed);
        }
        else
        {
            DrawDefaultTheme(canvas, r, g, b, fontR, fontG, fontB, text, iconPath, isPressed);
        }

        using var highResImage = highResSurface.Snapshot();
        
        using var finalSurface = SKSurface.Create(new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        using var filterPaint = new SKPaint
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        finalSurface.Canvas.DrawImage(highResImage, new SKRect(0, 0, Width, Height), filterPaint);

        using var bitmap = new SKBitmap(Width, Height);
        finalSurface.ReadPixels(bitmap.Info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0);

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

    private void DrawDefaultTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed)
    {
        // Professional Pro-Deck / Dark Glassmorphic Modern UI Theme:
        // - Dark sleek graphite/slate background with subtle vertical gradient and specular top sheen
        // - Category/accent color used as a stylish accent glow, subtle top/bottom accent bar or perimeter border, and tinted icon/caption
        // - High readability, clean professional typography with subtle shadow drop
        // - Tactile mechanical depression effect when pressed

        // Clear base background with deep black
        canvas.Clear(SKColors.Black);

        const float cornerRadius = 10f;
        
        SKRect buttonRect;
        float yOffset = isPressed ? 2f : 0f;
        float iconScaleFactor = isPressed ? 0.92f : 1.0f;

        if (isPressed)
        {
            // Inset button rect slightly to simulate physical key depression
            buttonRect = new SKRect(2f, 3f, Width - 2f, Height - 1f);
        }
        else
        {
            buttonRect = new SKRect(0.5f, 0.5f, Width - 0.5f, Height - 0.5f);
        }

        // Determine accent color
        bool hasAccent = (r > 30 || g > 30 || b > 30);
        var accentColor = hasAccent ? new SKColor(r, g, b) : new SKColor(fontR, fontG, fontB);
        if (accentColor.Red < 40 && accentColor.Green < 40 && accentColor.Blue < 40)
        {
            accentColor = new SKColor(220, 225, 230);
        }

        // Clip to rounded rectangle to ensure all gradient & inner content respects the curved key corners
        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(buttonRect, cornerRadius, cornerRadius), SKClipOperation.Intersect, antialias: true);

        // 1. Sleek dark gradient background (charcoal/slate dark pro aesthetic)
        byte baseTop = isPressed ? (byte)20 : (byte)32;
        byte baseBottom = isPressed ? (byte)12 : (byte)18;
        
        // Blend in a subtle tint of the button's accent color into the dark slate
        SKColor topGradColor = new SKColor(
            (byte)Math.Clamp(baseTop + (accentColor.Red * 0.15f), 0, 255),
            (byte)Math.Clamp(baseTop + (accentColor.Green * 0.15f), 0, 255),
            (byte)Math.Clamp(baseTop + (accentColor.Blue * 0.15f), 0, 255)
        );
        SKColor bottomGradColor = new SKColor(
            (byte)Math.Clamp(baseBottom + (accentColor.Red * 0.05f), 0, 255),
            (byte)Math.Clamp(baseBottom + (accentColor.Green * 0.05f), 0, 255),
            (byte)Math.Clamp(baseBottom + (accentColor.Blue * 0.05f), 0, 255)
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
            new[] { accentColor.WithAlpha(isPressed ? (byte)45 : (byte)30), accentColor.WithAlpha(0) },
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

        // 3. Subtle top accent highlight bar (modern studio console indicator)
        if (hasAccent)
        {
            using var barPaint = new SKPaint
            {
                Color = accentColor.WithAlpha(isPressed ? (byte)220 : (byte)180),
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRoundRect(new SKRect(buttonRect.Left + 8f, buttonRect.Top + 2.5f, buttonRect.Right - 8f, buttonRect.Top + 4.5f), 1f, 1f, barPaint);
        }

        // 4. Subtle top glass specular reflection / bevel
        using (var sheenShader = SKShader.CreateLinearGradient(
            new SKPoint(0, buttonRect.Top),
            new SKPoint(0, buttonRect.Top + 14f),
            new[] { SKColors.White.WithAlpha(isPressed ? (byte)15 : (byte)40), SKColors.White.WithAlpha(0) },
            null,
            SKShaderTileMode.Clamp))
        using (var sheenPaint = new SKPaint
        {
            Shader = sheenShader,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        })
        {
            canvas.DrawRect(new SKRect(buttonRect.Left, buttonRect.Top, buttonRect.Right, buttonRect.Top + 14f), sheenPaint);
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
            canvas.DrawRect(new SKRect(buttonRect.Left, buttonRect.Top, buttonRect.Right, buttonRect.Top + 6f), shadowPaint);
        }

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        // Professional typography & icon colors
        // Text is crisp clean off-white / light silver with subtle drop shadow for maximum legibility on dark surfaces
        SKColor textColor = (fontR == 0 && fontG == 0 && fontB == 0) 
            ? new SKColor(240, 242, 245) 
            : new SKColor(fontR, fontG, fontB);

        // Icon uses the vibrant accent color (or crisp white if no accent)
        SKColor iconColor = hasAccent ? accentColor : textColor;

        if (hasPicture && hasText)
        {
            // Draw icon in top/upper-middle region
            float iconSize = 28f * iconScaleFactor;
            float iconTop = 8f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, iconColor.Red, iconColor.Green, iconColor.Blue);

            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = 12f,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 160),
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = 12f,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, Width - 6);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = (36f + yOffset) + (32f - totalHeight) / 2f + textPaint.TextSize;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                // Draw text shadow
                canvas.DrawText(line, x, textStartY + 1f, shadowPaint);
                // Draw text
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            // Draw icon centered in full button
            float iconSize = 42f * iconScaleFactor;
            float iconTop = (Height - iconSize) / 2f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, iconColor.Red, iconColor.Green, iconColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            // Centered text-only
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = 14f,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = new SKColor(0, 0, 0, 160),
                IsAntialias = true,
                Typeface = _typeface,
                TextSize = 14f,
                FakeBoldText = true
            };

            var lines = WrapText(text!, textPaint, Width - 6);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (Height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                // Draw text shadow
                canvas.DrawText(line, x, y + 1f, shadowPaint);
                // Draw text
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        // 5. Outer subtle border / bevel stroke for sleek hardware finish
        using var borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = hasAccent 
                ? accentColor.WithAlpha(isPressed ? (byte)100 : (byte)50) 
                : new SKColor(255, 255, 255, isPressed ? (byte)40 : (byte)25),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(buttonRect.Left + 0.5f, buttonRect.Top + 0.5f, buttonRect.Right - 0.5f, buttonRect.Bottom - 0.5f), cornerRadius, cornerRadius, borderPaint);

        canvas.Restore();
    }

    private void DrawKorryTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed)
    {
        // KORRY 389 / aviation switch style:
        // 1. Dark cockpit bezel background (almost black / charcoal)
        // 2. Beveled outer border with metallic/matte cockpit trim
        // 3. Inner recessed indicator display with split top/bottom annunciator windows
        // 4. Glowing illuminated annunciator indicators (white/amber/cyan/green depending on theme color or fontColor)
        
        // Clear background with black
        canvas.Clear(SKColors.Black);

        const float outerCornerRadius = 10f;
        const float innerCornerRadius = 8f;
        const float litBorderCornerRadius = 7.5f;

        // Outer Bezel / Housing
        using var bezelPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f,
            Color = new SKColor(48, 52, 58),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(1, 1, Width - 1, Height - 1), outerCornerRadius, outerCornerRadius, bezelPaint);

        // Inner recessed button face (when pressed, push deeper down and inward)
        var innerRect = isPressed 
            ? new SKRect(4f, 5.5f, Width - 4f, Height - 2.5f) 
            : new SKRect(3, 3, Width - 3, Height - 3);

        using var innerFacePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = isPressed ? new SKColor(6, 6, 8) : new SKColor(10, 10, 12),
            IsAntialias = true
        };
        canvas.DrawRoundRect(innerRect, innerCornerRadius, innerCornerRadius, innerFacePaint);

        // If pressed, draw dark top shadow inside bezel to simulate deep mechanical recess
        if (isPressed)
        {
            using var recessShadowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 190),
                IsAntialias = true
            };
            canvas.DrawRect(new SKRect(2, 2, Width - 2, 6f), recessShadowPaint);
        }

        // If a distinct non-black accent background is provided, use it as the illumination color.
        // Otherwise, use font color (if non-black) or default to white.
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

        float yOffset = isPressed ? 2f : 0f;
        float iconScaleFactor = isPressed ? 0.90f : 1.0f;
        float dividerY = (Height / 2f) + yOffset;

        // Korry horizontal divider line (classic split legend)
        using var dividerPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 1.8f : 1.5f,
            Color = litColor,
            IsAntialias = true
        };
        canvas.DrawLine(innerRect.Left + 1f, dividerY, innerRect.Right - 1f, dividerY, dividerPaint);

        // Top and bottom annunciator cell backgrounds (subtle dark glass effect with soft tint, clipped to rounded inner face)
        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(innerRect, innerCornerRadius, innerCornerRadius), SKClipOperation.Intersect, antialias: true);

        // When pressed, backlighting intensity increases (annunciator shines brighter when active/engaged)
        byte cellAlpha = isPressed ? (byte)55 : (byte)24;
        using var cellGlowPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = litColor.WithAlpha(cellAlpha),
            IsAntialias = true
        };
        canvas.DrawRect(new SKRect(innerRect.Left, innerRect.Top, innerRect.Right, dividerY - 1), cellGlowPaint);
        canvas.DrawRect(new SKRect(innerRect.Left, dividerY + 1, innerRect.Right, innerRect.Bottom), cellGlowPaint);

        if (isPressed)
        {
            // Inner shadow along top of lens
            using var topLensShadow = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 110),
                IsAntialias = true
            };
            canvas.DrawRect(new SKRect(innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Top + 4f), topLensShadow);
        }

        canvas.Restore();

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        if (hasPicture && hasText)
        {
            // Upper half: Icon (annunciator symbol)
            float iconSize = 24f * iconScaleFactor;
            float iconTop = 6f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, litColor.Red, litColor.Green, litColor.Blue);

            // Lower half: Aviation uppercase stencil/sans text
            using var fontTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            using var textPaint = new SKPaint
            {
                Color = litColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 11.5f
            };

            string korryText = text!.ToUpperInvariant();
            var lines = WrapText(korryText, textPaint, Width - 8);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = dividerY + ((innerRect.Bottom - dividerY) - totalHeight) / 2f + textPaint.TextSize - 1f;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            // Centered Illuminated Icon
            float iconSize = 36f * iconScaleFactor;
            float iconTop = (Height - iconSize) / 2f + yOffset;
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, litColor.Red, litColor.Green, litColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            // Split or centered aviation text
            using var fontTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            using var textPaint = new SKPaint
            {
                Color = litColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 13f
            };

            string korryText = text!.ToUpperInvariant();
            var lines = WrapText(korryText, textPaint, Width - 8);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (Height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y, textPaint);
                y += lineHeight;
            }
        }

        // Inner glowing border indicator highlight
        using var litBorderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 1.5f : 1f,
            Color = litColor.WithAlpha(isPressed ? (byte)120 : (byte)70),
            IsAntialias = true
        };
        canvas.DrawRoundRect(new SKRect(innerRect.Left + 0.5f, innerRect.Top + 0.5f, innerRect.Right - 0.5f, innerRect.Bottom - 0.5f), litBorderCornerRadius, litBorderCornerRadius, litBorderPaint);
    }

    private void DrawCyberTheme(SKCanvas canvas, byte r, byte g, byte b, byte fontR, byte fontG, byte fontB, string? text, string? iconPath, bool isPressed)
    {
        // Cyberpunk / Sci-Fi Tactical HUD Theme:
        // - Deep tech obsidian background with subtle horizontal scanlines
        // - Radial ambient neon bloom in accent color
        // - Tactical HUD corner brackets with glowing neon accents
        // - Mini status LED ticks and tactical indicator marks
        // - Neon bloom & vibrant backlit icon and high-contrast HUD typography
        // - Overdrive neon pulse and compression effect when pressed

        canvas.Clear(SKColors.Black);

        const float cornerRadius = 8f;
        float yOffset = isPressed ? 1.5f : 0f;
        float iconScaleFactor = isPressed ? 0.94f : 1.0f;

        SKRect buttonRect = isPressed
            ? new SKRect(2f, 2.5f, Width - 2f, Height - 0.5f)
            : new SKRect(0.5f, 0.5f, Width - 0.5f, Height - 0.5f);

        // Determine neon accent color
        bool hasAccent = (r > 30 || g > 30 || b > 30);
        var accentColor = hasAccent ? new SKColor(r, g, b) : new SKColor(fontR, fontG, fontB);
        if (accentColor.Red < 40 && accentColor.Green < 40 && accentColor.Blue < 40)
        {
            accentColor = new SKColor(0, 229, 255); // Default to Electric Cyan
        }

        canvas.Save();
        canvas.ClipRoundRect(new SKRoundRect(buttonRect, cornerRadius, cornerRadius), SKClipOperation.Intersect, antialias: true);

        // 1. Tech Obsidian Background with subtle vertical gradient
        byte bgTop = isPressed ? (byte)10 : (byte)14;
        byte bgBottom = isPressed ? (byte)4 : (byte)6;
        SKColor topGrad = new SKColor(
            (byte)Math.Clamp(bgTop + (accentColor.Red * 0.08f), 0, 255),
            (byte)Math.Clamp(bgTop + (accentColor.Green * 0.08f), 0, 255),
            (byte)Math.Clamp(bgTop + (accentColor.Blue * 0.08f), 0, 255)
        );
        SKColor bottomGrad = new SKColor(
            (byte)Math.Clamp(bgBottom + (accentColor.Red * 0.03f), 0, 255),
            (byte)Math.Clamp(bgBottom + (accentColor.Green * 0.03f), 0, 255),
            (byte)Math.Clamp(bgBottom + (accentColor.Blue * 0.03f), 0, 255)
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

        // 2. High-tech scanlines
        using (var scanlinePaint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 12),
            StrokeWidth = 0.5f,
            IsAntialias = false,
            Style = SKPaintStyle.Stroke
        })
        {
            for (float y = buttonRect.Top + 2f; y < buttonRect.Bottom - 1f; y += 3f)
            {
                canvas.DrawLine(buttonRect.Left, y, buttonRect.Right, y, scanlinePaint);
            }
        }

        // 3. Cyber Neon Ambient Bloom / Core Glow
        using (var glowShader = SKShader.CreateRadialGradient(
            new SKPoint(buttonRect.MidX, buttonRect.MidY),
            buttonRect.Width * 0.7f,
            new[] { accentColor.WithAlpha(isPressed ? (byte)70 : (byte)35), accentColor.WithAlpha(0) },
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
            StrokeWidth = 1f,
            Color = accentColor.WithAlpha(isPressed ? (byte)160 : (byte)50),
            IsAntialias = true
        })
        {
            canvas.DrawRoundRect(new SKRect(buttonRect.Left + 0.5f, buttonRect.Top + 0.5f, buttonRect.Right - 0.5f, buttonRect.Bottom - 0.5f), cornerRadius, cornerRadius, framePaint);
        }

        // 5. Tactical HUD Corner Brackets (Glowing neon brackets on 4 corners)
        float pad = 3f;
        float bLeft = buttonRect.Left + pad;
        float bTop = buttonRect.Top + pad;
        float bRight = buttonRect.Right - pad;
        float bBottom = buttonRect.Bottom - pad;
        float bracketLen = 8f;

        using (var bracketPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = isPressed ? 2.0f : 1.5f,
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

        // 6. Mini Tactical Status Ticks (3 mini LED dots at the top right)
        using (var ledPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = accentColor.WithAlpha(isPressed ? (byte)255 : (byte)180),
            IsAntialias = true
        })
        {
            float dotY = bTop + 2.5f;
            float dotStartX = bRight - bracketLen - 12f;
            for (int i = 0; i < 3; i++)
            {
                canvas.DrawRect(new SKRect(dotStartX + i * 4.5f, dotY, dotStartX + i * 4.5f + 2f, dotY + 2f), ledPaint);
            }
        }

        // 7. Mini Tech Slash Marks in bottom left
        using (var tickPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1f,
            Color = accentColor.WithAlpha(isPressed ? (byte)200 : (byte)120),
            IsAntialias = true
        })
        {
            float slashX = bLeft + bracketLen + 2f;
            float slashY = bBottom - 2f;
            canvas.DrawLine(slashX, slashY, slashX + 2f, slashY - 4f, tickPaint);
            canvas.DrawLine(slashX + 3.5f, slashY, slashX + 5.5f, slashY - 4f, tickPaint);
        }

        var picture = LoadSvgPicture(iconPath);
        bool hasPicture = picture != null;
        bool hasText = !string.IsNullOrWhiteSpace(text);

        // Futuristic typography & Neon lighting
        SKTypeface fontTypeface = SKTypeface.FromFamilyName("Consolas", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright) ?? _typeface;
        
        SKColor textColor = new SKColor(245, 250, 255);
        SKColor glowColor = accentColor.WithAlpha(180);

        if (hasPicture && hasText)
        {
            float iconSize = 27f * iconScaleFactor;
            float iconTop = 8.5f + yOffset;

            // Draw glowing neon icon background bloom
            DrawPictureScaledAndTinted(canvas, picture!, iconSize + 2f, iconTop - 1f, (byte)(accentColor.Red / 2), (byte)(accentColor.Green / 2), (byte)(accentColor.Blue / 2));
            // Draw crisp neon icon
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, accentColor.Red, accentColor.Green, accentColor.Blue);

            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 11.5f,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = glowColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 11.5f,
                FakeBoldText = true
            };

            var lines = WrapText(text!.ToUpperInvariant(), textPaint, Width - 10);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float textStartY = (36f + yOffset) + (31f - totalHeight) / 2f + textPaint.TextSize;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                // Neon glow under text
                canvas.DrawText(line, x, textStartY, shadowPaint);
                // Crisp text
                canvas.DrawText(line, x, textStartY, textPaint);
                textStartY += lineHeight;
            }
        }
        else if (hasPicture && !hasText)
        {
            float iconSize = 40f * iconScaleFactor;
            float iconTop = (Height - iconSize) / 2f + yOffset;
            
            // Neon bloom
            DrawPictureScaledAndTinted(canvas, picture!, iconSize + 2f, iconTop - 1f, (byte)(accentColor.Red / 2), (byte)(accentColor.Green / 2), (byte)(accentColor.Blue / 2));
            // Sharp vector
            DrawPictureScaledAndTinted(canvas, picture!, iconSize, iconTop, accentColor.Red, accentColor.Green, accentColor.Blue);
        }
        else if (!hasPicture && hasText)
        {
            using var textPaint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 13f,
                FakeBoldText = true
            };

            using var shadowPaint = new SKPaint
            {
                Color = glowColor,
                IsAntialias = true,
                Typeface = fontTypeface,
                TextSize = 13f,
                FakeBoldText = true
            };

            var lines = WrapText(text!.ToUpperInvariant(), textPaint, Width - 10);
            float lineHeight = textPaint.FontSpacing;
            float totalHeight = lines.Count * lineHeight;
            float y = (Height - totalHeight) / 2f + textPaint.TextSize + yOffset;

            foreach (var line in lines)
            {
                float x = (Width - textPaint.MeasureText(line)) / 2f;
                canvas.DrawText(line, x, y, shadowPaint);
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

        float dx = (Width - scaledW) / 2f - bounds.Left * scale;
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
        canvas.DrawPicture(picture, iconPaint);
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
        // Return preprogrammed indicator showing the icon is not available.
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

        // 4. Prominent diagonal slash indicating unavailable/broken
        canvas.DrawLine(36, 36, 476, 476, strokePaint);

        return recorder.EndRecording();
    }

    private string? ResolveIconPath(string iconPath)
    {
        if (string.IsNullOrWhiteSpace(iconPath)) return null;

        if (File.Exists(iconPath)) return Path.GetFullPath(iconPath);

        var normalized = iconPath.Replace('/', '\\');

        if (File.Exists(Path.Combine(AppContext.BaseDirectory, normalized)))
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, normalized));

        if (!string.IsNullOrWhiteSpace(_iconRoot))
        {
            var resolvedInCustomRoot = TryResolveInBaseDir(_iconRoot, normalized);
            if (resolvedInCustomRoot != null) return resolvedInCustomRoot;
        }

        var baseAppResolved = TryResolveInBaseDir(AppContext.BaseDirectory, normalized);
        if (baseAppResolved != null) return baseAppResolved;

        return null;
    }

    private static string? TryResolveInBaseDir(string baseDir, string normalized)
    {
        if (!Directory.Exists(baseDir)) return null;

        var direct = Path.Combine(baseDir, normalized);
        if (File.Exists(direct)) return direct;
        if (!direct.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) && File.Exists(direct + ".svg"))
            return direct + ".svg";

        foreach (var sub in SubDirs)
        {
            var subPath = Path.Combine(baseDir, sub, normalized);
            if (File.Exists(subPath)) return subPath;
            if (!subPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) && File.Exists(subPath + ".svg"))
                return subPath + ".svg";
        }

        return null;
    }

    private static List<string> WrapText(string text, SKPaint paint, float maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            if (paint.MeasureText(testLine) > maxWidth)
            {
                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }
        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines;
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
