using System;
using System.Collections.Generic;
using System.Linq;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Action.Descriptors;
using COSMIC.Warpdeck.Domain.Button;
using WindowsInput;
using WindowsInput.Events;

namespace COSMIC.Warpdeck.Windows.Plugins.Actions.Macro
{
    public class KeyMacro : ButtonAction<KeyMacroModel>, IHasActionParameters
    {
        public KeyMacro(Dictionary<string, string> parameters) : base(parameters)
        {
        }

        public KeyMacro()
        {
        }

        public override void StartAction()
        {
            string keys = Model.Keys;
            EventBuilder eventBuilder = Simulate.Events();

            string[] events = keys.Split('|');
            foreach (string keyEvent in events)
            {
                if (keyEvent.StartsWith('(') && keyEvent.EndsWith(")"))
                    eventBuilder.ClickChord(ParseChord(keyEvent));
                else
                    eventBuilder.Click(keyEvent.Trim('\''));
            }

            eventBuilder.Invoke();
        }

        private KeyCode[] ParseChord(string chordString)
        {
            string[] chordParts = chordString.Trim('(', ')').Split('+');

            return chordParts.Select(ParseKeyCode).ToArray();
        }

        private KeyCode ParseKeyCode(string chordPart)
        {
            return Enum.Parse<KeyCode>(chordPart, true);
        }

        public ActionParamDescriptorSet SpecifyParameters()
        {
            return new ActionParamDescriptorSet()
            {
                Parameters =
                {
                    new ActionParamDescriptor()
                    {
                        Name = "buttons",
                        FriendlyName = "Buttons",
                        Description = "The key macro to press"
                    }
                }
            };
        }
    }
}