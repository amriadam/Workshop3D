using Playground.Contract.Geometries;

namespace Playground.Contract.Models
{
    public interface IGeometryModel
        : IModel
    {
        IGeometry Geometry { get; }
    }
}
