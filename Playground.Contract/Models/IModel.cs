using System.Numerics;

namespace Playground.Contract.Models;

public interface IModel
    : IDisposable
{
    Matrix4x4? Transform { get; set; }

    void Render(IRenderContext context);
}
