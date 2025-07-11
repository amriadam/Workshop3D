using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglFrameBuffer
    : OglObject
{
    private ReadBufferMode mReadBufferMode = ReadBufferMode.None;
    private DrawBufferMode mDrawBufferMode = DrawBufferMode.None;

    public OglFrameBuffer()
        : this(FramebufferTarget.Framebuffer)
    {
    }

    private OglFrameBuffer(FramebufferTarget target)
    {
        Target = target;

        GL.GenFramebuffers(1, out int handle);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;
        
            using (Bind())
            {
                GL.ReadBuffer(mReadBufferMode);
                OglUtils.AssertSucceeded(GL.GetError());

                GL.DrawBuffer(mDrawBufferMode);
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

    public FramebufferTarget Target { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public ReadBufferMode ReadBufferMode
    {
        get => mReadBufferMode;
        set
        {
            using (Bind())
            {
                GL.ReadBuffer( value);
                OglUtils.AssertSucceeded(GL.GetError());

                mReadBufferMode = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public DrawBufferMode DrawBufferMode
    {
        get => mDrawBufferMode;
        set
        {
            using (Bind())
            {
                GL.DrawBuffer(value);
                OglUtils.AssertSucceeded(GL.GetError());

                mDrawBufferMode = value;
            }
        }
    }

    public IDisposable Bind()
    {
        GL.BindFramebuffer(Target, AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());

        return new DisposeAction(Unbind);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void Attach(FramebufferAttachment attachmentTarget, OglTexture texture)
    {
        ArgumentNullException.ThrowIfNull(texture);

        using (Bind())
        {
            GL.FramebufferTexture(
                Target,
                attachmentTarget,
                texture.Handle,
                texture.Level);
            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void Attach(FramebufferAttachment attachmentTarget, OglRenderBuffer renderBuffer)
    {
        ArgumentNullException.ThrowIfNull(renderBuffer);

        using (Bind())
        {
            GL.FramebufferRenderbuffer(
                Target,
                attachmentTarget,
                renderBuffer.Target,
                renderBuffer.Handle);
            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    private void Unbind()
    {
        if (Handle == 0)
        {
            return;
        }

        GL.BindFramebuffer(Target, 0);
    }
}
