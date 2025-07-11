namespace Playground.Contract.Camera;

public interface IPerspectiveCamera
    : IProjectionCamera
{
    float FieldOfView  { get; set; }
    float AspectRatio { get; set; }

    event EventHandler? FieldOfViewChanged;
    event EventHandler? AspectRatioChanged;
}
