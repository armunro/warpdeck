using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;

namespace COSMIC.Warpdeck.Managers;

public class ClipListManager
{
    private readonly IClipListReader _clipListReader;
    public List<ClipList> Lists { get; private set; }
    
    

    public ClipListManager (IClipListReader clipListReader)
    {
        _clipListReader = clipListReader;
    }

    public List<ClipList> GetClips()
    {
        Lists = _clipListReader.ReadClipLists();
        return Lists;
    }


}