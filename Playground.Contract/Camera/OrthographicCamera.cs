using System.Numerics;

namespace Playground.Contract.Camera;

/// <summary>
/// Realizes the <see cref="IOrthographicCamera"/> interface.
/// </summary>
public sealed class OrthographicCamera
    : ProjectionCamera
    , IOrthographicCamera
{
    private float mWidth;
    private float mHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrthographicCamera"/> class.
    /// </summary>
    public OrthographicCamera()
    {
        mWidth  = 1;
        mHeight = 1;

        ProjectionMatrix = CalculateProjectionMatrix();
    }

    /// <inheritdoc/>
    public float Width
    {
        get => mWidth;
        set
        {
            if (SetProperty(ref mWidth, value, WidthChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public float Height
    {
        get => mHeight;
        set
        {
            if (SetProperty(ref mHeight, value, HeightChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public event EventHandler? WidthChanged;

    /// <inheritdoc/>
    public event EventHandler? HeightChanged;

    protected override void UpdateProjectionMatrix()
    {
        ProjectionMatrix = CalculateProjectionMatrix();
    }

    private Matrix4x4 CalculateProjectionMatrix()
    {
        return Matrix4x4.CreateOrthographic(Width, Height, NearPlaneDistance, FarPlaneDistance);
    }
}
