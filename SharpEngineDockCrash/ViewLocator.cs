using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model.Core;
using SharpEngineDockCrash.Views;

namespace SharpEngineDockCrash;

public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> s_views = new()
    {
        [typeof(ScenePanelViewModel)] = () => new ScenePanelView(),
    };

    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var type = data.GetType();
        if (s_views.TryGetValue(type, out var factory))
        {
            return factory();
        }

        return new TextBlock { Text = $"ViewLocator: no view for {type.Name}" };
    }

    public bool Match(object? data)
    {
        if (data is null)
        {
            return false;
        }

        return data is IDockable || s_views.ContainsKey(data.GetType());
    }
}
