using Ab4d.SharpEngine.AvaloniaUI;
using Avalonia;
using Avalonia.Controls;

namespace MinimizeCrash.Views;

public partial class ScenePanelView : UserControl
{
    private SharpEngineSceneView? _sceneView;

    public ScenePanelView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ScenePanelViewModel vm)
        {
            _sceneView = vm.SceneView;
            Content = _sceneView;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Content = null;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_sceneView is not null)
        {
            Content = _sceneView;
            _sceneView.RenderScene();
        }
    }
}
