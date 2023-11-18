using System;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Key.Behavior;

namespace COSMIC.Warpdeck.Plugins.Behaviors
{
    public class PressAndHold : KeyBehavior
    {
        private readonly int _holdDelay = 250;

        public override ActionDescriptorSet SpecifyActions() => ActionDescriptorSet.New(nameof(PressAndHold))
            .Action(ActionDescriptor.New("press"))
            .Action(ActionDescriptor.New("hold"));
        
        public PressAndHold(KeyTimer keyTimer) : base(keyTimer)
        {
        }

        public override void OnKeyDown(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
        }

        public override void OnKeyUp(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
            FireEvent(behavior,
                keyHistory.LastDown.AddMilliseconds(_holdDelay) < DateTime.Now ? "hold" : "press");
        }
    }
}