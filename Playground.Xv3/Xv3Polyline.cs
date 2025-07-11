using Playground.Essentials;

namespace Playground.Xv3;

public sealed class Xv3Polyline
    : Xv3Entity
{
    public List<Vector3<float>> Points { get; } 
        = [];
}
