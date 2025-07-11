using System.Runtime.InteropServices;

namespace Playground.Essentials;

public sealed class PinPtr<T>
    : DisposableObject
{
    private readonly GCHandle mHandle;

    public PinPtr(T target)
    {
        mHandle = GCHandle.Alloc(target, GCHandleType.Pinned);
    }

    protected override void Dispose(bool disposing)
    {
        if (mHandle.IsAllocated)
        {
            mHandle.Free();
        }
    }

    public T Target => (T)mHandle.Target;

    public IntPtr Address => mHandle.AddrOfPinnedObject();
}
