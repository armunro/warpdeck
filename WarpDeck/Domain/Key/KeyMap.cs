using System.Collections.Generic;

namespace WarpDeck.Domain.Key
{
    public class KeyMap : Dictionary<int, KeyModel>
    {
        public void UpdateKeyState(int keyId, KeyModel keyModel)
        {
            this[keyId] = keyModel;
        }


        public bool IsKeyMapped(int keyId) => ContainsKey(keyId);
    }
}