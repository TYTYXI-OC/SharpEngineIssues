using Dock.Model.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;

namespace SharpEngineDockCrash;

public class DockFactory : Dock.Model.Mvvm.Factory
{
    private readonly ScenePanelViewModel[] _panels;

    public DockFactory(ScenePanelViewModel[] panels)
    {
        _panels = panels;
    }

    public IRootDock CreateFocusLayout()
    {
        var leftPanel = new ToolDock
        {
            Id = "Focus_Left",
            Title = "Focus Left",
            ActiveDockable = _panels[0],
            VisibleDockables = CreateList<IDockable>(_panels[0])
        };

        var leftContainer = new ProportionalDock
        {
            Id = "Focus_LeftContainer",
            Proportion = 0.67,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(leftPanel)
        };

        var rightStack = new ProportionalDock
        {
            Id = "Focus_RightStack",
            Proportion = 0.33,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(
                Tool("Focus_R1", _panels[1]),
                Tool("Focus_R2", _panels[2]),
                Tool("Focus_R3", _panels[3])
            )
        };

        var mainSplit = new ProportionalDock
        {
            Id = "Focus_Main",
            Proportion = double.NaN,
            Orientation = Orientation.Horizontal,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(leftContainer, rightStack)
        };

        return Root(mainSplit);
    }

    public IRootDock CreateMaximizeLayout()
    {
        var maximized = Tool("Max_Main", _panels[0]);

        var container = new ProportionalDock
        {
            Id = "Max_Container",
            Proportion = double.NaN,
            Orientation = Orientation.Horizontal,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>(maximized)
        };

        return Root(container);
    }

    private ToolDock Tool(string id, ScenePanelViewModel vm)
    {
        return new ToolDock
        {
            Id = id,
            Title = vm.Title,
            ActiveDockable = vm,
            VisibleDockables = CreateList<IDockable>(vm)
        };
    }

    private IRootDock Root(IDockable content)
    {
        return new RootDock
        {
            Id = "Root",
            Title = "Root",
            IsCollapsable = false,
            ActiveDockable = content,
            DefaultDockable = content,
            VisibleDockables = CreateList<IDockable>(content)
        };
    }
}
