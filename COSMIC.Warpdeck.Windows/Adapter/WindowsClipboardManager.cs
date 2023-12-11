using System;
using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;
using WK.Libraries.SharpClipboardNS;

namespace COSMIC.Warpdeck.Windows.Adapter
{
    public class WindowsClipboardManager : IClipboardManager
    {
        private readonly SharpClipboard _clipboardMonitor = new();
        private readonly List<Clip> _clips = new();
        private readonly List<ClipPattern> _patterns = new();


        public WindowsClipboardManager(IClipPatternReader clipPatternReader)
        {
            _patterns = clipPatternReader.ReadPatterns();
            _clipboardMonitor.ClipboardChanged += OnClipboardMonitorOnClipboardChanged;
        }

        public List<Clip> GetClips()
        {
            return _clips;
        }

        public void StartMonitoring()
        {
            _clipboardMonitor.StartMonitoring();
        }

        public void StopMonitoring()
        {
            _clipboardMonitor.StopMonitoring();
        }


        private void OnClipboardMonitorOnClipboardChanged(object? sender, SharpClipboard.ClipboardChangedEventArgs args)
        {
            Clip newClip = new()
            {
                Text = args.Content.ToString(),
                Time = DateTime.Now
            };

            switch (args.ContentType)
            {
                case SharpClipboard.ContentTypes.Text:
                    var textSuggestions = ProcessCopiedTextSuggestions(args);
                    newClip.Suggestions = textSuggestions;
                    break;
                default:
                    break;
            }

            _clips.Add(newClip);
        }

        private List<ClipSuggestion> ProcessCopiedTextSuggestions(SharpClipboard.ClipboardChangedEventArgs args)
        {
            List<ClipSuggestion> returnSuggestion = new();
            foreach (ClipPattern pattern in _patterns)
            {
                returnSuggestion.AddRange(pattern.OfferSuggestions(args.Content.ToString()));
            }

            return returnSuggestion;
        }
    }
}