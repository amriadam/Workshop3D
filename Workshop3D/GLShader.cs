using OpenTK.Graphics.OpenGL;

namespace WinFormsApp1;

public abstract class GLShader
    : DisposableObject
{
    protected GLShader(ShaderType type, string source)
    {
        int id = GL.CreateShader(type);
        GLUtils.AssertSuccess(GL.GetError());

        try
        {
            GL.ShaderSource(id, source);
            GLUtils.AssertSuccess(GL.GetError());

            GL.CompileShader(id);
            GLUtils.AssertSuccess(GL.GetError());

            string infoLog = GL.GetShaderInfoLog(id);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception($"Shader Compilation Error: {infoLog}");
            }

            ID = id;
        }
        catch
        {
            GL.DeleteShader(id);
            throw;
        }
    }

    protected override void Dispose(bool _)
    {
        if (ID != 0)
        {
            if (GL.IsShader(ID))
            {
                GL.DeleteShader(ID);
            }

            ID = 0;
        }
    }

    public int ID { get; private set; }
}
