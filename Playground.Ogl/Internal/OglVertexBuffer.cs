using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglVertexBuffer
    : OglBuffer
{
    public static OglVertexBuffer Create<T>(T[] vertices, int vertexSize)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vertexSize);

        using var pinPtr = new PinPtr<T[]>(vertices);

        return new OglVertexBuffer(pinPtr.Address, vertices.Length, vertexSize);
    }

    private OglVertexBuffer(
        nint vertexPtr,
        int  vertexCount,
        int  vertexSize)

        : base(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, MakeBufferSize(vertexCount, vertexSize), vertexPtr)
    {
        VertexCount = vertexCount;
        VertexSize  = vertexSize;
    }

    public int VertexCount { get; }
    public int VertexSize  { get; }

    private static int MakeBufferSize(int vertexCount, int vertexSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(vertexCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vertexSize);

        return checked(vertexCount * vertexSize);
    }
}
