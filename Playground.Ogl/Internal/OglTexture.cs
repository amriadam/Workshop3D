using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglTexture
    : OglObject
{
    private TextureMinFilter mMinFilter = TextureMinFilter.Linear;
    private TextureMagFilter mMagFilter = TextureMagFilter.Linear;
    private TextureWrapMode  mWrapModeS = TextureWrapMode.ClampToEdge;
    private TextureWrapMode  mWrapModeT = TextureWrapMode.ClampToEdge;

    public OglTexture(
        TextureTarget       target, 
        int                 level, 
        PixelInternalFormat internalPixelFormat, 
        int                 width, 
        int                 border, 
        PixelFormat         pixelFormat, 
        PixelType           pixelType,
        nint                pixels)

        : this(target, level, internalPixelFormat, width, 1, 1, border, pixelFormat, pixelType, pixels)
    {
    }

    public OglTexture(
        TextureTarget       target, 
        int                 level, 
        PixelInternalFormat internalPixelFormat, 
        int                 width, 
        int                 height,
        int                 border, 
        PixelFormat         pixelFormat, 
        PixelType           pixelType,
        nint                pixels)

        : this(target, level, internalPixelFormat, width, height, 1, border, pixelFormat, pixelType, pixels)
    {
    }

    public OglTexture(
        TextureTarget       target, 
        int                 level, 
        PixelInternalFormat internalPixelFormat, 
        int                 width, 
        int                 height, 
        int                 depth, 
        int                 border, 
        PixelFormat         pixelFormat, 
        PixelType           pixelType,
        nint                pixels)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);
        ArgumentOutOfRangeException.ThrowIfNegative(depth);
        ArgumentOutOfRangeException.ThrowIfNegative(border);

        Target              = target;
        Level               = level;
        InternalPixelFormat = internalPixelFormat;
        Width               = width;
        Height              = height;
        Depth               = depth;
        Border              = border;
        PixelFormat         = pixelFormat;
        PixelType           = pixelType;

        GL.CreateTextures(target, 1, out int handle);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;
        
            using (Bind())
            {
                GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)mMinFilter);
                OglUtils.AssertSucceeded(GL.GetError());

                GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)mMagFilter);
                OglUtils.AssertSucceeded(GL.GetError());

                GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)mWrapModeS);
                OglUtils.AssertSucceeded(GL.GetError());

                GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)mWrapModeT);
                OglUtils.AssertSucceeded(GL.GetError());

                switch (target)
                {
                    case TextureTarget.Texture1D:
                        GL.TexImage1D(target, level, internalPixelFormat, width, border, pixelFormat, pixelType, pixels);
                        OglUtils.AssertSucceeded(GL.GetError());
                        break;
                    case TextureTarget.Texture2D:
                        GL.TexImage2D(target, level, internalPixelFormat, width, height, border, pixelFormat, pixelType, pixels);
                        OglUtils.AssertSucceeded(GL.GetError());
                        break;
                    case TextureTarget.Texture3D:
                        GL.TexImage3D(target, level, internalPixelFormat, width, height, depth, border, pixelFormat, pixelType, pixels);
                        OglUtils.AssertSucceeded(GL.GetError());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(target), $"{Enum.GetName(target)}");
                }
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

        GL.DeleteTextures(1, ref handle);

        Handle = 0;
    }

    public TextureTarget       Target { get; }
    public int                 Level { get; }
    public PixelInternalFormat InternalPixelFormat { get; }
    public int                 Width { get; }
    public int                 Height { get; }
    public int                 Depth { get; }
    public int                 Border { get; }
    public PixelFormat         PixelFormat { get; }
    public PixelType           PixelType { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public TextureMinFilter MinFilter
    {
        get => mMinFilter;
        set
        {
            using (Bind())
            {
                GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)value);
                OglUtils.AssertSucceeded(GL.GetError());

                mMinFilter = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public TextureMagFilter MagFilter
    {
        get => mMagFilter;
        set
        {
            using (Bind())
            {
                GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)value);
                OglUtils.AssertSucceeded(GL.GetError());

                mMagFilter = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public TextureWrapMode WrapModeS
    {
        get => mWrapModeS;
        set
        {
            using (Bind())
            {
                GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)value);
                OglUtils.AssertSucceeded(GL.GetError());

                mWrapModeS = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public TextureWrapMode WrapModeT
    {
        get => mWrapModeT;
        set
        {
            using (Bind())
            {
                GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)value);
                OglUtils.AssertSucceeded(GL.GetError());

                mWrapModeT = value;
            }
        }
    }

    public IDisposable Bind()
    {
        GL.BindTexture(Target, AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());

        return new DisposeAction(Unbind);
    }

    private void Unbind()
    {
        if (Handle == 0)
        {
            return;
        }

        GL.BindTexture(Target, 0);
    }
}
