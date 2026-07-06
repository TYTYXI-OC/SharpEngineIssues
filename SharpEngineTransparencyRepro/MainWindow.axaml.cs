using System;
using System.Numerics;
using Ab4d.SharpEngine.AvaloniaUI;
using Ab4d.SharpEngine.Cameras;
using Ab4d.SharpEngine.Common;
using Ab4d.SharpEngine.Lights;
using Ab4d.SharpEngine.Materials;
using Ab4d.SharpEngine.Meshes;
using Ab4d.SharpEngine.SceneNodes;
using Ab4d.SharpEngine.Utilities;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SharpColors = Ab4d.SharpEngine.Common.Colors;

namespace SharpEngineTransparencyRepro;

public partial class MainWindow : Window
{
    private readonly PointerCameraController _pointerCameraController;
    private readonly SharpEngineSceneView _sceneView;

    private FreeCamera? _camera;

    private StandardMesh? _sphereMesh;
    private MeshTrianglesSorter? _sphereSorter;

    private StandardMesh? _tubeMesh;
    private MeshTrianglesSorter? _tubeSorter;

    private bool _isTriangleSortingEnabled = true;

    private string _currentDepthStateName = "DepthRead";

    private int _cameraChangedCount;

    public MainWindow()
    {
        InitializeComponent();

        _sceneView = new SharpEngineSceneView(PresentationTypes.SharedTexture);
        _pointerCameraController = new PointerCameraController(_sceneView)
        {
            RotateCameraConditions = PointerAndKeyboardConditions.LeftPointerButtonPressed,
            MoveCameraConditions = PointerAndKeyboardConditions.LeftPointerButtonPressed | PointerAndKeyboardConditions.ControlKey,
            QuickZoomConditions = PointerAndKeyboardConditions.LeftPointerButtonPressed | PointerAndKeyboardConditions.RightPointerButtonPressed,
            RotateAroundPointerPosition = false,
            ZoomMode = CameraZoomMode.ViewCenter,
            CameraSmoothing = CameraController.CameraSmoothingPresets.Normal
        };

        SceneViewBorder.Child = _sceneView;
        ConfigureScene();

        Unloaded += OnUnloaded;
    }

    private void ConfigureScene()
    {
        _camera = new FreeCamera
        {
            CameraPosition = new Vector3(-964.1814f, 633.92737f, 1148.2133f),
            ViewWidth = 1000,
            TargetPosition = Vector3.Zero,
            UpDirection = Vector3.UnitY,
            ShowCameraLight = ShowCameraLightType.Auto
        };

        _camera.CalculateCurrentUpDirection();
        _camera.CameraChanged += OnCameraChanged;
        _sceneView.SceneView.Camera = _camera;

        var scene = _sceneView.Scene;
        scene.RootNode.Clear();
        scene.Lights.Clear();
        scene.Lights.Add(new DirectionalLight(new Vector3(-1, -0.35f, -0.25f)));
        scene.SetAmbientLight(0.35f);

        scene.IsTransparencySortingEnabled = false;
        scene.DefaultTransparentDepthStencilState = CommonStatesManager.DepthRead;

        var sphereMaterial = new StandardMaterial(SharpColors.SeaGreen)
        {
            Opacity = 0.28f
        };
        _sphereMesh = MeshFactory.CreateSphereMesh(
            centerPosition: Vector3.Zero,
            radius: 90,
            segments: 30,
            name: "SphereMesh");
        _sphereSorter = new MeshTrianglesSorter(_sphereMesh.Vertices!, _sphereMesh.TriangleIndices!);

        var sphereNode = new MeshModelNode(_sphereMesh, sphereMaterial)
        {
            Name = "TransparentSphere",
            BackMaterial = sphereMaterial
        };

        var tubeMaterial = new StandardMaterial(SharpColors.OrangeRed)
        {
            Opacity = 0.55f
        };
        _tubeMesh = MeshFactory.CreateTubeMeshAlongPath(
            pathPositions: CreateHelixPositions(),
            radius: 9,
            isTubeClosed: true,
            isPathClosed: false,
            segments: 16,
            pathPositionTextureCoordinates: null,
            generateTextureCoordinates: false,
            name: "HelixMesh");
        _tubeSorter = new MeshTrianglesSorter(_tubeMesh.Vertices!, _tubeMesh.TriangleIndices!);

        var tubeNode = new MeshModelNode(_tubeMesh, tubeMaterial)
        {
            Name = "TransparentHelix",
            BackMaterial = tubeMaterial
        };

        scene.RootNode.Add(sphereNode);
        scene.RootNode.Add(tubeNode);

        SortAllTriangles();

        UpdateStatus();
    }

    private static Vector3[] CreateHelixPositions()
    {
        const int POINT_COUNT = 96;
        const float HELIX_RADIUS = 65;
        const float HELIX_LENGTH = 260;
        const float TURN_COUNT = 3.5f;

        var positions = new Vector3[POINT_COUNT];

        for (int i = 0; i < POINT_COUNT; i++)
        {
            float t = (float)i / (POINT_COUNT - 1);
            float angle = t * TURN_COUNT * MathF.Tau;
            float x = (t - 0.5f) * HELIX_LENGTH;
            float y = HELIX_RADIUS * MathF.Cos(angle);
            float z = HELIX_RADIUS * MathF.Sin(angle);
            positions[i] = new Vector3(x, y, z);
        }

        return positions;
    }

    private void OnCameraChanged(object? sender, EventArgs e)
    {
        _cameraChangedCount++;
        if (_isTriangleSortingEnabled)
            SortAllTriangles();
        UpdateStatus();
    }

    private void SortAllTriangles()
    {
        if (_camera == null)
            return;

        var cameraPosition = _camera.CameraPosition;
        SortMesh(_sphereMesh, _sphereSorter, cameraPosition);
        SortMesh(_tubeMesh, _tubeSorter, cameraPosition);
    }

    private static void SortMesh(StandardMesh? mesh, MeshTrianglesSorter? sorter, Vector3 cameraPosition)
    {
        if (mesh == null || sorter == null)
            return;

        var sortedIndices = sorter.SortByCameraDistance(
            cameraPosition,
            checkIfAlreadySorted: true,
            out _);

        if (!ReferenceEquals(mesh.TriangleIndices, sortedIndices))
        {
            mesh.TriangleIndices = sortedIndices;
        }
        else if (mesh.Scene != null && mesh.Scene.GpuDevice != null)
        {
            mesh.RecreateIndexBuffer();
        }
    }

    private void UseDepthRead_OnClick(object? sender, RoutedEventArgs e)
    {
        _sceneView.Scene.DefaultTransparentDepthStencilState = CommonStatesManager.DepthRead;
        _currentDepthStateName = "DepthRead";
        UpdateStatus();
        _sceneView.RenderScene();
    }

    private void UseDepthReadWrite_OnClick(object? sender, RoutedEventArgs e)
    {
        _sceneView.Scene.DefaultTransparentDepthStencilState = CommonStatesManager.DepthReadWrite;
        _currentDepthStateName = "DepthReadWrite";
        UpdateStatus();
        _sceneView.RenderScene();
    }

    private void TriangleSorting_OnClick(object? sender, RoutedEventArgs e)
    {
        _isTriangleSortingEnabled = TriangleSortingCheckBox.IsChecked == true;

        if (_isTriangleSortingEnabled)
            SortAllTriangles();

        UpdateStatus();
        _sceneView.RenderScene();
    }

    private void ObjectSorting_OnClick(object? sender, RoutedEventArgs e)
    {
        _sceneView.Scene.IsTransparencySortingEnabled = ObjectSortingCheckBox.IsChecked == true;
        UpdateStatus();
        _sceneView.RenderScene();
    }

    private void ForceSort_OnClick(object? sender, RoutedEventArgs e)
    {
        SortAllTriangles();
        _sceneView.RenderScene();
    }

    private void UpdateStatus()
    {
        var sortState = _isTriangleSortingEnabled ? "ON" : "OFF";
        var objSortState = _sceneView.Scene.IsTransparencySortingEnabled ? "ON" : "OFF";
        StatusText.Text = $"Depth: {_currentDepthStateName} | TriSort: {sortState} | ObjSort: {objSortState} | camEvents: {_cameraChangedCount}";
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (_camera != null)
            _camera.CameraChanged -= OnCameraChanged;

        _sceneView.Dispose();
        GC.KeepAlive(_pointerCameraController);
    }
}
