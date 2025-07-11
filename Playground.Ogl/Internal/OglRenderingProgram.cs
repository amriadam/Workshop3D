using System.Numerics;

namespace Playground.Ogl.Internal;

internal sealed class OglRenderingProgram
    : OglDefaultProgram
{
    private readonly int mLightModeUniformLocation      = -1;
    private readonly int mLightDirectionUniformLocation = -1;

    private int     mLightMode;
    private Vector3 mLightDirection;

    public OglRenderingProgram()
        : base(OglShaderSourceCode.RenderVertexShader, OglShaderSourceCode.RenderFragmentShader)
    {
        try
        {
            mLightModeUniformLocation      = GetUniformLocation("uLightMode");
            mLightDirectionUniformLocation = GetUniformLocation("uLightDirection");

            mLightDirection = -Vector3.UnitY;

            TrySetUniform(mLightModeUniformLocation     , mLightMode);
            TrySetUniform(mLightDirectionUniformLocation, mLightDirection);
        }
        catch
        {
            Dispose(false);
            throw;
        }
    }

    public int LightMode
    {
        get => mLightMode;
        set
        {
            if (mLightMode == value)
            {
                return;
            }

            TrySetUniform(mLightModeUniformLocation, value);

            mLightMode = value;
        }
    }

    public Vector3 LightDirection
    {
        get => mLightDirection;
        set
        {
            if (mLightDirection == value)
            {
                return;
            }

            TrySetUniform(mLightDirectionUniformLocation, value);

            mLightDirection = value;
        }
    }
}
