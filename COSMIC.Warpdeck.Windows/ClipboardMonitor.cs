using System;
using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;
using WK.Libraries.SharpClipboardNS;

namespace COSMIC.Warpdeck.Windows
{
    public class ClipboardMonitor
    {
        public readonly SharpClipboard _clipboardMonitor = new();
        public List<string> _clips = new();
        public List<string> _suggestions = new();

        public ClipboardMonitor()
        {
            _clipboardMonitor.ClipboardChanged += OnClipboardMonitorOnClipboardChanged;
            _clipboardMonitor.StartMonitoring();

        }

        private void OnClipboardMonitorOnClipboardChanged(object? sender, SharpClipboard.ClipboardChangedEventArgs args)
        {
            _clips.Add($"{DateTime.Now.ToString()} {args.Content}");

            switch (args.ContentType)
            {
                case SharpClipboard.ContentTypes.Text:
                    ProcessCopiedText(args);
                    break;
                default:
                    Console.WriteLine("\t ???");
                    break;
            }
        }

        private void ProcessCopiedText(SharpClipboard.ClipboardChangedEventArgs args)
        {
            foreach (ClipboardPattern pattern in MyPatterns.Patterns)
            {
                List<ClipboardSuggestion> suggestions = pattern.OfferSuggestions(args.Content.ToString());
                foreach (ClipboardSuggestion suggestion in suggestions)
                {
                    _suggestions.Add($"{suggestion.PatternName} | {suggestion.Type} | {suggestion.Value}");
                }
            }
        }
    }
}