using OpenTK.Graphics.OpenGL4;

namespace Playground.Ogl.Internal;

internal sealed class OglVertexAttributeDescriptor
{
    public static OglVertexAttributeDescriptor Position { get; } 
        = new OglVertexAttributeDescriptor(
            location   : 0,
            dimension  : 3,
            pointerType: VertexAttribPointerType.Float,
            normalized : false,
            offset     : 0);

    public static OglVertexAttributeDescriptor Color { get; }
        = new OglVertexAttributeDescriptor(
            location    : 1,
            dimension   : 3,
            pointerType : VertexAttribPointerType.Float,
            normalized  : false,
            offset      : 0);

    public static OglVertexAttributeDescriptor Normal { get; }
        = new OglVertexAttributeDescriptor(
            location    : 2,
            dimension   : 3,
            pointerType : VertexAttribPointerType.Float,
            normalized  : false,
            offset      : 0);

    public static OglVertexAttributeDescriptor Name { get; }
        = new OglVertexAttributeDescriptor(
            location    : 3,
            dimension   : 1,
            integerType : VertexAttribIntegerType.Int);

    public OglVertexAttributeDescriptor(
        int                     location,
        int                     dimension,
        VertexAttribPointerType pointerType,
        bool                    normalized,
        int                     offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(location);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dimension);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        Location    = location;
        Dimension   = dimension;
        Size        = dimension * MakeDimensionSize(pointerType);
        PointerType = pointerType;
        Normalized  = normalized;
        Offset      = offset;
    }

    public OglVertexAttributeDescriptor(
        int                     location,
        int                     dimension,
        VertexAttribIntegerType integerType)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(location);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dimension);

        Location    = location;
        Dimension   = dimension;
        Size        = dimension * MakeDimensionSize(integerType);
        IntegerType = integerType;
    }

    public OglVertexAttributeDescriptor(
        int                    location,
        int                    dimension,
        VertexAttribDoubleType doubleType)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(location);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dimension);

        Location   = location;
        Dimension  = dimension;
        Size       = dimension * MakeDimensionSize(doubleType);
        DoubleType = doubleType;
    }

    public int Location  { get; }
    public int Dimension { get; }
    public int Size      { get; }
    public VertexAttribPointerType? PointerType { get; }
    public VertexAttribIntegerType? IntegerType { get; }
    public VertexAttribDoubleType?  DoubleType  { get; }

    public bool Normalized { get; }
    public int  Offset     { get; }

    private static int MakeDimensionSize(VertexAttribPointerType pointerType)
    {
        return pointerType switch
        {
            VertexAttribPointerType.Float         => sizeof(float),
            VertexAttribPointerType.Double        => sizeof(double),
            VertexAttribPointerType.Byte          => sizeof(sbyte),
            VertexAttribPointerType.UnsignedByte  => sizeof(byte),
            VertexAttribPointerType.Short         => sizeof(short),
            VertexAttribPointerType.UnsignedShort => sizeof(ushort),
            VertexAttribPointerType.Int           => sizeof(int),
            VertexAttribPointerType.UnsignedInt   => sizeof(uint),
            _ => throw new ArgumentOutOfRangeException(nameof(pointerType), $"{Enum.GetName(pointerType)}"),
        };
    }

    private static int MakeDimensionSize(VertexAttribIntegerType integerType)
    {
        return integerType switch
        {
            VertexAttribIntegerType.Byte          => sizeof(sbyte),
            VertexAttribIntegerType.UnsignedByte  => sizeof(byte),
            VertexAttribIntegerType.Short         => sizeof(short),
            VertexAttribIntegerType.UnsignedShort => sizeof(ushort),
            VertexAttribIntegerType.Int           => sizeof(int),
            VertexAttribIntegerType.UnsignedInt   => sizeof(uint),
            _ => throw new ArgumentOutOfRangeException(nameof(integerType), $"{Enum.GetName(integerType)}"),
        };
    }

    private static int MakeDimensionSize(VertexAttribDoubleType doubleType)
    {
        return doubleType switch
        {
            VertexAttribDoubleType.Double => sizeof(double),
            _ => throw new ArgumentOutOfRangeException(nameof(doubleType), $"{Enum.GetName(doubleType)}"),
        };
    }
}
