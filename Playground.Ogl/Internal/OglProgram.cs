using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace Playground.Ogl.Internal;

internal abstract class OglProgram
    : OglObject
{
    protected OglProgram(
        string vertexShaderSource, 
        string fragmentShaderSource)
    {
        using var vertexShader   = new OglShader(ShaderType.VertexShader  , vertexShaderSource);
        using var fragmentShader = new OglShader(ShaderType.FragmentShader, fragmentShaderSource);

        int handle = GL.CreateProgram();
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;

            GL.AttachShader(handle, vertexShader.Handle);
            OglUtils.AssertSucceeded(GL.GetError());

            GL.AttachShader(handle, fragmentShader.Handle);
            OglUtils.AssertSucceeded(GL.GetError());

            GL.LinkProgram(handle);
            OglUtils.AssertSucceeded(GL.GetError());

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int status);
            OglUtils.AssertSucceeded(GL.GetError());

            if (status == 0)
            {
                throw new Exception($"Program linking failed with message '{GL.GetProgramInfoLog(handle)}'.");
            }
        }
        catch
        {
            Dispose(false);

            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        int handle = Handle;

        if (handle == 0)
        {
            return;
        }

        GL.DeleteProgram(handle);

        Handle = 0;
    }

    public void Use()
    {
        GL.UseProgram(AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());
    }

    protected int GetUniformLocation(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        int location = GL.GetUniformLocation(AssertNotDisposed(), name);
        OglUtils.AssertSucceeded(GL.GetError());

        return location;
    }

    protected int GetAttributeLocation(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        int location = GL.GetAttribLocation(AssertNotDisposed(), name);
        OglUtils.AssertSucceeded(GL.GetError());

        return location;
    }

    protected bool TrySetUniform(int location, byte value)
    {
        if (location < 0)
        {
            return false;
        }
        
        GL.ProgramUniform1(AssertNotDisposed(), location, value);
        OglUtils.AssertSucceeded(GL.GetError());

        return true;
    }

    protected bool TrySetUniform(int location, int value)
    {
        if (location < 0)
        {
            return false;
        }

        GL.ProgramUniform1(AssertNotDisposed(), location, value);
        OglUtils.AssertSucceeded(GL.GetError());

        return true;
    }

    protected bool TrySetUniform(int location, float value)
    {
        if (location < 0)
        {
            return false;
        }

        GL.ProgramUniform1(AssertNotDisposed(), location, value);
        OglUtils.AssertSucceeded(GL.GetError());

        return true;
    }

    protected bool TrySetUniform(int location, Vector3 value)
    {
        if (location < 0)
        {
            return false;
        }

        GL.ProgramUniform3(AssertNotDisposed(), location, value.X, value.Y, value.Z);
        OglUtils.AssertSucceeded(GL.GetError());

        return true;
    }

    protected bool TrySetUniform(int location, Matrix4x4 value)
    {
        if (location < 0)
        {
            return false;
        }

        float[] m = 
        [
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44,
        ];

        GL.ProgramUniformMatrix4(AssertNotDisposed(), location, 1, false, m);
        OglUtils.AssertSucceeded(GL.GetError());

        return true;
    }
}
