using OpenTK.Graphics.OpenGL;

namespace Workshop3D.Common;

public static class GLUtils
{
    public static void AssertSuccess(ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"{Enum.GetName(errorCode)}");
        }
    }
}
