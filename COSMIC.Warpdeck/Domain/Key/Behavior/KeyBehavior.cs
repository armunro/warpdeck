using System;
using Autofac;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Key.Action.Exceptions;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;

namespace COSMIC.Warpdeck.Domain.Key.Behavior
{
    public  class KeyBehavior : IHasProperties, IHasActions
    {
        private readonly int _holdDelay = 250;
        public PropertyDescriptor Category = PropertyDescriptor.Text("key.category")
            .Named("Category")
            .Described("For categorizing keys. Useful when using property rules.")
            .WithDefault("Uncategorized");
        
        public PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New()
            .Named("Key Properties")
            .Has(Category);

        protected readonly KeyTimer KeyTimer;

        
        public KeyBehavior()
        {
            
        }
        
        protected KeyBehavior(KeyTimer keyTimer)
        {
            KeyTimer = keyTimer;
        }


        protected void FireEvent(BehaviorModel behaviorModel, string eventName)
        {
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
        
        
        public  void OnKeyDown(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
        }

        public  void OnKeyUp(DeviceModel device, int key, BehaviorModel behavior, KeyHistoryModel keyHistory)
        {
            FireEvent(behavior,
                keyHistory.LastDown.AddMilliseconds(_holdDelay) < DateTime.Now ? "hold" : "press");
        }

      

        public ActionDescriptorSet SpecifyActions() => ActionDescriptorSet.New(nameof(KeyBehavior))
            .Action(ActionDescriptor.New("press"))
            .Action(ActionDescriptor.New("hold"));

    }
}