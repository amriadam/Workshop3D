using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Color4<T>
    where T : struct
{
    public T R;
    public T G;
    public T B;
    public T A;

    public Color4(T r, T g, T b, T a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}
