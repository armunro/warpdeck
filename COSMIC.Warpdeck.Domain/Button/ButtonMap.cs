namespace COSMIC.Warpdeck.Domain.Button
{
    public class ButtonMap : Dictionary<string, ButtonModel>
    {
        public void UpdateKeyState(string keyId, ButtonModel buttonModel)
        {
            this[keyId] = buttonModel;
        }
        public bool IsKeyMapped(string keyId) => ContainsKey(keyId);
    }
}