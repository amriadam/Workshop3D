using OpenTK.GLControl;

using OpenTK.Graphics.OpenGL;

using OpenTK.Mathematics;
using System.Diagnostics;
using Workshop3D;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private readonly GLControl _surface;
    private readonly GLCamera _camera;

    private readonly Vector3[] _data;

    private const float _zoomSensitivity  = 0.5f;
    private const float _minZoomDistance  = 1.0f;
    private const float _maxZoomDistance  = 10.0f;
    private const int   _zoomFactor       = 2;

    private Vector3     _panningVelocity = Vector3.Zero;
    private const float _panningAccelerationFactor = 0.001f; 
    private const float _panningDampingFactor = 0.85f;

    
    private GLProgram? _shaderProgram;
    private GLProgram? _axisShaderProgram;

    private int _vertexBufferObject;
    private int _vertexArrayObject;

    private Matrix4 _projectionMatrix;
    private Matrix4 _modelMatrix;

    private Point? _lastMousePosition;
    private bool _keyPressed;

    private const float _rotationSpeed = 0.01f;

    public Form1()
    {
        InitializeComponent();

        _surface = new GLControl
        {
            Dock = DockStyle.Fill
        };

        _camera = new GLCamera
        {
            Position = new Vector3(5, 5, 5),
            Target = new Vector3(0,0,0)
        };

        _surface.Load       += OnSurfaceLoad;
        _surface.Paint      += OnSurfacePaint;
        _surface.Resize     += OnSurfaceResize;
        _surface.MouseWheel += OnSurfaceMouseWheel;

        _surface.MouseDown  += OnSurfaceMouseDown;
        _surface.MouseMove  += OnSurfaceMouseMove;
        _surface.MouseUp    += OnSurfaceMouseUp;

        _data = GenerateData();

        Controls.Add(_surface);

        _surface.KeyDown += OnKeyDown;
        _surface.KeyUp += OnKeyUp;

        _surface.TabStop  = true;
        _surface.TabIndex = 0;
        _surface.Focus();
        _surface.Enabled = true;
        _surface.Visible= true;
        _surface.Capture = true;


        var timer = new System.Windows.Forms.Timer { Interval = 16 };
        timer.Tick += OnRotationTimerTick;
        timer.Start();
    }

    private void OnRotationTimerTick(object? sender, EventArgs e)
    {
        if (_keyPressed)
        {
            RotateCameraY(_rotationSpeed);
            UpdateModelViewProjectionMatrix();
        }
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
        {
            _keyPressed = true;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
        {
            _keyPressed = false;
        }
    }

    private void RotateCameraY(float angleRadians)
    {
        Vector3 currentPosition = _camera.Position;
        Vector3 target = _camera.Target;
        Vector3 offset = currentPosition - target;

        Matrix3 rotationMatrix = Matrix3.CreateRotationY(angleRadians);
        Vector3 rotatedOffset = offset * rotationMatrix;

        _camera.Position = target + rotatedOffset;
    }

    private void OnSurfaceLoad(object? sender, EventArgs e)
    {
        GL.ClearColor(Color.Beige);
        GL.Enable(EnableCap.DepthTest); 
        GL.Enable(EnableCap.ProgramPointSize);
        
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, 3 * _data.Length * sizeof(float), _data, BufferUsageHint.StaticDraw);

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

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        OnSurfaceResize(this, EventArgs.Empty);

        _shaderProgram.Use();

        _modelMatrix = Matrix4.Identity;

        Matrix4 modelViewProjection = _modelMatrix * _camera.ViewMatrix * _projectionMatrix;

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
            MathHelper.DegreesToRadians(45.0f),
            aspectRatio,
            0.1f, 
            100.0f
        );
    }

    private void OnSurfacePaint(object? sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vertexArrayObject);
        {
            GL.DrawArrays(PrimitiveType.Points, 0, _data.Length);
        }
        GL.BindVertexArray(0);

        _surface.SwapBuffers();
    }

    private void OnSurfaceMouseDown(object? sender, MouseEventArgs e)
    {
        _lastMousePosition = e.Location;
    }

    private void OnSurfaceMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_lastMousePosition.HasValue)
        {
            return;
        }

        var newPosition = e.Location;

        var delta = new Point(
            newPosition.X - _lastMousePosition.Value.X,
            newPosition.Y - _lastMousePosition.Value.Y);

        var viewMatrix = _camera.ViewMatrix;

        var rightVector = new Vector3(viewMatrix.M11, viewMatrix.M21, viewMatrix.M31);
        var upVector    = new Vector3(viewMatrix.M12, viewMatrix.M22, viewMatrix.M32);

        Vector3 acceleration = -rightVector * delta.X * _panningAccelerationFactor + upVector * delta.Y * _panningAccelerationFactor;

        _panningVelocity = (_panningVelocity + acceleration) * _panningDampingFactor;

        _camera.Position += _panningVelocity;

        UpdateModelViewProjectionMatrix();

        _lastMousePosition = newPosition;
    }

    private void OnSurfaceMouseUp(object? sender, MouseEventArgs e)
    {
        _lastMousePosition = null;
    }

    private void OnSurfaceMouseWheel(object? sender, MouseEventArgs e)
    {
        if (e.Delta == 0)
        {
            return;
        }

        Zoom(e.Delta > 0);
    }

    private void UpdateModelViewProjectionMatrix()
    {
        var modelViewProjection = _modelMatrix * _camera.ViewMatrix * _projectionMatrix;

        if (_shaderProgram is not null)
        {
            int mvpLocation = _shaderProgram.GetUniformLocation("modelViewProjection");

            GL.UniformMatrix4(mvpLocation, false, ref modelViewProjection);
        }

        _surface.Invalidate();
    }

    private void Zoom(bool zoomIn)
    {
        var lookDirection = Vector3.Normalize(_camera.Target - _camera.Position);

        float distanceToTarget = (_camera.Target - _camera.Position).Length;

        float desiredDistance = distanceToTarget / _zoomFactor;

        Vector3 zoomDelta = lookDirection * _zoomFactor * _zoomSensitivity;

        if (zoomIn)
        {
            if (desiredDistance > _minZoomDistance)
            {
                _camera.Position += zoomDelta;
                UpdateModelViewProjectionMatrix();
                return;
            }
        }
        else
        {
            if (desiredDistance < _maxZoomDistance)
            {
                _camera.Position -= zoomDelta;
                UpdateModelViewProjectionMatrix();
            }
        }
    }

    private static Vector3[] GenerateData()
    {
        double T = 10; // duration

        double fs = 1_000; // sampling frequency(how many samples per second)

        double dt = 1 / fs; // time interval of samples between 2 samples

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
