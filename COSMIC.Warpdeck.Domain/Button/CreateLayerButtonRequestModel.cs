namespace COSMIC.Warpdeck.Domain.Button
{
    public class CreateLayerButtonRequestModel
    {
        public string KeyId { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}