using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using COSMIC.Warpdeck.Windows.Domain;
using WK.Libraries.SharpClipboardNS;

namespace COSMIC.Warpdeck.Windows
{
    public partial class ClipboardForm : Form
    {
        readonly SharpClipboard _clipboardMonitor = new();
        readonly ListBox _clips = new();
        readonly ListBox _suggestions = new();

        public ClipboardForm()
        {
            _clipboardMonitor.ClipboardChanged += OnClipboardMonitorOnClipboardChanged;
            InitializeComponent();
            _clips.Dock = DockStyle.Left;
            _suggestions.Dock = DockStyle.Right;
            _clips.Width = this.Width / 3;

            _suggestions.Width = (this.Width / 3) * 2 - 20;
            _clips.Height = this.Height;
            _suggestions.Height = this.Height;
            _clips.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            //_suggestions.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom ;
            Controls.Add(_clips);
            Controls.Add(_suggestions);
        }

        private void OnClipboardMonitorOnClipboardChanged(object? sender, SharpClipboard.ClipboardChangedEventArgs args)
        {
            _clips.Items.Add($"{DateTime.Now.ToString()}   {args.Content}");

            switch (args.ContentType)
            {
                case SharpClipboard.ContentTypes.Text:
                    ProcessCopiedText(args);
                    break;
                case SharpClipboard.ContentTypes.Image:
                    Console.WriteLine("\t You copied an image but I don't know how to handle those. Yet...");
                    break;
                case SharpClipboard.ContentTypes.Files:
                    Console.WriteLine("\t You files files but I don't know how to handle those. Yet...");
                    break;
                case SharpClipboard.ContentTypes.Other:
                    Console.WriteLine("\t ???");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessCopiedText(SharpClipboard.ClipboardChangedEventArgs args)
        {
            foreach (Pattern pattern in MyPatterns.Patterns)
            {
                List<Suggestion> suggestions = pattern.OfferSuggestions(args.Content.ToString());
                foreach (Suggestion suggestion in suggestions)
                {
                    _suggestions.Items.Add($"{suggestion.PatternName} | {suggestion.Type} | {suggestion.Value}");
                }
            }
        }
    }
}