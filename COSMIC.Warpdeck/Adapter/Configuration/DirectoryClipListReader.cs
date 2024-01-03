using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;

namespace COSMIC.Warpdeck.Adapter.Configuration;

public class DirectoryClipListReader : IClipListReader
{
    public string _configBaseDir;

    public DirectoryClipListReader(string configBaseDir)
    {
        _configBaseDir = configBaseDir;
    }

    public List<ClipList> ReadClipLists()
    {
        List<ClipList> returnLists = new List<ClipList>();
        
        if(Directory.Exists(_configBaseDir))
        {
        string[] directories = Directory.GetDirectories(_configBaseDir);
        foreach (string directory in directories)
        {
            ClipList clips = ProcessDirectory(directory);
            returnLists.Add(clips);
        }
        }

        return returnLists;
    }

    private ClipList ProcessDirectory(string directory)
    {
        ClipList clips = new ClipList();
        
        string[] files = Directory.GetFiles(directory);
        foreach (string file in files.Where(x => x.ToLower().EndsWith("txt")))
        {
            string fileContents = File.ReadAllText(file);
            clips.Add(fileContents);
        }

        return clips;
    }
}
