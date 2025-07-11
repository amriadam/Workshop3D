using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Playground.Ogl.Internal;
using System.Windows.Forms;

namespace Playground.Ogl;

/*
public sealed class Surface 
    : GLControl
{
    private readonly List<OglGeometryModel> mModels = [];
    private OglProgram?               mProgram;
    private OglPerspectiveCamera?           mCamera;

    public Surface()
        : base(new GLControlSettings 
        { 
            APIVersion = new Version(4, 6),
            Flags      = ContextFlags.ForwardCompatible,
            Profile    = ContextProfile.Compatability,
        })
    {
    }

    public void AddModel(OglGeometryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        mModels.Add(model);
    }

    public bool RemoveModel(OglGeometryModel model)
    {
        return mModels.Remove(model);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GL.ClearColor(BackColor);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        mProgram = new OglProgram();
        mCamera  = new OglPerspectiveCamera();

        mProgram.ViewMatrix       = mCamera.ViewMatrix;
        mProgram.ProjectionMatrix = mCamera.ProjectionMatrix;

        mCamera.ViewMatrixChanged       += OnCameraViewMatrixChanged;
        mCamera.ProjectionMatrixChanged += OnCameraProjectionMatrixChanged;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Width, Height);

        var camera = mCamera;
        if (camera is not null)
        {
            camera.AspectRatio = AspectRatio;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

        mProgram?.Use();

        foreach (var model in mModels)
        {
            model.Render();
        }

        SwapBuffers();
    }

    private void OnCameraViewMatrixChanged(object? sender, EventArgs e)
    {
        var camera = mCamera;
        if (camera is null)
        {
            return;
        }

        var program = mProgram;
        if (program is null)
        {
            return;
        }

        program.ViewMatrix = camera.ViewMatrix;

        Invalidate();
    }

    private void OnCameraProjectionMatrixChanged(object? sender, EventArgs e)
    {
        var camera = mCamera;
        if (camera is null)
        {
            return;
        }

        var program = mProgram;
        if (program is null)
        {
            return;
        }

        program.ProjectionMatrix = camera.ProjectionMatrix;

        Invalidate();
    }
}
*/