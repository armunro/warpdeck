using System;
using System.Collections.Generic;
using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Action.Exceptions;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Button.Behavior;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using ActionModel = COSMIC.Warpdeck.Domain.Action.ActionModel;

namespace COSMIC.Warpdeck
{
    public class ButtonBehavior : IHasProperties, IHasActions
    {
        private readonly int _holdDelay = 250;

        
        public PropertyDescriptor Category = PropertyDescriptor.Text("key.category")
            .Named("Category")
            .Described("For categorizing buttons. Useful when using property rules.")
            .WithDefault("Uncategorized");

        public PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New()
            .Named("Button Properties")
            .Has(Category);


  

        public void FireEvent(ButtonModel buttonModel, string eventName)
        {
            //Null check
            ActionModel actionModel = buttonModel.Actions[eventName];

            try
            {
                //TODO: Cannot move this to Domain package because WarpdeckAppContext will not be available
                ButtonAction buttonAction = WarpdeckAppContext.Container.ResolveNamed<ButtonAction>(actionModel.Type,
                    new NamedParameter("parameters", actionModel.Parameters));
                buttonAction.StartAction(actionModel);
            }
            catch (Exception ex)
            {
                throw new ActionNotFoundException(actionModel.Type);
            }
        }


        public void OnKeyDown(DeviceModel device, int key, ButtonModel buttonModel, ButtonHistoryModel buttonHistory)
        {
        }

        public void OnKeyUp(DeviceModel device, int key, ButtonModel buttonModel, ButtonHistoryModel buttonHistory)
        {
            FireEvent(buttonModel, buttonHistory.LastDown.AddMilliseconds(_holdDelay) < DateTime.Now ? "hold" : "press");
        }


        public ActionDescriptorSet SpecifyActions() => ActionDescriptorSet.New(nameof(ButtonBehavior))
            .Action(ActionDescriptor.New("press"))
            .Action(ActionDescriptor.New("hold"));
    }
}