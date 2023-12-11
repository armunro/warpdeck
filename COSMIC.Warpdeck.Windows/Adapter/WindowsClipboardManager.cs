using System;
using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;
using WK.Libraries.SharpClipboardNS;

namespace COSMIC.Warpdeck.Windows.Adapter
{
    public class WindowsClipboardManager : IClipboardManager
    {
        private readonly SharpClipboard _clipboardMonitor = new();
        public List<Clip> Clips = new();
        

        public WindowsClipboardManager()
        {
            _clipboardMonitor.ClipboardChanged += OnClipboardMonitorOnClipboardChanged;

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
                    if(textSuggestions.Count > 0)
                        newClip.Suggestions = textSuggestions;
                    break;
                default:
                    Console.WriteLine("\t ???");
                    break;
            }
            Clips.Add(newClip);
        }

        private List<ClipSuggestion> ProcessCopiedTextSuggestions(SharpClipboard.ClipboardChangedEventArgs args)
        {
            List<ClipSuggestion> returnSuggestion = new();
            
            foreach (ClipboardPattern pattern in MyPatterns.Patterns)
            {
                List<ClipSuggestion> suggestions = pattern.OfferSuggestions(args.Content.ToString());
                foreach (ClipSuggestion suggestion in suggestions)
                {
                    returnSuggestion.Add(suggestion);

                }
            }

            return returnSuggestion;
        }

        public List<Clip> GetClips()
        {
            return Clips;
        }

        public void StartMonitoring()
        {
            _clipboardMonitor.StartMonitoring();
        }

        public void StopMonitoring()
        {
           _clipboardMonitor.StopMonitoring();
        }
        
    }
}