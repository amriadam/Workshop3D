using OpenTK.Graphics.OpenGL;

namespace Workshop3D.Shaders;

public sealed class GLVertexShader(string source)
    : GLShader(ShaderType.VertexShader, source)
{
}
