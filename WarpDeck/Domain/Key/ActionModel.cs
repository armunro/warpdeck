using System.Collections.Generic;

namespace WarpDeck.Domain.Key
{
    public abstract class ActionModelModel
    {
        public abstract void MapParameters(Dictionary<string, string> parameters);
    }
}