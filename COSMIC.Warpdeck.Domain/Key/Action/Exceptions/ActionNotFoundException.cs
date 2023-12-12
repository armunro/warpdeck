namespace COSMIC.Warpdeck.Domain.Key.Action.Exceptions
{
    public class ActionNotFoundException : Exception
    {
        public ActionNotFoundException(string actionModelType) :base ($"An action with the type {actionModelType} was not found.")
        {
            
        }
    }
}