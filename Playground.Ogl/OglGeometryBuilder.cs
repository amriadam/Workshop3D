using OpenTK.Graphics.OpenGL4;
using Playground.Contract.Geometries;
using Playground.Essentials;
using Playground.Ogl.Internal;

namespace Playground.Ogl;

public sealed class OglGeometryBuilder
    : IGeometryBuilder
{
    private readonly List<GeometryRange>   mRanges    = [];
    private readonly List<Vector3<float>>  mPositions = [];
    private readonly List<Color3<float>>   mColors    = [];
    private readonly List<Vector3<float>>  mNormals   = [];
    private readonly List<int>             mIndices   = [];
    private PrimitiveType? mPrimitiveType;

    private void Clear()
    {
        mRanges.Clear();
        mPositions.Clear();
        mColors.Clear();
        mNormals.Clear();
        mIndices.Clear();
        mPrimitiveType = null;
    }

    public void BeginPointGeometry()
    {
        Clear();

        mPrimitiveType = PrimitiveType.Points;
    }

    public bool AddPoints(int name, Vector3<float>[] positions, Color3<float>[] colors)
    {
        ArgumentNullException.ThrowIfNull(positions);
        ArgumentNullException.ThrowIfNull(colors);

        if (mPrimitiveType != PrimitiveType.Points)
        {
            return false;
        }

        if (positions.Length < 1)
        {
            return false;
        }

        var range = new GeometryRange
        {
            Name = name,
            VertexStart = mPositions.Count,
            VertexCount = positions.Length,
            IndexStart = mPositions.Count,
            IndexCount = positions.Length,
        };

        mRanges.Add(range);
        mPositions.AddRange(positions);
        mColors.AddRange(colors);

        return true;
    }

    public void BeginLineGeometry()
    {
        Clear();

        mPrimitiveType = PrimitiveType.Lines;
    }

    public bool AddPolyline(int name, Vector3<float>[] positions, Color3<float>[] colors, bool closed)
    {
        ArgumentNullException.ThrowIfNull(positions);
        ArgumentNullException.ThrowIfNull(colors);

        if (mPrimitiveType != PrimitiveType.Lines)
        {
            return false;
        }

        if (positions.Length < 2)
        {
            return false;
        }

        int first = mPositions.Count;
        int count = positions.Length - 1;

        var range = new GeometryRange
        {
            Name = name,
            VertexStart = first,
            VertexCount = positions.Length,
            IndexStart = mIndices.Count,
            IndexCount = count * 2,
        };

        int index = first;
        while (count > 0)
        {
            mIndices.Add(index);
            mIndices.Add(index + 1);

            ++index;
            --count;
        }

        if (closed)
        {
            mIndices.Add(index);
            mIndices.Add(first);

            range.IndexCount += 2;
        }

        mRanges.Add(range);
        mPositions.AddRange(positions);
        mColors.AddRange(colors);

        return true;
    }

    public void BeginMeshGeometry()
    {
        Clear();

        mPrimitiveType = PrimitiveType.Triangles;
    }

    public bool AddMesh(int name, Vector3<float>[] positions, Color3<float>[] colors, Vector3<float>[] normals, int[] indices)
    {
        ArgumentNullException.ThrowIfNull(positions);
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(normals);
        ArgumentNullException.ThrowIfNull(indices);

        if (mPrimitiveType != PrimitiveType.Triangles)
        {
            return false;
        }

        if (positions.Length < 3)
        {
            return false;
        }

        var range = new GeometryRange
        {
            Name        = name,
            VertexStart = mPositions.Count,
            VertexCount = positions.Length,
            IndexStart  = mIndices.Count,
            IndexCount  = indices.Length,
        };
        
        mRanges.Add(range);
        mPositions.AddRange(positions);
        mColors.AddRange(colors);
        mNormals.AddRange(normals);
        mIndices.AddRange(indices);

        return true;
    }

    public IGeometry BuildGeometry()
    {
        var primitiveType = mPrimitiveType 
            ?? throw new InvalidOperationException();

        var geometry = new OglGeometry(primitiveType, mRanges, [.. mPositions], [.. mColors], [.. mNormals], [.. mIndices]);

        Clear();

        return geometry;
    }
}
