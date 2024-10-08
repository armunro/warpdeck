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
        
        public WindowsClipboardManager(IClipPatternReader clipPatternReader)
        {
            Patterns = clipPatternReader.ReadPatterns();
            _clipboardMonitor.ClipboardChanged += OnClipboardMonitorOnClipboardChanged;
        }

        public List<ClipPattern> Patterns { get; set; }

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
                    newClip.Suggestions.Add(new ClipSuggestion()
                    {
                        Match = "",
                        PatternName = "",
                        ActionName = "PasteAction",
                        ActionParameters = "{ \"Text\": \"" + args.Content.ToString() + "\"}"
                        
                    });
                    break;
            }

            _clips.Add(newClip);
        }

        private List<ClipSuggestion> ProcessCopiedTextSuggestions(SharpClipboard.ClipboardChangedEventArgs args)
        {
            List<ClipSuggestion> returnSuggestion = new();
            foreach (ClipPattern pattern in Patterns)
            {
                returnSuggestion.AddRange(pattern.OfferSuggestions(args.Content.ToString()));
            }

            return returnSuggestion;
        }
    }
}