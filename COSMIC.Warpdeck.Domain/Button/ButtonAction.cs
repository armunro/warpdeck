using COSMIC.Warpdeck.Domain.Action;

namespace COSMIC.Warpdeck.Domain.Button
{
    public abstract class ButtonAction<TModel> : ButtonAction where TModel : Button.ButtonActionModel, new()
    {
        public TModel Model { get; set; }
        
   
        public ButtonAction(Dictionary<string, string> parameters)
        {
            Model = new TModel();
            Model.MapParameters(parameters);
        }

        public ButtonAction()
        {
            Model = new TModel();
        }   


        public abstract override void StartAction(ActionModel actionModel);
    }
    
    public abstract class ButtonAction
    {
        public string Name { get; set; }
        public abstract void StartAction(ActionModel actionModel);
        
    }
}