using Playground.Contract.Geometries;

namespace Playground.Contract.Models;

public sealed class MeshGeometryModel
    : GeometryModel
{
    public MeshGeometryModel(IGeometry geometry) 
        : base(geometry)
    {
        PolygonMode = MeshMode.Fill;
    }

    public MeshMode PolygonMode { get; set; }

    public override void Render(IRenderContext context)
    {
        using (context.EnableLight(true))
        using (context.SetPolygonMode(PolygonMode))
        {
            base.Render(context);
        }
    }
}
