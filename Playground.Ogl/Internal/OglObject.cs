using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal abstract class OglObject
    : DisposableObject
{
    public int Handle { get; protected set; }

    protected int AssertNotDisposed()
    {
        int handle = Handle;

        ObjectDisposedException.ThrowIf(handle == 0, this);

        return handle;
    }
}
