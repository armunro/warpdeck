namespace COSMIC.Warpdeck.Domain.Action.Descriptors
{
    public class ActionParamDescriptorSet
    {
        public  List<ActionParamDescriptor> Parameters { get; }= new();
        
        public ActionParamDescriptorSet Parameter(string name, string friendlyName, string description)
        {
            Parameters.Add(new ActionParamDescriptor()
            {
                Name = name,
                FriendlyName = friendlyName,
                Description = description
            });
            return this;
        }
    }
}