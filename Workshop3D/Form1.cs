using OpenTK.GLControl;

using OpenTK.Graphics.OpenGL;

using OpenTK.Mathematics;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private readonly GLControl _surface;
    private readonly Vector3[] _data;

    private int _vertexBufferObject;
    private int _vertexArrayObject;

    private GLProgram? _shaderProgram;

    private Matrix4 _projectionMatrix;
    private Matrix4 _modelMatrix;

    private GLCamera _camera = new GLCamera
    {
        Position = new Vector3(5, 5, 5),
    };


    private bool _isPanning;
    private Point _startMousePosition;
    
    private readonly double[] _zoomList = [0.125, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0];
    private int _zoomIndex = 3;
    private Matrix4 _translation = Matrix4.Identity;
    private Matrix4 _scale = Matrix4.Identity;

    public Form1()
    {
        InitializeComponent();

        _surface = new GLControl();

        _surface.Dock = DockStyle.Fill;

        _surface.Paint += OnSurfacePaint;

        _surface.Resize += OnSurfaceResize; // Handle resizing for projection matrix

        _surface.Load += OnSurfaceLoad;

        _surface.MouseDown  += OnSurfaceMouseDown;
        _surface.MouseMove  += OnSurfaceMouseMove;
        _surface.MouseUp    += OnSurfaceMouseUp;
        _surface.MouseWheel += OnSurfaceMouseWheel;

        Controls.Add(_surface);

        // Example vertex data (moved slightly off origin for better visibility)

        _data = GenerateData();

    }

    private void OnSurfaceLoad(object? sender, EventArgs e)

    {

        GL.ClearColor(Color.CornflowerBlue);

        GL.Enable(EnableCap.DepthTest); // Enable depth testing for proper 3D rendering

        GL.Enable(EnableCap.ProgramPointSize);

        //GL.Enable(EnableCap.PointSmooth);


        // Generate and bind Vertex Array Object (VAO)

        _vertexArrayObject = GL.GenVertexArray();

        GL.BindVertexArray(_vertexArrayObject);

        // Create and populate Vertex Buffer Object (VBO)

        _vertexBufferObject = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

        // Convert Vector3 list to a float array for BufferData

        float[] vertices = new float[_data.Length * 3];

        for (int i = 0; i < _data.Length; i++)

        {

            vertices[i * 3] = _data[i].X;

            vertices[i * 3 + 1] = _data[i].Y;

            vertices[i * 3 + 2] = _data[i].Z;

        }

        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Create and compile shaders (modified to accept MVP matrix)

        string vertexShaderSource = @"

            #version 330 core

            layout (location = 0) in vec3 aPosition;

            uniform mat4 modelViewProjection;
 
            void main()
            {
                gl_Position = modelViewProjection * vec4(aPosition, 1.0);

                gl_PointSize = 3.0;
            }";

        string fragmentShaderSource = @"

            #version 330 core

            out vec4 FragColor;
 
            void main()
            {
                FragColor = vec4(1.0f, 0.0f, 0.0f, 1.0f);
            }";

        _shaderProgram = new GLProgram(vertexShaderSource, fragmentShaderSource);
        var positionAttributeLocation = _shaderProgram.GetAttributeLocation("aPosition");

        // Set up vertex attributes
        GL.EnableVertexAttribArray(positionAttributeLocation);
        GL.VertexAttribPointer(positionAttributeLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        // Unbind VAO and VBO
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Initial projection matrix setup (will be updated on resize)

        OnSurfaceResize(this, EventArgs.Empty);

        _shaderProgram.Use();

        // Create the model matrix (identity for now, as the points are defined in world space)
        _modelMatrix = Matrix4.Identity;

        // Calculate the model-view-projection matrix
        Matrix4 modelViewProjection = _modelMatrix * _camera.ViewMatrix * _projectionMatrix;

        // Get the location of the uniform variable in the shader

        int mvpLocation = _shaderProgram.GetUniformLocation("modelViewProjection");

        // Pass the MVP matrix to the shader

        GL.UniformMatrix4(mvpLocation, false, ref modelViewProjection);

        _surface.Invalidate();
    }

    private void OnSurfaceResize(object? sender, EventArgs e)
    {
        GL.Viewport(0, 0, _surface.Width, _surface.Height);

        if (_surface.Width == 0 || _surface.Height == 0)
        {
            return;
        }

        float aspectRatio = ((float)_surface.Width) / _surface.Height;

        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f), // Field of view in radians
            aspectRatio,
            0.1f,   // Near clipping plane
            100.0f  // Far clipping plane
        );
    }

    private void OnSurfacePaint(object? sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vertexArrayObject);
        {
            // Draw the vertices
            GL.DrawArrays(PrimitiveType.Points, 0, _data.Length);
        }
        GL.BindVertexArray(0);

        _surface.SwapBuffers();
    }


    private void OnSurfaceMouseDown(object? sender, MouseEventArgs e)
    {
        _isPanning = true;
        _startMousePosition = e.Location;
    }

    private void OnSurfaceMouseMove(object? sender, MouseEventArgs e)
    {
        if (_isPanning)
        {
            var oldPosition = new Vector3(_startMousePosition.X, _startMousePosition.Y, 0);
            var newPosition = new Vector3(e.Location.X, e.Location.Y, 0);


            var delta = newPosition - oldPosition;

            delta.X /= _surface.Width;
            delta.Y /= -_surface.Height;

            _translation = Matrix4.CreateTranslation(-delta);

            UpdateModelViewProjectionMatrix();
        }
    }

    private void OnSurfaceMouseUp(object? sender, MouseEventArgs e)
    {
        _isPanning = false;
    }

    private void OnSurfaceMouseWheel(object? sender, MouseEventArgs e)
    {
        _zoomIndex = Math.Clamp(_zoomIndex + Math.Sign(e.Delta), 0, _zoomList.Length - 1);

        double zoom = _zoomList[_zoomIndex];

        _scale = Matrix4.CreateScale((float)zoom);

        UpdateModelViewProjectionMatrix();
    }

    private void UpdateModelViewProjectionMatrix()
    {
        _modelMatrix *= _translation * _scale;

        // Calculate the model-view-projection matrix

        var modelViewProjection = _modelMatrix * _camera.ViewMatrix * _projectionMatrix;

        // Get the location of the uniform variable in the shader

        int mvpLocation = _shaderProgram.GetUniformLocation("modelViewProjection");

        // Pass the MVP matrix to the shader

        GL.UniformMatrix4(mvpLocation, false, ref modelViewProjection);

        _surface.Invalidate();
    }

    private static Vector3[] GenerateData()
    {
        double T = 10;

        double fs = 1_000_000;

        double dt = 1 / fs;

        int N = (int)(T * fs);

        var data = new Vector3[N];

        for (int i = 0; i < N; i++)
        {
            double t = i * dt;

            double rad = 2 * Math.PI * t;

            double x = Math.Cos(rad);

            double y = Math.Sin(rad);

            double z = -t;

            data[i] = new Vector3((float)x, (float)y, (float)z);
        }

        return data;
    }
}
