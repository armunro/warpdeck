namespace COSMIC.Warpdeck.Domain.Button
{
    public class ButtonHistoryModel
    {
        public DateTime LastDown { get; set; }
        public DateTime LastUp { get; set; }
        public DateTime LastSetIcon { get; set; }
    }
}