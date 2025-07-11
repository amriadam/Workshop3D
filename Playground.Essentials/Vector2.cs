using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Vector2<T> 
    where T : struct
{
    public T X;
    public T Y;

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }
}
