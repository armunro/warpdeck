using System;
using Autofac;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key.Action;
using WarpDeck.Domain.Key.Action.Descriptors;
using WarpDeck.Domain.Key.Action.Exceptions;
using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Descriptors;

namespace WarpDeck.Domain.Key.Behavior
{
    public abstract class KeyBehavior : IHasProperties, IHasActions
    {
        public PropertyDescriptor Category = PropertyDescriptor.Text("key.category")
            .Named("Category")
            .Described("For categorizing keys. Useful when using property rules.")
            .WithDefault("Uncategorized");
        
        public PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New()
            .Named("Key Properties")
            .Has(Category);

        protected readonly KeyTimer KeyTimer;

        protected KeyBehavior(KeyTimer keyTimer)
        {
            KeyTimer = keyTimer;
        }


        protected void FireEvent(BehaviorModel behaviorModel, string eventName)
        {
            ActionModel actionModel = behaviorModel.Actions[eventName];

            try
            {
                KeyAction keyAction = WarpDeckApp.Container.ResolveNamed<KeyAction>(actionModel.Type,
                    new NamedParameter("parameters", actionModel.Parameters));
                keyAction.StartAction();
            }
            catch (Exception)
            {
                throw new ActionNotFoundException(actionModel.Type);
            }

           
        }

        public abstract void OnKeyDown(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory);
        public abstract void OnKeyUp(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory);

        public abstract ActionDescriptorSet SpecifyActions();

    }
}