using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal abstract class OglBuffer
    : OglObject
{
    protected OglBuffer(BufferTarget target, BufferUsageHint usageHint, int size, nint ptr)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);

        Target = target;
        Size   = size;

        GL.GenBuffers(1, out int handle);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;

            using (Bind())
            {
                GL.BufferData(target, size, ptr, usageHint);
                OglUtils.AssertSucceeded(GL.GetError());
            }
        }
        catch
        {
            Dispose(false);

            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        int handle = Handle;

        if (handle == 0)
        {
            return;
        }

        GL.DeleteBuffers(1, ref handle);

        Handle = 0;
    }

    public BufferTarget  Target { get; }
    public int           Size   { get; }


    public IDisposable Bind()
    {
        GL.BindBuffer(Target, AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());

        return new DisposeAction(Unbind);
    }

    private void Unbind()
    {
        if (Handle == 0)
        {
            return;
        }

        GL.BindBuffer(Target, 0);
    }
}
