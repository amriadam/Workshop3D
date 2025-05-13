using OpenTK.Graphics.OpenGL;

namespace WinFormsApp1;

public sealed class GLVertexShader(string source)
    : GLShader(ShaderType.VertexShader, source)
{
}
