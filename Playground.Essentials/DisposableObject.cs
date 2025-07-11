namespace Playground.Essentials;

public abstract class DisposableObject
    : IDisposable
{
    ~DisposableObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void Dispose(bool disposing);
}
