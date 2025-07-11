using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Playground.Contract;
using Playground.Ogl;
using Playground.Ogl.Internal;
using Playground.Stl;
using Playground.Xv3;
using System.Diagnostics;
using System.Drawing;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private const float MovementSpeed = 0.5f;  // [distance]
    private const float RotationSpeed = 1.0f;  // [angle in deg]

    private readonly Label         mInfo;
    private readonly GLControl     mSurface;

    private OglRenderingProgram?      mRendering;
    private OglPickingProgram?        mPicking;
    
    private OglPerspectiveCamera?     mCamera;
    
    private readonly List<OglGeometryModel> mModelList = [];

    public Form1()
    {
        InitializeComponent();

        var settings = new GLControlSettings 
        { 
            API        = ContextAPI.OpenGL, 
            APIVersion = new Version(4, 6), 
            Flags      = ContextFlags.ForwardCompatible,
            Profile    = ContextProfile.Compatability 
        };

        mSurface = new GLControl(settings)
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.LightSkyBlue,
        };

        mInfo = new Label
        {
            Width     = 200,
            Height    = 30,
            Font      = new Font("Arial", 12),
            BackColor = Color.Black,
            ForeColor = Color.Red,
            Visible   = true,
        };

        mSurface.Load += OnSurfaceLoad;

        Controls.Add(mSurface);
        Controls.Add(mInfo);

        mInfo.BringToFront();
    }

    private void OnSurfaceLoad(object? sender, EventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        foreach (var name in Enum.GetValues<StringName>())
        {
            string value = GL.GetString(name);

            Debug.WriteLine($"'{name}':'{value}'");
        }

        surface.Load -= OnSurfaceLoad;

        surface.Resize     += OnSurfaceResize;
        surface.Paint      += OnSurfacePaint;
        surface.MouseWheel += OnSurfaceMouseWheel;
        surface.MouseMove  += OnSurfaceMouseMove;
        surface.KeyPress   += OnSurfaceKeyPress;

        GL.ClearColor(surface.BackColor);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.DepthFunc(DepthFunction.Less);
        GL.CullFace(TriangleFace.Back);

        mRendering = new OglRenderingProgram();
        mPicking   = new OglPickingProgram(surface.Width, surface.Height);

        mCamera = new OglPerspectiveCamera
        {
            AspectRatio = surface.AspectRatio,
        };

        mRendering.ViewMatrix       
            = mCamera.ViewMatrix;

        mRendering.ProjectionMatrix 
            = mCamera.ProjectionMatrix;

        mPicking.ViewMatrix 
            = mCamera.ViewMatrix;

        mPicking.ProjectionMatrix
            = mCamera.ProjectionMatrix;

        mCamera.ViewMatrixChanged       += OnCameraMatrixChanged;
        mCamera.ProjectionMatrixChanged += OnCameraMatrixChanged;

        mModelList.Add(LoadXv3GeometryModel("complettetire.xv3"));
        //mModelList.Add(new PointGeometryModel(new PointGeometry([new Vector3(0, 0, 0)], [Color4.Red]), 10));
        //mModelList.Add(LoadStlGeometryModel("sponge.STL", new Color4(0x7F, 0x7F, 0x7F, 0xFF)));
    }

    private void OnSurfaceResize(object? sender, EventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        GL.Viewport(0, 0, surface.Width, surface.Height);

        var camera = mCamera;

        if (camera is not null)
        {
            camera.AspectRatio = surface.AspectRatio;
        }

        mPicking?.Dispose();
        mPicking = new OglPickingProgram(surface.Width, surface.Height);

        surface.Focus();
        surface.Invalidate();
    }

    private void OnSurfacePaint(object? sender, PaintEventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.LineSmooth);

        GL.ClearColor(surface.BackColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var program = mRendering;

        if (program is null)
        {
            return;
        }

        program.Use();
        program.LightMode = 0;

        foreach (var model in mModelList)
        {
            model.Render();
        }

        surface.SwapBuffers();

        mInfo.Text = $"{stopwatch.Elapsed.TotalMicroseconds:F0} µs";
    }

    private void OnSurfaceMouseWheel(object? sender, MouseEventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        var camera = mCamera;
        if (camera is null)
        {
            return;
        }

        surface.Invalidate();
    }

    private void OnSurfaceMouseMove(object? sender, MouseEventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        Point mousePosition = e.Location;

        if (mPicking is not null)
        {
            mPicking.Use();

            if (mPicking.TryPickRange(mModelList, mousePosition.X, mousePosition.Y, out var range))
            {
                Debug.WriteLine($"({DateTime.UtcNow.Ticks}):{mousePosition} -> {range.Name}");
            }

            mRendering?.Use();
        }

        surface.Invalidate();
    }

    private void OnSurfaceKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        var program = mRendering;
        if (program is null)
        {
            return;
        }

        Matrix4 modelMatrix;

        switch (e.KeyChar)
        {
            case 'w':
                modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotationSpeed));
                break;
            case 's':
                modelMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-RotationSpeed));
                break;
            case 'a':
                modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotationSpeed));
                break;
            case 'd':
                modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-RotationSpeed));
                break;
            case 'e':
                modelMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotationSpeed));
                break;
            case 'q':
                modelMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-RotationSpeed));
                break;
            default:
                return;
        }

        program.ModelMatrix *= modelMatrix;

        surface.Invalidate();
    }

    private void OnCameraMatrixChanged(object? sender, EventArgs e)
    {
        if (sender is not OglProjectionCamera camera)
        {
            return;
        }

        OglDefaultProgram?[] programs = 
        [
            mRendering,
            mPicking,
        ];

        foreach (var program in programs)
        {
            if (program is null)
            {
                continue;
            }

            program.ViewMatrix       = camera.ViewMatrix;
            program.ProjectionMatrix = camera.ProjectionMatrix;
        }
    }

    private static OglLineGeometryModel LoadXv3GeometryModel(string fileName)
    {
        const string folderPath = @"C:\\Users\\tino.kreutzberger\\Desktop\\HelixToolkitWorkshop\\HelixToolkitWorkshop\\bin\\Debug\\net8.0-windows\\Exchange";

        var document = Xv3Document.LoadFromXmlFile(Path.Combine(folderPath, fileName));

        List<Xv3Group> groups = [];
        document.Flat(groups, null, null);

        var builder = new OglLineGeometryBuilder();

        var palette = MakeHsvPalette();
        Shuffle(palette, new Random());

        int groupName = 0;

        foreach (var grouping in groups.GroupBy(group => group.Name[..3]))
        {
            int color = palette[groupName].ToArgb();

            foreach (var group in grouping)
            {
                foreach (var entity in group.Children)
                {
                    if (entity is Xv3Polyline polyline)
                    {
                        var points = polyline.Points;

                        builder.AddPolyline(int.MaxValue, [.. points.Select(p => new Vector3(p.X, p.Y, p.Z))], color, false);
                    }
                }
            }

            groupName = (groupName + 1) % palette.Length;
        }

        var geometry = builder.BuildGeometry();

        try
        {
            var model = new OglLineGeometryModel(geometry, 1);

            geometry = null;

            return model;
        }
        finally
        {
            geometry?.Dispose();
        }
    }

    private static MeshGeometryModel LoadStlGeometryModel(string fileName, int name, Color color)
    {
        const string folderPath = @"C:\\Users\\tino.kreutzberger\\Desktop\\HelixToolkitWorkshop\\HelixToolkitWorkshop\\bin\\Debug\\net8.0-windows\\Exchange";

        var solid = StlReader.LoadSolidFromFile(Path.Combine(folderPath, fileName));

        var builder = new OglGeometryBuilder();

        builder.AddSolid(name, solid, color.ToArgb());

        var geometry = builder.BuildGeometry();

        try
        {
            var model = new OglMeshGeometryModel(geometry);

            geometry = null;

            return model;
        }
        finally
        {
            geometry?.Dispose();
        }
    }

    private static Color[] MakeHsvPalette()
    {
        return 
        [
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(255, 64, 0),
            Color.FromArgb(255, 128, 0),
            Color.FromArgb(255, 192, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(192, 255, 0),
            Color.FromArgb(128, 255, 0),
            Color.FromArgb(64, 255, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(0, 255, 64),
            Color.FromArgb(0, 255, 128),
            Color.FromArgb(0, 255, 192),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 192, 255),
            Color.FromArgb(0, 128, 255),
            Color.FromArgb(0, 64, 255),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(64, 0, 255),
            Color.FromArgb(128, 0, 255),
            Color.FromArgb(192, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(255, 0, 192),
            Color.FromArgb(255, 0, 128),
            Color.FromArgb(255, 0, 64),
            Color.FromArgb(128, 0, 0),
            Color.FromArgb(128, 128, 0),
            Color.FromArgb(0, 128, 0),
            Color.FromArgb(0, 0, 128),
            Color.FromArgb(128, 0, 128),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(0, 0, 0),
        ];
    }

    private static void Shuffle<T>(T[] values, Random random)
    {
        int[] bins = new int[values.Length];
        for (int i = 0; i < bins.Length; ++i)
        {
            bins[i] = random.Next();
        }

        bins = [.. bins.Select((value, index) => (value, index)).OrderBy(a => a.value).Select(a => a.index)];

        T[] shuffle = [.. values];
        for (int i = 0; i < values.Length; ++i)
        {
            values[i] = shuffle[bins[i]];
        }
    }
}
