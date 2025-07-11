using Playground.Contract.Geometries;

namespace Playground.Contract.Models;

/// <summary>
/// Realizes the <see cref="IGeometryModel"/> interface.
/// </summary>
public abstract class GeometryModel
    : Model
    , IGeometryModel
{
    /// <summary>
    /// Initializes a new instance the <see cref="GeometryModel"/> class.
    /// </summary>
    /// <param name="geometry">
    /// The geometry.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="geometry"/> is <c>null</c>.
    /// </exception>
    protected GeometryModel(IGeometry geometry) 
    {
        if (geometry is null)
        {
            throw new ArgumentNullException(nameof(geometry));
        }

        Geometry = geometry;
    }

    protected override void Dispose(bool disposing)
    {
        Geometry?.Dispose();
    }

    public IGeometry Geometry { get; }

    public override void Render(IRenderContext context)
    {
        using (ApplyTransform(context))
        {
            Geometry.Render(context);
        }
    }

    private IDisposable? ApplyTransform(IRenderContext context)
    {
        var transform = Transform;

        return transform.HasValue
            ? context.AppendModelMatrix(transform.Value) 
            : null;
    }
}
