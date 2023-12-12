namespace COSMIC.Warpdeck.Domain.Property.Rules
{
    public interface IPropertyRule
    {
        public bool IsMetBy(PropertyLookup properties);
    }
}