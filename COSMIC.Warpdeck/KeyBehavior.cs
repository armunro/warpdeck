using System;
using Autofac;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Key.Action.Exceptions;
using COSMIC.Warpdeck.Domain.Key.Behavior;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;

namespace COSMIC.Warpdeck
{
    public class KeyBehavior : IHasProperties, IHasActions
    {
        private readonly int _holdDelay = 250;

        public PropertyDescriptor Category = PropertyDescriptor.Text("key.category")
            .Named("Category")
            .Described("For categorizing keys. Useful when using property rules.")
            .WithDefault("Uncategorized");

        public PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New()
            .Named("Key Properties")
            .Has(Category);

        protected readonly ActionTimer ActionTimer;


        public KeyBehavior()
        {
        }

        protected KeyBehavior(ActionTimer actionTimer)
        {
            ActionTimer = actionTimer;
        }


        public void FireEvent(BehaviorModel behaviorModel, string eventName)
        {
            //Null check
            ActionModel actionModel = behaviorModel.Actions[eventName];

            try
            {
                KeyAction keyAction = WarpdeckApp.Container.ResolveNamed<KeyAction>(actionModel.Type,
                    new NamedParameter("parameters", actionModel.Parameters));
                keyAction.StartAction();
            }
            catch (Exception)
            {
                throw new ActionNotFoundException(actionModel.Type);
            }
        }


        public void OnKeyDown(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
        }

        public void OnKeyUp(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
            FireEvent(behavior,
                keyHistory.LastDown.AddMilliseconds(_holdDelay) < DateTime.Now ? "hold" : "press");
        }


        public ActionDescriptorSet SpecifyActions() => ActionDescriptorSet.New(nameof(KeyBehavior))
            .Action(ActionDescriptor.New("press"))
            .Action(ActionDescriptor.New("hold"));
    }
}