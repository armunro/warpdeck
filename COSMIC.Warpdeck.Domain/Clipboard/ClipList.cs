namespace COSMIC.Warpdeck.Domain.Clipboard;

public class ClipList
{
    public List<Clip> Clips { get; set; } = new List<Clip>();

    public void Add(string fileContents)
    {
        Clip clip = new Clip()
        {
            Text = fileContents,
            Suggestions = new List<ClipSuggestion>(),
            Time = DateTime.MinValue
        };
        Clips.Add(clip);
    }
}