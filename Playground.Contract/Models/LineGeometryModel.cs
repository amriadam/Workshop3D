using Playground.Contract.Geometries;

namespace Playground.Contract.Models;

/// <summary>
/// Realizes the <see cref="ILineGeometryModel"/> class.
/// </summary>
public class LineGeometryModel
    : GeometryModel
    , ILineGeometryModel
{
    public LineGeometryModel(IGeometry geometry)
        : base(geometry)
    {
        LineWidth = 1;
    }

    public float LineWidth { get; set; }

    public override void Render(IRenderContext context)
    {
        using (context.EnableLight(false))
        using (context.SetLineWidth(LineWidth))
        {
            base.Render(context);
        }
    }
}
