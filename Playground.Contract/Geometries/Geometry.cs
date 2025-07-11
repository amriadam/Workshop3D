using Playground.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playground.Contract.Geometries
{
    public abstract class Geometry
        : DisposableObject
        , IGeometry
    {
        protected Geometry(IEnumerable<GeometryRange> ranges)
        {
            if (ranges is null)
            {
                throw new ArgumentNullException(nameof(ranges));
            }

            Ranges = ranges.ToList();
        }

        public IReadOnlyList<GeometryRange> Ranges { get; }

        public abstract void Render(IRenderContext context);
    }
}
