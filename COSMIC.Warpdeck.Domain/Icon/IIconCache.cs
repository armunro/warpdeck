using COSMIC.Warpdeck.Domain.Button;

namespace COSMIC.Warpdeck.Domain.Icon
{
    public interface IIconCache
    {
        bool DoesCacheHaveIcon(ButtonModel buttonModel);
        KeyIcon GetIcon(ButtonModel buttonModel);
        KeyIcon SetIcon(ButtonModel model, KeyIcon icon);
        void Clear();
    }
}