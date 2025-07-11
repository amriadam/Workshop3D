using System.Runtime.InteropServices;

namespace Playground.Essentials;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct Matrix4x4<T>
    where T : struct
{
    public T M11;
    public T M12;
    public T M13;
    public T M14;

    public T M21;
    public T M22;
    public T M23;
    public T M24;

    public T M31;
    public T M32;
    public T M33;
    public T M34;

    public T M41;
    public T M42;
    public T M43;
    public T M44;

    public Matrix4x4(
        T m11, T m12, T m13, T m14,
        T m21, T m22, T m23, T m24,
        T m31, T m32, T m33, T m34,
        T m41, T m42, T m43, T m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }
}
