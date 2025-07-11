using System.Numerics;

namespace Playground.Contract.Camera;

/// <summary>
/// Realizes the <see cref="ICamera"/> interface.
/// </summary>
public abstract class Camera
    : ICamera
{
    private Vector3   mPosition;
    private Vector3   mUpDirection;
    private Vector3   mLookDirection;
    private Matrix4x4 mViewMatrix;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera"/> class.
    /// </summary>
    protected Camera()
    {
        mPosition      =  Vector3.Zero;
        mUpDirection   =  Vector3.UnitY;
        mLookDirection = -Vector3.UnitZ;
        mViewMatrix    =  CalculateViewMatrix();
    }

    /// <inheritdoc/>
    public Vector3 Position
    {
        get => mPosition;
        set
        {
            if (SetProperty(ref mPosition, value, PositionChanged))
            {
                UpdateViewMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public Vector3 UpDirection
    {
        get => mUpDirection;
        set
        {
            if (SetProperty(ref mUpDirection, value, UpDirectionChanged))
            {
                UpdateViewMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public Vector3 LookDirection
    {
        get => mLookDirection;
        set
        {
            if (SetProperty(ref mLookDirection, value, LookDirectionChanged))
            {
                UpdateViewMatrix();
            }
        }
    }

    /// <inheritdoc/>
    public Matrix4x4 ViewMatrix
    {
        get => mViewMatrix;
        private set => SetProperty(ref mViewMatrix, value, ViewMatrixChanged);
    }

    /// <inheritdoc/>
    public event EventHandler? PositionChanged;

    /// <inheritdoc/>
    public event EventHandler? UpDirectionChanged;

    /// <inheritdoc/>
    public event EventHandler? LookDirectionChanged;

    /// <inheritdoc/>
    public event EventHandler? ViewMatrixChanged;

    protected bool SetProperty<T>(ref T field, T value, EventHandler? handler, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if (comparer.Equals(field, value))
        {
            return false;
        }

        field = value;

        handler?.Invoke(this, EventArgs.Empty);

        return true;
    }

    private void UpdateViewMatrix()
    {
        ViewMatrix = CalculateViewMatrix();
    }

    private Matrix4x4 CalculateViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, Position + LookDirection, UpDirection);
    }
}
