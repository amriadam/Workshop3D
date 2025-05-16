using OpenTK.Graphics.OpenGL;

namespace Workshop3D.Shaders;

public sealed class GLFragmentShader(string source)
    : GLShader(ShaderType.FragmentShader, source)
{
}
