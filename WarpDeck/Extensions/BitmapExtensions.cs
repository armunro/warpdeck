using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WarpDeck.Extensions
{
    public static class BitmapExtensions
    {
        public static MemoryStream ToMemoryStream(this Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}