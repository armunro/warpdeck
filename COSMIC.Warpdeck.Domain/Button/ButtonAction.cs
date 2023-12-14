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


        public abstract override void StartAction();
    }
    
    public abstract class ButtonAction
    {
        public abstract void StartAction();
    }
}