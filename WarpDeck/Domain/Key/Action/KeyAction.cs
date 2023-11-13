using System.Collections.Generic;

namespace WarpDeck.Domain.Key.Action
{
    public abstract class KeyAction<TModel> : KeyAction where TModel : ActionModelModel, new()
    {
        public TModel Model { get; set; }

   
        public KeyAction(Dictionary<string, string> parameters)
        {
            Model = new TModel();
            Model.MapParameters(parameters);
        }

        public KeyAction()
        {
            Model = new TModel();
        }


        public abstract override void StartAction();
    }
    
    public abstract class KeyAction
    {
        public abstract void StartAction();
    }
}