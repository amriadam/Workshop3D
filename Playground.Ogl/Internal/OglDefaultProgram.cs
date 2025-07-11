using System.Numerics;

namespace Playground.Ogl.Internal;

internal abstract class OglDefaultProgram
    : OglProgram
{
    private readonly int mModelMatrixUniformLocation      = -1;
    private readonly int mViewMatrixUniformLocation       = -1;
    private readonly int mProjectionMatrixUniformLocation = -1;
    private readonly int mNormalMatrixUniformLocation     = -1;

    private Matrix4x4 mModelMatrix;
    private Matrix4x4 mViewMatrix;
    private Matrix4x4 mProjectionMatrix;
    private Matrix4x4 mNormalMatrix;

    protected OglDefaultProgram(
        string vertexShaderSource,
        string fragmentShaderSource)
        
        : base(vertexShaderSource, fragmentShaderSource)
    {
        try
        {
            mModelMatrixUniformLocation      = GetUniformLocation("uModelMatrix");
            mViewMatrixUniformLocation       = GetUniformLocation("uViewMatrix");
            mProjectionMatrixUniformLocation = GetUniformLocation("uProjectionMatrix");
            mNormalMatrixUniformLocation     = GetUniformLocation("uNormalMatrix");

            mModelMatrix      = Matrix4x4.Identity;
            mViewMatrix       = Matrix4x4.Identity;
            mProjectionMatrix = Matrix4x4.Identity;
            mNormalMatrix     = CalculateNormalMatrix();

            TrySetUniform(mModelMatrixUniformLocation     , mModelMatrix);
            TrySetUniform(mViewMatrixUniformLocation      , mViewMatrix);
            TrySetUniform(mProjectionMatrixUniformLocation, mProjectionMatrix);
            TrySetUniform(mNormalMatrixUniformLocation    , mNormalMatrix);
        }
        catch
        {
            Dispose(false);

            throw;
        }
    }

    public Matrix4x4 ModelMatrix
    {
        get => mModelMatrix;
        set
        {
            if (mModelMatrix == value)
            {
                return;
            }

            TrySetUniform(mModelMatrixUniformLocation, value);

            mModelMatrix = value;

            NormalMatrix = CalculateNormalMatrix();
        }
    }

    public Matrix4x4 ViewMatrix
    {
        get => mViewMatrix;
        set
        {
            if (mViewMatrix == value)
            {
                return;
            }

            TrySetUniform(mViewMatrixUniformLocation, value);

            mViewMatrix = value;

            NormalMatrix = CalculateNormalMatrix();
        }
    }

    public Matrix4x4 ProjectionMatrix
    {
        get => mProjectionMatrix;
        set
        {
            if (mProjectionMatrix == value)
            {
                return;
            }

            TrySetUniform(mProjectionMatrixUniformLocation, value);

            mProjectionMatrix = value;
        }
    }

    public Matrix4x4 NormalMatrix
    {
        get => mNormalMatrix;
        private set
        {
            if (mNormalMatrix == value)
            {
                return;
            }

            TrySetUniform(mNormalMatrixUniformLocation, value);

            mNormalMatrix = value;
        }
    }

    private Matrix4x4 CalculateNormalMatrix()
    {
        // Shader: mat4 normalMatrix = transpose(inverse(modelView));

        if (!Matrix4x4.Invert(mViewMatrix * mModelMatrix, out var result))
        {
            result = Matrix4x4.Identity;
        }

        return Matrix4x4.Transpose(result);
    }
}
