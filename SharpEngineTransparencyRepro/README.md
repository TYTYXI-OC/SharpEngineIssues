# SharpEngine Transparency Repro

This is one repro subproject under the `SharpEngineIssues` solution.

Minimal Avalonia + Ab4d SharpEngine project for reproducing transparency ordering artifacts.

The app uses Ab4d's public samples license by default. To run with a private Ab4d license, set these environment variables before launching:

```powershell
$env:AB4D_LICENSE_OWNER = "Your owner"
$env:AB4D_LICENSE_TYPE = "Your license type"
$env:AB4D_LICENSE = "Your license key"
```

The scene contains:

- A transparent green sphere (radius 90) centered at the origin, rendered as a `MeshModelNode` so its triangles can be re-sorted per camera change.
- A transparent red helix tube (radius 65, length 260, 3.5 turns) wrapped inside the sphere, also a `MeshModelNode`.
- `Scene.IsTransparencySortingEnabled = true` (still useful for object-level order).
- `Scene.DefaultTransparentDepthStencilState = CommonStatesManager.DepthRead` (toggleable to `DepthReadWrite`).
- Per-triangle sorting via `MeshTrianglesSorter` on every `FreeCamera.CameraChanged` event. The sphere and the helix are both non-convex self-overlapping surfaces whose bounding-box centers coincide at the origin, so object-level sort alone is unstable — `MeshTrianglesSorter` reorders each mesh's triangles back-to-front for the current camera position, eliminating the transparency/opaque flicker while rotating.

The toolbar lets you compare:
- `DepthRead` vs `DepthReadWrite` (transparent depth-stencil state).
- `Per-triangle sorting` on/off (when off, the original flicker reappears while rotating).

Run:

```powershell
dotnet run --project SharpEngineTransparencyRepro/SharpEngineTransparencyRepro.csproj --configuration Debug
```

Rotate with the left mouse button and zoom with the wheel. The toolbar buttons switch the global transparent depth state between `DepthRead` and `DepthReadWrite` for comparison.
