using OpenTK.Mathematics;

namespace WinFormsApp1;

/// <summary>
/// Represents a camera in a 3D graphics scene, responsible for generating the view matrix.
/// The view matrix transforms world-space coordinates into camera-space coordinates.
/// </summary>
public sealed class GLCamera
{
    private Vector3 mPosition   = Vector3.Zero;  // Position.
    private Vector3 mTarget     = Vector3.Zero;  // Look at the origin
    private Vector3 mUp         = Vector3.UnitY; // Up direction

    /// <summary>
    /// Initializes a new instance of the <see cref="GLCamera"/> class.
    /// </summary>
    public GLCamera()
    {
        UpdateViewMatrix();
    }

    /// <summary>
    /// Gets or sets the position of the camera in world space.
    /// Setting the position will automatically recalculate the view matrix.
    /// </summary>
    public Vector3 Position
    {
        get => mPosition;
        set
        {
            mPosition = value; 
            UpdateViewMatrix();
        }
    }

    /// <summary>
    /// Gets or sets the point in world space that the camera is looking at.
    /// Setting the target will automatically recalculate the view matrix.
    /// </summary>
    public Vector3 Target
    {
        get => mTarget;
        set
        {
            mTarget = value; 
            UpdateViewMatrix();
        }
    }

    /// <summary>
    /// Gets or sets the up direction vector for the camera.
    /// Setting the up direction will automatically recalculate the view matrix.
    /// </summary>
    public Vector3 Up
    {
        get => mUp;
        set
        { 
            mUp = value;
            UpdateViewMatrix();
        }
    }

    /// <summary>
    /// Gets the current view matrix of the camera.
    /// The view matrix transforms world coordinates to camera (or view) coordinates.
    /// It's derived from the camera's position, target, and up direction.
    /// </summary>
    public Matrix4 ViewMatrix
    {
        get;
        private set;
    }

    private void UpdateViewMatrix()
    {
        ViewMatrix = Matrix4.LookAt(mPosition, mTarget, mUp);
    }
}
