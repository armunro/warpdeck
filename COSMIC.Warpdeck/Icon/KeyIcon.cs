using System.Drawing;

namespace COSMIC.Warpdeck.Domain.Icon
{
    public class KeyIcon
    {
        private readonly Bitmap _bitmap;


        public Bitmap ToBitmap() => _bitmap;

        public KeyIcon(Bitmap bitmap)
        {
            _bitmap = (Bitmap) bitmap.Clone();
        }
    }
}