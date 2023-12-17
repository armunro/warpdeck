namespace COSMIC.Warpdeck.Domain.Button
{
    public class CreateLayerButtonRequestModel
    {
        public int KeyId { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}