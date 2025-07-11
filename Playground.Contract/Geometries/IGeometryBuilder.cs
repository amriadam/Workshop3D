using Playground.Essentials;

namespace Playground.Contract.Geometries;

public interface IGeometryBuilder
{
    void BeginPointGeometry();
    bool AddPoints(int name, Vector3<float>[] positions, Color3<float>[] colors);


    void BeginLineGeometry();
    bool AddPolyline(int name, Vector3<float>[] positions, Color3<float>[] colors, bool closed);


    void BeginMeshGeometry();
    bool AddMesh(int name, Vector3<float>[] positions, Color3<float>[] colors, Vector3<float>[] normals, int[] indices);

    IGeometry BuildGeometry();
}
