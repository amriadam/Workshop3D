using Playground.Contract.Models;
using Playground.Essentials;
using System.Numerics;

namespace Playground.Contract;

public interface IRenderContext
{
    Matrix4x4 ViewMatrix { get; set; }
    Matrix4x4 ProjectionMatrix { get; set; }
    Matrix4x4 ModelMatrix { get; set; }

    float PointSize { get; set; }
    float LineWidth { get; set; }

    bool IsLightEnabled { get; set; }
    bool IsPointSmoothEnabled { get; set; }
    bool IsLineSmoothEnabled { get; set; }

    Color4<float> ClearColor { get; set; }
    
    MeshMode PolygonMode { get; set; }

    void ClearBuffers(bool color, bool depth, bool stencil);
}
