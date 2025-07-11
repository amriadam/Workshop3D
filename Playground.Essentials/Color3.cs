using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Color3<T>
    where T : struct
{
    public T R;
    public T G;
    public T B;

    public Color3(T r, T g, T b)
    {
        R = r;
        G = g;
        B = b;
    }
}
