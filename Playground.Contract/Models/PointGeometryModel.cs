using Playground.Contract.Geometries;

namespace Playground.Contract.Models;

public class PointGeometryModel
    : GeometryModel
    , IPointGeometryModel
{
    public PointGeometryModel(IGeometry geometry) 
        : base(geometry)
    {
        PointSize = 1;
    }

    public float PointSize { get; set; }

    public override void Render(IRenderContext context)
    {
        using (context.EnableLight(false))
        using (context.SetPointSize(PointSize))
        {
            base.Render(context);
        }
    }
}
