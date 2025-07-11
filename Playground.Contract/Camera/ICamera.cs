using System.Numerics;

namespace Playground.Contract.Camera;

public interface ICamera
{
    Vector3   Position      { get; set; }
    Vector3   LookDirection { get; set; }
    Vector3   UpDirection   { get; set; }
    Matrix4x4 ViewMatrix    { get; }

    event EventHandler? PositionChanged;
    event EventHandler? UpDirectionChanged;
    event EventHandler? LookDirectionChanged;
    event EventHandler? ViewMatrixChanged;
}
