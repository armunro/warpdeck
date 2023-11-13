using System;
using System.Collections.Generic;
using System.Linq;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Key.Action;
using WarpDeck.Domain.Key.Action.Descriptors;
using WindowsInput;
using WindowsInput.Events;

namespace WarpDeck.Windows.Plugins.Actions.Macro
{
    public class KeyMacro : KeyAction<KeyMacroModelModel>, IHasActionParameters
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
                        Name = "keys",
                        FriendlyName = "Keys",
                        Description = "The key macro to press"
                    }
                }
            };
        }
    }
}