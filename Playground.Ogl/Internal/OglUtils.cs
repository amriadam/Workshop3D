using OpenTK.Graphics.OpenGL4;

namespace Playground.Ogl.Internal;

internal static class OglUtils
{
    public static void AssertSucceeded(ErrorCode error)
    {
        if (error != ErrorCode.NoError)
        {
            throw new Exception($"{Enum.GetName(error)}.");
        }
    }
}
