namespace COSMIC.Warpdeck.Domain.Property
{
    public class PropertyLookup : Dictionary<string, string>
    {
        public bool HasProperty(string key)
        {
            return ContainsKey(key);
        }

        public string GetProperty(string tagName)
        {
            return this[tagName];
        }
    }
}