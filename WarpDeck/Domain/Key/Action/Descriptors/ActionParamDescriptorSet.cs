using System.Collections.Generic;

namespace WarpDeck.Domain.Key.Action.Descriptors
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