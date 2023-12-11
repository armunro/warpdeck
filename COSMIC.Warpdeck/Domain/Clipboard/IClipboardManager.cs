using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Clipboard
{
    public interface IClipboardManager
    {
        public List<Clip> GetClips();
        public void StartMonitoring();
        public void StopMonitoring();
    }
}