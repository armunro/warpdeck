using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenMacroBoard.SDK;

namespace Warpdeck.UI.Common;

public static class WpfImageHelper
{
    public static BitmapSource? ToBitmapSource(KeyBitmap? keyBitmap)
    {
        if (keyBitmap == null) return null;

        try
        {
            if (keyBitmap is IKeyBitmapDataAccess dataAccess)
            {
                var dataSpan = dataAccess.GetData();
                var raw = dataSpan.ToArray();
                int width = keyBitmap.Width > 0 ? keyBitmap.Width : 96;
                int height = keyBitmap.Height > 0 ? keyBitmap.Height : 96;
                var stride = width * 3;
                var bmp = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr24, null, raw, stride);
                bmp.Freeze();
                return bmp;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
