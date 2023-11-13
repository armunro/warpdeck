using System;
using System.Drawing;
using System.Globalization;

namespace WarpDeck.Helpers
{
    public class IconHelpers
    {
        public static Bitmap DrawBlankKeyIcon(int width, int height)
        {
            Bitmap blankImage = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(blankImage);
            graphics.Clear(Color.Black);
            graphics.Dispose();
            return blankImage;
        }


        public static Color GetColorFromHex(string hex)
        {
            FromHex(hex, out var a, out var r, out var g, out var b);
            return Color.FromArgb(a, r, g, b);
        }

        private static void FromHex(string hex, out byte a, out byte r, out byte g, out byte b)
        {
            hex = ToRgbaHex(hex);
            if (hex == null || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                out var packedValue))
            {
                throw new ArgumentException("Hexadecimal string is not in the correct format.", nameof(hex));
            }

            a = (byte)(packedValue >> 0);
            r = (byte)(packedValue >> 24);
            g = (byte)(packedValue >> 16);
            b = (byte)(packedValue >> 8);
        }


        private static string ToRgbaHex(string hex)
        {
            hex = hex.StartsWith("#") ? hex.Substring(1) : hex;

            switch (hex.Length)
            {
                case 8:
                    return hex;
                case 6:
                    return hex + "FF";
            }

            if (hex.Length < 3 || hex.Length > 4)
                return null;

            //Handle values like #3B2
            string red = char.ToString(hex[0]);
            string green = char.ToString(hex[1]);
            string blue = char.ToString(hex[2]);
            string alpha = hex.Length == 3 ? "F" : char.ToString(hex[3]);

            return string.Concat(red, red, green, green, blue, blue, alpha, alpha);
        }
    }
}