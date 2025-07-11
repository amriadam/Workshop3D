namespace Playground.Contract.Models;

public sealed class MeshMode
{
    public static MeshMode Fill { get; } 
        = new MeshMode();

    public static MeshMode Point { get; }
        = new MeshMode();

    public static MeshMode Line { get; }
    = new MeshMode();

    private MeshMode()
    {
    }
}
