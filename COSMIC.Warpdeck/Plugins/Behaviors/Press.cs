using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Key.Behavior;

namespace COSMIC.Warpdeck.Plugins.Behaviors
{
    public class Press : KeyBehavior
    {
        public override ActionDescriptorSet SpecifyActions() =>
            ActionDescriptorSet.New(nameof(Press)).Action(ActionDescriptor.New("press"));

        public Press(KeyTimer keyTimer) : base(keyTimer)
        {
        }

        public override void OnKeyDown(DeviceModel device, int key, BehaviorModel behavior,
            KeyHistoryModel keyHistory)
        {
            KeyTimer.RegisterRepeatable(device.DeviceId, key, 250, () => FireEvent(behavior, "press"));
        }

        public override void OnKeyUp(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
            KeyTimer.UnregisterRepeatable(device.DeviceId, key);
        }
    }
}