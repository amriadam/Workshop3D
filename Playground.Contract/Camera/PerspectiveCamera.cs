using System.Numerics;

namespace Playground.Contract.Camera;

public sealed class PerspectiveCamera
    : ProjectionCamera
{
    private float mFieldOfView;
    private float mAspectRatio;

    public PerspectiveCamera()
    {
        mFieldOfView      = 45.0f;
        mAspectRatio      = 1.0f;
        ProjectionMatrix  = CalculateProjectionMatrix();
    }

    public float FieldOfView
    {
        get => mFieldOfView;
        set
        {
            if (SetProperty(ref mFieldOfView, value, FieldOfViewChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    public float AspectRatio
    {
        get => mAspectRatio;
        set
        {
            if (SetProperty(ref mAspectRatio, value, AspectRatioChanged))
            {
                UpdateProjectionMatrix();
            }
        }
    }

    public event EventHandler? FieldOfViewChanged;
    public event EventHandler? AspectRatioChanged;

    protected override void UpdateProjectionMatrix()
    {
        ProjectionMatrix = CalculateProjectionMatrix();
    }

    private Matrix4x4 CalculateProjectionMatrix()
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(DegToRad(FieldOfView), AspectRatio, NearPlaneDistance, FarPlaneDistance);
    }

    private static float DegToRad(float deg)
    {
        const double PiOver180 = Math.PI / 180.0;

        return (float)(deg * PiOver180);
    }
}
