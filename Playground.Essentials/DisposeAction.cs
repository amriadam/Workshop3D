namespace Playground.Essentials;

public sealed class DisposeAction
    : DisposableObject
{
    private Action? mAction;

    public DisposeAction(Action action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        mAction = action;
    }

    protected override void Dispose(bool disposing)
    {
        mAction?.Invoke();
        mAction = null;
    }
}
