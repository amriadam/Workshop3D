using OpenTK.Graphics.OpenGL;

namespace WinFormsApp1;

public sealed class GLProgram 
    : DisposableObject
{
    public GLProgram(string vertexShaderSource, string fragmentShaderSource)
    {
        int id = GL.CreateProgram();
        GLUtils.AssertSuccess(GL.GetError());

        try
        {
            using var vertexShader   = new GLVertexShader(vertexShaderSource);
            using var fragmentShader = new GLFragmentShader(fragmentShaderSource);

            GL.AttachShader(id, vertexShader.ID);
            GLUtils.AssertSuccess(GL.GetError());

            GL.AttachShader(id, fragmentShader.ID);
            GLUtils.AssertSuccess(GL.GetError());

            GL.LinkProgram(id);
            GLUtils.AssertSuccess(GL.GetError());

            string infoLog = GL.GetProgramInfoLog(id);
            if (!string.IsNullOrEmpty(infoLog))
            {
                throw new Exception($"Shader Program Linking Error: {infoLog}");
            }

            ID = id;
        }
        catch
        {
            GL.DeleteProgram(id);
            throw;
        }
    }

    protected override void Dispose(bool _)
    {
        if (ID != 0)
        {
            if (GL.IsProgram(ID))
            {
                GL.DeleteProgram(ID);
            }

            ID = 0;
        }
    }

    public int ID { get; private set; }

    public void Use()
    {
        GL.UseProgram(ID);
        GLUtils.AssertSuccess(GL.GetError());
    }

    public int GetAttributeLocation(string name)
    {
        return GL.GetAttribLocation(ID, name);
    }

    public int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(ID, name);
    }
}
