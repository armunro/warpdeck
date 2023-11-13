using System;
using System.Collections.Generic;
using System.Drawing;
using OpenMacroBoard.SDK;

namespace WarpDeck.Adapter.Hardware
{
    
    public class VirtualBoard : IMacroBoard
    {
        public void Dispose()
        {
        }

        public void SetBrightness(byte percent)
        {
        }

        public void SetKeyBitmap(int keyId, KeyBitmap bitmapData)
        {
        }

        public void ShowLogo()
        {
        }

        public IKeyPositionCollection Keys { get; } = new KeyPositionCollection(new List<Rectangle>());
        public bool IsConnected { get; }
#pragma warning disable CS0067 // Not handled or accessible.
        public event EventHandler<KeyEventArgs> KeyStateChanged;
        
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;
#pragma warning restore CS0067
    }
}