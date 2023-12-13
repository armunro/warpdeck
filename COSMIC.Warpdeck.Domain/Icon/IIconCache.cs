using COSMIC.Warpdeck.Domain.Key;

namespace COSMIC.Warpdeck.Domain.Icon
{
    public interface IIconCache
    {
        bool DoesCacheHaveIcon(KeyModel keyModel);
        KeyIcon GetIcon(KeyModel keyModel);
        KeyIcon SetIcon(KeyModel model, KeyIcon icon);
        void Clear();
    }
}