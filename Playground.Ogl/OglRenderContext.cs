using OpenTK.Graphics.OpenGL4;
using Playground.Contract;
using Playground.Essentials;
using Playground.Ogl.Internal;
using System.Numerics;

namespace Playground.Ogl;

public sealed class OglRenderContext
    : DisposableObject
    , IRenderContext
{
    private readonly OglRenderingProgram mRenderingProgram;

    private float mPointSize;
    private float mLineWidth;

    private bool mPointSmoothEnabled;
    private bool mLineSmoothEnabled;
    private Color4<float> mClearColor;
    private Contract.Models.MeshMode mPolygonMode;

    public OglRenderContext()
    {
        var renderingProgram = new OglRenderingProgram();
        
        try
        {
            renderingProgram.Use();

            mPointSize = 1;
            GL.PointSize(mPointSize);

            mLineWidth = 1;
            GL.LineWidth(mLineWidth);

            mPointSmoothEnabled = false;
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Fastest);

            mLineSmoothEnabled = false;
            GL.Disable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);

            mClearColor = new Color4<float>(1f, 0f, 0f, 1f);
            GL.ClearColor(mClearColor.R, mClearColor.G, mClearColor.B, mClearColor.A);

            mPolygonMode = Contract.Models.MeshMode.Fill;
            GL.PolygonMode(TriangleFace.FrontAndBack, OpenTK.Graphics.OpenGL4.PolygonMode.Fill);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            mRenderingProgram = renderingProgram;
            renderingProgram  = null;
        }
        finally
        {
            renderingProgram?.Dispose();
        }
    }

    protected override void Dispose(bool disposing)
    {
        mRenderingProgram?.Dispose();
    }

    public Matrix4x4 ViewMatrix
    {
        get => mRenderingProgram.ViewMatrix;
        set => mRenderingProgram.ViewMatrix = value;
    }

    public Matrix4x4 ProjectionMatrix
    {
        get => mRenderingProgram.ProjectionMatrix;
        set => mRenderingProgram.ProjectionMatrix = value;
    }

    public Matrix4x4 ModelMatrix
    {
        get => mRenderingProgram.ModelMatrix;
        set => mRenderingProgram.ModelMatrix = value;
    }

    public float PointSize 
    {
        get => mPointSize;
        set
        {
            GL.PointSize(value);
            mPointSize = value;
        }
    }
    
    public float LineWidth 
    {
        get => mLineWidth;
        set
        {
            GL.LineWidth(value);
            mLineWidth = value;
        }
    }
    
    public bool IsLightEnabled
    {
        get => mRenderingProgram.LightMode != 0;
        set => mRenderingProgram.LightMode = value ? 1 : 0;
    }

    public bool IsPointSmoothEnabled
    {
        get => mPointSmoothEnabled;
        set
        {
            GL.Hint(HintTarget.PointSmoothHint, value ? HintMode.Nicest : HintMode.Fastest);
            mPointSmoothEnabled = value;
        }
    }

    public bool IsLineSmoothEnabled
    {
        get => mLineSmoothEnabled;
        set
        {
            if (value)
            {
                GL.Enable(EnableCap.LineSmooth);
            }
            else
            {
                GL.Disable(EnableCap.LineSmooth);
            }

            GL.Hint(HintTarget.LineSmoothHint, value ? HintMode.Nicest : HintMode.Fastest);
            
            mLineSmoothEnabled = value;
        }
    }

    public Color4<float> ClearColor
    {
        get => mClearColor;
        set
        {
            GL.ClearColor(value.R, value.G, value.B, value.A);
            mClearColor = value;
        }
    }

    public Contract.Models.MeshMode PolygonMode
    {
        get => mPolygonMode;
        set
        {
            OpenTK.Graphics.OpenGL4.PolygonMode polygonMode;

            if (value == Contract.Models.MeshMode.Fill)
            {
                polygonMode = OpenTK.Graphics.OpenGL4.PolygonMode.Fill;
            }
            else
            if (value == Contract.Models.MeshMode.Line)
            {
                polygonMode = OpenTK.Graphics.OpenGL4.PolygonMode.Line;
            }
            else
            if (value == Contract.Models.MeshMode.Point)
            {
                polygonMode = OpenTK.Graphics.OpenGL4.PolygonMode.Point;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            GL.PolygonMode(TriangleFace.FrontAndBack, polygonMode);

            mPolygonMode = value;
        }
    }

    public void ClearBuffers(bool color, bool depth, bool stencil)
    {
        ClearBufferMask mask = ClearBufferMask.None;

        if (color)
        {
            mask |= ClearBufferMask.ColorBufferBit;
        }

        if (depth)
        {
            mask |= ClearBufferMask.DepthBufferBit;
        }

        if (stencil)
        {
            mask |= ClearBufferMask.StencilBufferBit;
        }

        GL.Clear(mask);
    }
}
