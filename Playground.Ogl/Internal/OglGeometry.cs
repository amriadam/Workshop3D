using OpenTK.Graphics.OpenGL4;
using Playground.Contract;
using Playground.Contract.Geometries;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglGeometry 
    : Geometry
{
    private readonly PrimitiveType    mPrimitiveType;
    private readonly OglVertexArray   mVertexArray;
    private readonly OglVertexBuffer  mPositionBuffer;
    private readonly OglVertexBuffer  mColorBuffer;
    private readonly OglVertexBuffer? mNormalBuffer;
    private readonly OglIndexBuffer?  mIndexBuffer;

    public OglGeometry(
        PrimitiveType              primitiveType,
        IEnumerable<GeometryRange> ranges,
        Vector3<float>[]           positions,
        Color3<float>[]            colors,
        Vector3<float>[]           normals,
        int[]                      indices)

        : base(ranges)
    {
        ArgumentNullException.ThrowIfNull(positions);
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(normals);
        ArgumentNullException.ThrowIfNull(indices);

        mPrimitiveType = primitiveType;

        var positionAttribute = OglVertexAttributeDescriptor.Position;
        var colorAttribute    = OglVertexAttributeDescriptor.Color;
        var normalAttribute   = OglVertexAttributeDescriptor.Normal;

        OglVertexArray?  vertexArray    = null;
        OglVertexBuffer? positionBuffer = null;
        OglVertexBuffer? colorBuffer    = null;
        OglVertexBuffer? normalBuffer   = null;
        OglIndexBuffer?  indexBuffer    = null;
        try
        {
            vertexArray = new OglVertexArray();

            positionBuffer = OglVertexBuffer.Create(
                positions, positionAttribute.Size);

            colorBuffer = OglVertexBuffer.Create(
                colors, colorAttribute.Size);

            if (normals.Length > 0)
            {
                normalBuffer = OglVertexBuffer.Create(
                    normals, normalAttribute.Size);
            }

            if (indices.Length > 0)
            {
                indexBuffer = OglIndexBuffer.Create(
                    indices);
            }

            vertexArray.Link(
                positionBuffer,
                positionAttribute.Size,
                positionAttribute);

            vertexArray.Link(
                colorBuffer,
                colorAttribute.Size,
                colorAttribute);

            if (normalBuffer is not null)
            {
                vertexArray.Link(
                    normalBuffer,
                    normalAttribute.Size,
                    normalAttribute);
            }

            if (indexBuffer is not null)
            {
                // todo: Link.
            }

            mVertexArray    = vertexArray;
            mPositionBuffer = positionBuffer;
            mColorBuffer    = colorBuffer;
            mNormalBuffer   = normalBuffer;
            mIndexBuffer    = indexBuffer;

            vertexArray    = null;
            positionBuffer = null; 
            colorBuffer    = null;
            normalBuffer   = null;
            indexBuffer    = null;
        }
        finally
        {
            vertexArray?.Dispose();
            positionBuffer?.Dispose();
            colorBuffer?.Dispose();
            normalBuffer?.Dispose();
            indexBuffer?.Dispose();
        }
    }

    protected override void Dispose(bool disposing)
    {
        mVertexArray?.Dispose();
        mPositionBuffer?.Dispose();
        mColorBuffer?.Dispose();
        mNormalBuffer?.Dispose();
        mIndexBuffer?.Dispose();
    }

    public override void Render(IRenderContext context)
    {
        if (mIndexBuffer is not null)
        {
            mVertexArray.Draw(mPrimitiveType, mIndexBuffer);
        }
        else
        {
            mVertexArray.Draw(mPrimitiveType, mPositionBuffer.VertexCount);
        }
    }
}
