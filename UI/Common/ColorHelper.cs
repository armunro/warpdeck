using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Warpdeck.UI.Common;

public static class ColorHelper
{
    public static readonly List<string> ExpandedPresetColors = new()
    {
        // Neutrals & Grayscale
        "#000000", "#16181D", "#2D3748", "#4A5568", "#718096", "#A0AEC0", "#E2E8F0", "#FFFFFF",
        // Vivid / Cyber Neons
        "#0070F3", "#00B0FF", "#00E5FF", "#00E676", "#76FF03", "#FFD600", "#FF9100", "#FF6B00",
        "#FF1744", "#FF007F", "#FF4081", "#E040FB", "#7928CA", "#651FFF", "#00BFA5", "#1DE9B6",
        // Classic & Deep Shades
        "#1A365D", "#3182CE", "#0D9488", "#059669", "#15803D", "#B45309", "#C2410C", "#B91C1C",
        "#BE185D", "#6B21A8", "#4338CA", "#374151"
    };

    public static readonly List<string> RecentCustomColors = new()
    {
        "#0070F3", "#7928CA", "#FF6B00", "#00E676", "#FFD600", "#E040FB", "#00E5FF", "#FF1744"
    };

    public static void AddRecentColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return;
        hex = NormalizeHex(hex);
        if (string.IsNullOrEmpty(hex)) return;

        RecentCustomColors.Remove(hex);
        RecentCustomColors.Insert(0, hex);

        if (RecentCustomColors.Count > 16)
        {
            RecentCustomColors.RemoveAt(RecentCustomColors.Count - 1);
        }
    }

    public static string NormalizeHex(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var s = input.Trim().TrimStart('#');
        if (s.Length == 3)
        {
            s = $"{s[0]}{s[0]}{s[1]}{s[1]}{s[2]}{s[2]}";
        }

        if (s.Length == 6 && uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            return $"#{s.ToUpperInvariant()}";
        }
        else if (s.Length == 8 && uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            return $"#{s.Substring(2).ToUpperInvariant()}";
        }

        return input.StartsWith("#") ? input.ToUpperInvariant() : $"#{input.ToUpperInvariant()}";
    }

    public static Color ParseHex(string? input, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(input)) return fallback;

        var s = input.Trim().TrimStart('#');
        if (s.Length == 3)
        {
            s = $"{s[0]}{s[0]}{s[1]}{s[1]}{s[2]}{s[2]}";
        }

        if (s.Length == 6 && uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hex))
        {
            byte r = (byte)((hex >> 16) & 0xFF);
            byte g = (byte)((hex >> 8) & 0xFF);
            byte b = (byte)(hex & 0xFF);
            return Color.FromRgb(r, g, b);
        }

        if (s.Length == 8 && uint.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexAlpha))
        {
            byte a = (byte)((hexAlpha >> 24) & 0xFF);
            byte r = (byte)((hexAlpha >> 16) & 0xFF);
            byte g = (byte)((hexAlpha >> 8) & 0xFF);
            byte b = (byte)(hexAlpha & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }

        return fallback;
    }

    public static string ToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    public static (double H, double S, double V) RgbToHsv(byte r, byte g, byte b)
    {
        double rd = r / 255.0;
        double gd = g / 255.0;
        double bd = b / 255.0;

        double max = Math.Max(rd, Math.Max(gd, bd));
        double min = Math.Min(rd, Math.Min(gd, bd));
        double delta = max - min;

        double h = 0;
        if (delta > 0.00001)
        {
            if (Math.Abs(max - rd) < 0.00001)
            {
                h = 60 * (((gd - bd) / delta) % 6);
            }
            else if (Math.Abs(max - gd) < 0.00001)
            {
                h = 60 * (((bd - rd) / delta) + 2);
            }
            else
            {
                h = 60 * (((rd - gd) / delta) + 4);
            }
        }

        if (h < 0) h += 360;

        double s = max > 0.00001 ? delta / max : 0;
        double v = max;

        return (h, s, v);
    }

    public static (byte R, byte G, byte B) HsvToRgb(double h, double s, double v)
    {
        h = (h % 360 + 360) % 360;
        s = Math.Clamp(s, 0.0, 1.0);
        v = Math.Clamp(v, 0.0, 1.0);

        double c = v * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = v - c;

        double r1 = 0, g1 = 0, b1 = 0;
        if (h < 60)
        {
            r1 = c; g1 = x; b1 = 0;
        }
        else if (h < 120)
        {
            r1 = x; g1 = c; b1 = 0;
        }
        else if (h < 180)
        {
            r1 = 0; g1 = c; b1 = x;
        }
        else if (h < 240)
        {
            r1 = 0; g1 = x; b1 = c;
        }
        else if (h < 300)
        {
            r1 = x; g1 = 0; b1 = c;
        }
        else
        {
            r1 = c; g1 = 0; b1 = x;
        }

        byte r = (byte)Math.Round((r1 + m) * 255);
        byte g = (byte)Math.Round((g1 + m) * 255);
        byte b = (byte)Math.Round((b1 + m) * 255);

        return (r, g, b);
    }
}
