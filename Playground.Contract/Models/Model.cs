using Playground.Essentials;
using System.Numerics;

namespace Playground.Contract.Models
{
    public abstract class Model
        : DisposableObject
        , IModel
    {
        /// <inheritdoc/>
        public Matrix4x4? Transform { get; set; }

        /// <inheritdoc/>
        public abstract void Render(IRenderContext context);
    }
}
