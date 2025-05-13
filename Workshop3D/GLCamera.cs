using OpenTK.Mathematics;

namespace WinFormsApp1;

public sealed class GLCamera
{
    private Vector3 mPosition   = Vector3.Zero;  // Position.
    private Vector3 mTarget     = Vector3.Zero;  // Look at the origin
    private Vector3 mUp         = Vector3.UnitY; // Up direction

    public GLCamera()
    {
        UpdateViewMatrix();
    }

    public Vector3 Position
    {
        get => mPosition;
        set
        {
            mPosition = value; 
            UpdateViewMatrix();
        }
    }

    public Vector3 Target
    {
        get => mTarget;
        set
        {
            mTarget = value; 
            UpdateViewMatrix();
        }
    }

    public Vector3 Up
    {
        get => mUp;
        set
        { 
            mUp = value;
            UpdateViewMatrix();
        }
    }

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
