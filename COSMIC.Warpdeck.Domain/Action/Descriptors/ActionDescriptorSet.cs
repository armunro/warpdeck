namespace COSMIC.Warpdeck.Domain.Action.Descriptors
{
    public class ActionDescriptorSet
    {
        public string BehaviorName { get; set; }
        public List<ActionDescriptor> Actions { get; set; } = new();

        public static ActionDescriptorSet New(string behaviorName)
        {
            return new ActionDescriptorSet() { BehaviorName = behaviorName };
        }

        public ActionDescriptorSet Action(ActionDescriptor actionRequest)
        {
            Actions.Add(actionRequest);
            return this;
        }
    }
}