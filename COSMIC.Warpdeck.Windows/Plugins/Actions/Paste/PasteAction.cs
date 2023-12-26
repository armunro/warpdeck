using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Button;
using WindowsInput;
using WindowsInput.Events;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Paste;

public class PasteAction : ButtonAction<PasteModel>, IHasActionParameters
{
    public PasteAction(Dictionary<string, string> parameters) : base(parameters)
    {
    }

    public PasteAction()
    {
    }

    public override void StartAction(ActionModel actionModel)
    {
        EventBuilder eventBuilder = Simulate.Events();
        eventBuilder.Wait(1000);
        eventBuilder.Click(Model.Text);
        eventBuilder.Invoke();
    }

    public ActionParamDescriptorSet SpecifyParameters() =>
        new ActionParamDescriptorSet().Parameter("Text", "Text",
            "The text to send to the focused window.");
}