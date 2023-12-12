namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public interface IClipboardManager
    {
        public List<ClipPattern> Patterns { get; set; }
        public List<Clip> GetClips();
        public void StartMonitoring();
        public void StopMonitoring();
    }
}