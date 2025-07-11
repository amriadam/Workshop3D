using OpenTK.Graphics.OpenGL4;

namespace Playground.Ogl.Internal;

internal sealed class OglShader
    : OglObject
{
    public OglShader(ShaderType shaderType, string shaderSource)
    {
        ArgumentException.ThrowIfNullOrEmpty(shaderSource);

        int handle = GL.CreateShader(shaderType);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;

            GL.ShaderSource(handle, shaderSource);
            OglUtils.AssertSucceeded(GL.GetError());

            GL.CompileShader(handle);
            OglUtils.AssertSucceeded(GL.GetError());

            GL.GetShader(handle, ShaderParameter.CompileStatus, out int status);
            OglUtils.AssertSucceeded(GL.GetError());

            if (status == 0)
            {
                throw new Exception($"Shader compilation of shader type '{Enum.GetName(shaderType)}' failed with message '{GL.GetShaderInfoLog(handle)}'.");
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

        GL.DeleteShader(handle);

        Handle = 0;
    }
}
