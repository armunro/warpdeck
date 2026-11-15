using OpenMacroBoard.SDK;

namespace Warpdeck.Core.Domain;

public class Layer
{
    public string Name { get; }
    public ICondition? Condition { get; }
    public int Priority { get; }
    public Dictionary<int, KeyBitmap> Keys { get; } = new();
    public Dictionary<int, (byte R, byte G, byte B)> Colors { get; } = new();
    public Dictionary<int, (byte R, byte G, byte B)> FontColors { get; } = new();
    public Dictionary<int, string> Captions { get; } = new();
    public Dictionary<int, IAction> Actions { get; } = new();

    public Layer(string name, ICondition? condition = null, int priority = 0)
    {
        Name = name;
        Condition = condition;
        Priority = priority;
    }
}
