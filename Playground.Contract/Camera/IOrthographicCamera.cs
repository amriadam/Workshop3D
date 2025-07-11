namespace Playground.Contract.Camera;

public interface IOrthographicCamera
    : IProjectionCamera
{
    float Width  { get; set; }
    float Height { get; set; }

    event EventHandler? WidthChanged;
    event EventHandler? HeightChanged;
}
