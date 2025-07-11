using System;
using System.Collections.Generic;

namespace Playground.Contract.Geometries
{
    public interface IGeometry
        : IDisposable
    {
        IReadOnlyList<GeometryRange> Ranges { get; }

        void Render(IRenderContext context);
    }
}
