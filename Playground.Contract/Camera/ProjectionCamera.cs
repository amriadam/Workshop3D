using System.Numerics;

namespace Playground.Contract.Camera;

/// <summary>
/// Realizes the <see cref="IProjectionCamera"/> interface.
/// </summary>
public abstract class ProjectionCamera
    : Camera
    , IProjectionCamera
{
    private float     mNearPlaneDistance;
    private float     mFarPlaneDistance;
    private Matrix4x4 mProjectionMatrix;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionCamera"/> class.
    /// </summary>
    protected ProjectionCamera()
    {
        mNearPlaneDistance = 1e-3f;
        mFarPlaneDistance  = 1e+3f;
    }

    /// <inheritdoc/>
    public float NearPlaneDistance
    {
        get => mNearPlaneDistance;
        set
        {
            if (SetProperty(ref mNearPlaneDistance, value, NearPlaneDistanceChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public float FarPlaneDistance
    {
        get => mFarPlaneDistance;
        set
        {
            if (SetProperty(ref mFarPlaneDistance, value, FarPlaneDistanceChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public Matrix4x4 ProjectionMatrix
    {
        get => mProjectionMatrix;
        protected set => SetProperty(ref mProjectionMatrix, value, ProjectionMatrixChanged);
    }

    /// <inheritdoc/>
    public event EventHandler? NearPlaneDistanceChanged;

    /// <inheritdoc/>
    public event EventHandler? FarPlaneDistanceChanged;

    /// <inheritdoc/>
    public event EventHandler? ProjectionMatrixChanged;

    protected abstract void UpdateProjectionMatrix();
}
