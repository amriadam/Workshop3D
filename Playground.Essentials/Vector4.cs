using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Vector4<T> 
    where T : struct
{
    public T X;
    public T Y;
    public T Z;
    public T W;
    
    public Vector4(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}
