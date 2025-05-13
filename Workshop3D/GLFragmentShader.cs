using OpenTK.Graphics.OpenGL;

namespace WinFormsApp1;

public sealed class GLFragmentShader(string source)
    : GLShader(ShaderType.FragmentShader, source)
{
}
