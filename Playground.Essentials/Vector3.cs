using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Vector3<T>
    where T : struct
{
    public T X;
    public T Y;
    public T Z;

    public Vector3(T x, T y, T z)
    {
        X = x; 
        Y = y; 
        Z = z;
    }
}
