namespace COSMIC.Warpdeck.Domain.Button
{
    public class ButtonMap : Dictionary<int, ButtonModel>
    {
        public void UpdateKeyState(int keyId, ButtonModel buttonModel)
        {
            this[keyId] = buttonModel;
        }
        public bool IsKeyMapped(int keyId) => ContainsKey(keyId);
    }
}