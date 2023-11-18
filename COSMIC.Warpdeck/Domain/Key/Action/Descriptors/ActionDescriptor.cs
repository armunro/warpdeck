namespace COSMIC.Warpdeck.Domain.Key.Action.Descriptors
{
    public class ActionDescriptor
    {
        public string ActionName { get; set; }
        
        public static ActionDescriptor New(string actionName)
        {
            return new ActionDescriptor()
            {
                ActionName = actionName
            };
        }

       
    }
}