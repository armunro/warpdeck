using System;
using System.Collections.Generic;
using Autofac;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Action.Exceptions;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using ActionModel = COSMIC.Warpdeck.Domain.Action.ActionModel;

namespace COSMIC.Warpdeck
{
    public class ButtonBehavior : IHasProperties, IHasActions
    {
        private readonly int _holdDelay = 250;

        
        public PropertyDescriptor Category = PropertyDescriptor.Text("Category")
            .Named("Category")
            .Described("For categorizing buttons. Useful when using property rules.")
            .WithDefault("Uncategorized");

        public PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New()
            .Named("Button Properties")
            .Has(Category);


  

        public void TriggerButtonAction(ButtonModel buttonModel, string actionName)
        {
            ActionModel actionModel = buttonModel.Actions[actionName];
            try
            {
                TriggerAction(actionModel);
            }
            catch (Exception ex)
            {
                throw new ActionNotFoundException(actionModel.Type);
            }
        }

        public void TriggerAction(ActionModel action)
        {
            ButtonAction buttonAction = WarpdeckAppContext.Container.ResolveNamed<ButtonAction>(action.Type,
                new NamedParameter("parameters", action.Parameters));
            buttonAction.StartAction(action);
        }


        public void OnKeyDown(DeviceModel device, string keyId, ButtonModel buttonModel, ButtonHistoryModel buttonHistory)
        {
        }

        public void OnKeyUp(DeviceModel device, string keyId, ButtonModel buttonModel, ButtonHistoryModel buttonHistory)
        {
            TriggerButtonAction(buttonModel, buttonHistory.LastDown.AddMilliseconds(_holdDelay) < DateTime.Now ? "Hold" : "Press");
        }


        public ActionDescriptorSet SpecifyActions() => ActionDescriptorSet.New(nameof(ButtonBehavior))
            .Action(ActionDescriptor.New("Press"))
            .Action(ActionDescriptor.New("Hold"));
    }
}