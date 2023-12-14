namespace COSMIC.Warpdeck.Domain.Action.Descriptors
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