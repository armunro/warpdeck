using System;
using System.Collections.Generic;
using System.Linq;
using Warpdeck.Infrastructure.Adapters.Yaml;

namespace Warpdeck.UI.Common;

public class WorkspaceSnapshot
{
    public List<LayerConfig> Layers { get; init; } = new();
    public string? IconRoot { get; init; }
    public int Brightness { get; init; } = 100;
    public int SelectedLayerIndex { get; init; }
    public int? SelectedButtonIndex { get; init; }
    public string Description { get; init; } = string.Empty;

    public static LayerConfig CloneLayer(LayerConfig source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new LayerConfig
        {
            Name = source.Name,
            Priority = source.Priority,
            Theme = source.Theme,
            Color = source.Color,
            FontColor = source.FontColor,
            Font = source.Font,
            IconRoot = source.IconRoot,
            ShowEmptyButtons = source.ShowEmptyButtons,
            Condition = source.Condition != null ? new ConditionConfig
            {
                ProcessName = source.Condition.ProcessName,
                WindowTitle = source.Condition.WindowTitle
            } : null,
            Buttons = source.Buttons?.Select(b => new ButtonConfig
            {
                Index = b.Index,
                Caption = b.Caption,
                Icon = b.Icon,
                Theme = b.Theme,
                Color = b.Color,
                FontColor = b.FontColor,
                Font = b.Font,
                Locked = b.Locked,
                Action = b.Action != null ? new ActionConfig
                {
                    Type = b.Action.Type,
                    Value = b.Action.Value
                } : null
            }).ToList() ?? new()
        };
    }
}

public class UndoRedoStack<T>
{
    private readonly LinkedList<T> _items = new();
    private readonly int _maxCapacity;

    public UndoRedoStack(int maxCapacity = 100)
    {
        _maxCapacity = maxCapacity;
    }

    public int Count => _items.Count;

    public void Push(T item)
    {
        _items.AddLast(item);
        if (_items.Count > _maxCapacity)
        {
            _items.RemoveFirst();
        }
    }

    public T Pop()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
        var val = _items.Last!.Value;
        _items.RemoveLast();
        return val;
    }

    public T Peek()
    {
        if (_items.Count == 0) throw new InvalidOperationException("Stack is empty");
        return _items.Last!.Value;
    }

    public void Clear() => _items.Clear();
}
