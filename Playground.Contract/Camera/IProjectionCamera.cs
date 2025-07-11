using System.Numerics;

namespace Playground.Contract.Camera;

public interface IProjectionCamera
    : ICamera
{
    float     NearPlaneDistance { get; set; }
    float     FarPlaneDistance  { get; set; }
    Matrix4x4 ProjectionMatrix  { get; }

    event EventHandler? NearPlaneDistanceChanged;
    event EventHandler? FarPlaneDistanceChanged;
    event EventHandler? ProjectionMatrixChanged;
}
