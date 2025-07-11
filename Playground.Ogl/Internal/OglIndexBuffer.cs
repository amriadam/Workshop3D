using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglIndexBuffer
    : OglBuffer
{
    public static OglIndexBuffer Create(int[] indices)
    {
        return Create(indices, DrawElementsType.UnsignedInt);
    }

    public static OglIndexBuffer Create(int start, int count)
    {
        return Create([.. Enumerable.Range(start, count)]);
    }

    private OglIndexBuffer(int indexCount, DrawElementsType indexType, nint indexPtr)
        : this(indexCount, MakeIndexSize(indexType), indexType, indexPtr)
    {
    }

    private OglIndexBuffer(int indexCount, int indexSize, DrawElementsType indexType, nint indexPtr)
        : base(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw, MakeBufferSize(indexCount, indexSize), indexPtr)
    {
        IndexCount = indexCount;
        IndexSize  = indexSize;
        IndexType  = indexType;
    }

    public int              IndexCount { get; }
    public int              IndexSize  { get; }
    public DrawElementsType IndexType  { get; }

    private static OglIndexBuffer Create<T>(T[] indices, DrawElementsType indexType)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(indices);

        using var pinPtr = new PinPtr<T[]>(indices);

        return new OglIndexBuffer(indices.Length, indexType, pinPtr.Address);
    }

    private static int MakeBufferSize(int indexCount, int indexSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(indexCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(indexSize);

        return checked(indexCount * indexSize);
    }

    private static int MakeIndexSize(DrawElementsType indexType)
    {
        return indexType switch
        {
            DrawElementsType.UnsignedByte  => sizeof(byte),
            DrawElementsType.UnsignedShort => sizeof(ushort),
            DrawElementsType.UnsignedInt   => sizeof(uint),
            _ => throw new ArgumentOutOfRangeException(nameof(indexType), $"{Enum.GetName(indexType)}"),
        };
    }
}
