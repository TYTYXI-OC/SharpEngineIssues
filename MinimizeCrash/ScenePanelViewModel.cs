using System.Numerics;
using Ab4d.SharpEngine.AvaloniaUI;
using Ab4d.SharpEngine.Cameras;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Materials;
using Ab4d.SharpEngine.SceneNodes;
using Avalonia.Media;
using Dock.Model.Mvvm.Controls;

namespace MinimizeCrash;

public partial class ScenePanelViewModel : Tool
{
    public SharpEngineSceneView SceneView { get; }

    public ScenePanelViewModel(string title)
    {
        Id = title;
        Title = title;
        SceneView = CreateSceneView();
    }

    private static SharpEngineSceneView CreateSceneView()
    {
        var sceneView = new SharpEngineSceneView(PresentationTypes.SharedTexture);

        sceneView.Initialize();

        sceneView.BackgroundColor = Avalonia.Media.Colors.White;

        sceneView.SceneView.Camera = new TargetPositionCamera
        {
            Heading = -40,
            Attitude = -25,
            Distance = 300,
            TargetPosition = new Vector3(0, 0, 0),
            ShowCameraLight = ShowCameraLightType.Auto
        };

        var scene = sceneView.Scene;
        if (scene is not null)
        {
            var sphere = new SphereModelNode("DefaultSphere")
            {
                CenterPosition = new Vector3(0, 0, 0),
                Radius = 50,
                Material = StandardMaterials.Red
            };
            scene.RootNode.Add(sphere);
        }

        return sceneView;
    }
}
