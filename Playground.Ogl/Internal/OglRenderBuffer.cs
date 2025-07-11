using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglRenderBuffer
    : OglObject
{
    public OglRenderBuffer(RenderbufferStorage format, int width, int height)
        : this(RenderbufferTarget.Renderbuffer, format, width, height)
    {
    }

    public OglRenderBuffer(RenderbufferTarget target, RenderbufferStorage format, int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        Target = target;
        Format = format;
        Width  = width;
        Height = height;
    
        GL.CreateRenderbuffers(1, out int handle);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;

            using (Bind())
            {
                GL.RenderbufferStorage(target, format, width, height);
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

        GL.DeleteFramebuffers(1, ref handle);

        Handle = 0;
    }

    public RenderbufferTarget  Target { get; }
    public RenderbufferStorage Format { get; }
    public int Width  { get; }
    public int Height { get; }

    public IDisposable Bind()
    {
        GL.BindRenderbuffer(Target, AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());

        return new DisposeAction(Unbind);
    }

    private void Unbind()
    {
        if (Handle == 0)
        {
            return;
        }

        GL.BindRenderbuffer(Target, 0);
    }
}
