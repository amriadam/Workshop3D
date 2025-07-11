using Playground.Contract.Geometries;

namespace Playground.Contract
{
    public interface IHitTestResult
    {
        IGeometry     Geometry { get; }
        GeometryRange Range    { get; }
        int           Vertex   { get; }
    }
}
