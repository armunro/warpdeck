using COSMIC.Warpdeck.Domain.Clipboard;

namespace COSMIC.Warpdeck.Domain.Configuration
{
    public interface IClipListReader
    {
        List<ClipList> ReadClipLists();
    }
}