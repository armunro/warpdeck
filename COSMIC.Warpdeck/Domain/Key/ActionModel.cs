using System.Collections.Generic;

namespace COSMIC.Warpdeck.Domain.Key
{
    public abstract class ActionModelModel
    {
        public abstract void MapParameters(Dictionary<string, string> parameters);
    }
}