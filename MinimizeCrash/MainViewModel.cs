using Avalonia.Controls.Recycling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Core;

namespace MinimizeCrash;

public partial class MainViewModel : ObservableObject
{
    private readonly ScenePanelViewModel[] _panels;
    private readonly DockFactory _factory;
    private readonly IRootDock _focusLayout;
    private readonly IRootDock _maximizeLayout;

    [ObservableProperty]
    private IRootDock? _layout;

    [ObservableProperty]
    private bool _isFocusLayout = true;

    public ControlRecycling ControlRecycling { get; } = new();

    public MainViewModel()
    {
        _panels = new ScenePanelViewModel[4];
        for (int i = 0; i < 4; i++)
        {
            _panels[i] = new ScenePanelViewModel($"Panel {i + 1}");
        }

        _factory = new DockFactory(_panels);
        _focusLayout = _factory.CreateFocusLayout();
        _maximizeLayout = _factory.CreateMaximizeLayout();

        _factory.InitLayout(_focusLayout);
        _factory.InitLayout(_maximizeLayout);

        Layout = _focusLayout;
    }

    [RelayCommand]
    private void ToggleLayout()
    {
        if (IsFocusLayout)
        {
            Layout = _maximizeLayout;
            IsFocusLayout = false;
        }
        else
        {
            Layout = _focusLayout;
            IsFocusLayout = true;
        }
    }

    [RelayCommand]
    private async Task RapidToggle()
    {
        for (int i = 0; i < 30; i++)
        {
            if (IsFocusLayout)
            {
                Layout = _maximizeLayout;
                IsFocusLayout = false;
            }
            else
            {
                Layout = _focusLayout;
                IsFocusLayout = true;
            }

            await Task.Delay(50);
        }
    }
}
