using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Playground.Contract;
using Playground.Contract.Geometries;
using Playground.Contract.Models;

namespace Playground.Ogl.Internal;

internal sealed class OglPickingProgram
    : OglDefaultProgram
{
    private readonly OglFrameBuffer  mFrameBuffer;
    private readonly OglTexture      mColorBuffer;
    private readonly OglRenderBuffer mDepthBuffer;

    public OglPickingProgram(int width, int height)
        : base(OglShaderSourceCode.PickingVertexShader, OglShaderSourceCode.PickingFragmentShader)
    {
        try
        {
            ArgumentOutOfRangeException.ThrowIfNegative(width);
            ArgumentOutOfRangeException.ThrowIfNegative(height);

            OglFrameBuffer?  frameBuffer = null;
            OglTexture?      colorBuffer = null;
            OglRenderBuffer? depthBuffer = null;

            try
            {
                frameBuffer = new OglFrameBuffer();

                colorBuffer = new OglTexture(
                    TextureTarget.Texture2D, 
                    0, 
                    PixelInternalFormat.Rgba32i,
                    width, 
                    height,
                    0, 
                    PixelFormat.RgbaInteger,
                    PixelType.Int, 
                    nint.Zero);

                depthBuffer = new OglRenderBuffer(
                    RenderbufferStorage.Depth24Stencil8,
                    width, 
                    height);

                colorBuffer.MinFilter = TextureMinFilter.Nearest;
                colorBuffer.MagFilter = TextureMagFilter.Nearest;

                frameBuffer.Attach(FramebufferAttachment.ColorAttachment0, colorBuffer);
                frameBuffer.Attach(FramebufferAttachment.DepthAttachment , depthBuffer);

                mFrameBuffer = frameBuffer;
                mColorBuffer = colorBuffer;
                mDepthBuffer = depthBuffer;

                frameBuffer = null;
                colorBuffer = null;
                depthBuffer = null;
            }
            finally
            {
                frameBuffer?.Dispose();
                colorBuffer?.Dispose();
                depthBuffer?.Dispose();
            }
        }
        catch
        {
            base.Dispose(false);
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        mFrameBuffer?.Dispose();
        mColorBuffer?.Dispose();
        mDepthBuffer?.Dispose();

        base.Dispose(disposing);
    }

    public bool TryPickRange(IRenderContext context, IEnumerable<IGeometryModel> models, int left, int top, out GeometryRange result)
    {
        RenderModels(context, models);

        Vector4i pixel = PickPixel(left, top);

        int name = pixel.X;

        foreach (var model in models)
        {
            var geometry = model.Geometry;

            foreach (var range in geometry.Ranges)
            {
                if (range.Name == name)
                {
                    result = range;
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    private void RenderModels(IRenderContext context, IEnumerable<IGeometryModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.LineSmooth);
        GL.Disable(EnableCap.PolygonSmooth);

        mFrameBuffer.DrawBufferMode = DrawBufferMode.ColorAttachment0;
        try
        {
            using (mFrameBuffer.Bind())
            {
                GL.ClearColor(0, 0, 0, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                foreach (var model in models)
                {
                    model.Render(context);
                }
            }
        }
        finally
        {
            mFrameBuffer.DrawBufferMode = DrawBufferMode.None;
        }
    }

    private Vector4i PickPixel(int left, int top)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(left);
        ArgumentOutOfRangeException.ThrowIfNegative(top);

        int bottom = mColorBuffer.Height - 1 - top;

        Vector4i pixel = default;

        mFrameBuffer.ReadBufferMode = ReadBufferMode.ColorAttachment0;
        try
        {
            using (mFrameBuffer.Bind())
            {
                GL.ReadPixels(left, bottom, 1, 1, mColorBuffer.PixelFormat, mColorBuffer.PixelType, ref pixel);
                OglUtils.AssertSucceeded(GL.GetError());
            }
        }
        finally
        {
            mFrameBuffer.ReadBufferMode = ReadBufferMode.None;
        }

        return pixel;
    }
}
