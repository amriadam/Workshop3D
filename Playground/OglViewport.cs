using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Playground.Contract.Camera;
using Playground.Contract.Models;
using Playground.Ogl;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Forms.Integration;

namespace Playground;

public class OglViewport
    : WindowsFormsHost
{
    public static readonly DependencyProperty CameraProperty 
        = DependencyProperty.Register(
            nameof(Camera),
            typeof(IProjectionCamera),
            typeof(OglViewport),
            new PropertyMetadata(default(IProjectionCamera), OnCameraChanged));

    public static readonly DependencyProperty ModelsProperty
        = DependencyProperty.Register(
            nameof(Models),
            typeof(ObservableCollection<IGeometryModel>),
            typeof(OglViewport),
            new PropertyMetadata(default(ObservableCollection<IGeometryModel>), OnModelsChanged));

    private OglRenderContext? mRenderContext;

    public OglViewport()
    {
        Models = [];

        DataContextChanged += OnDataContextChanged;
    }

    public IProjectionCamera Camera
    {
        get => (IProjectionCamera)GetValue(CameraProperty); 
        set => SetValue(CameraProperty, value);
    }

    public ObservableCollection<IGeometryModel> Models
    {
        get => (ObservableCollection<IGeometryModel>)GetValue(ModelsProperty);
        set => SetValue(ModelsProperty, value);
    }


    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is not null)
        {
            if (Child is not GLControl surface)
            {
                return;
            }

            Child = null;

            surface.Load   -= OnSurfaceLoad;
            surface.Resize -= OnSurfaceResize;
            surface.Paint  -= OnSurfacePaint;

            surface.Dispose();

            mRenderContext?.Dispose();
            mRenderContext = null;
        }

        if (e.NewValue is not null)
        {
            GL.LoadBindings(new GLFWBindingsContext());

            var surface = new GLControl(new GLControlSettings
            {
                APIVersion = new Version(4, 6),
                API        = ContextAPI.OpenGL,
                Flags      = ContextFlags.ForwardCompatible,
                Profile    = ContextProfile.Compatability,
            });

            surface.Load   += OnSurfaceLoad;
            surface.Resize += OnSurfaceResize;
            surface.Paint  += OnSurfacePaint;

            Child = surface;
        }
    }

    private void OnSurfaceLoad(object? sender, EventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        surface.MakeCurrent();

        mRenderContext = new OglRenderContext
        {
            ClearColor = new Essentials.Color4<float>(1f, 1f, 1f, 1f),
        };
    }

    private void OnSurfaceResize(object? sender, EventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        surface.MakeCurrent();

        GL.Viewport(surface.Size);

        if (Camera is IPerspectiveCamera camera)
        {
            camera.AspectRatio = surface.AspectRatio;
        }
    }

    private void OnSurfacePaint(object? sender, PaintEventArgs e)
    {
        if (sender is not GLControl surface)
        {
            return;
        }

        surface.MakeCurrent();

        var models  = Models;
        var context = mRenderContext;

        if (models is not null && context is not null)
        {
            context.ClearBuffers(color: true, depth: true, stencil: false);

            foreach (var model in models)
            {
                model.Render(context);
            }
        }

        GL.Finish();

        surface.SwapBuffers();
    }

    private static void OnCameraChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not OglViewport instance)
        {
            return;
        }

        if (e.OldValue is IProjectionCamera oldCamera)
        {
            oldCamera.ViewMatrixChanged       -= instance.OnCameraMatrixChanged;
            oldCamera.ProjectionMatrixChanged -= instance.OnCameraMatrixChanged;
        }

        if (e.NewValue is IProjectionCamera newCamera)
        {
            newCamera.ViewMatrixChanged       += instance.OnCameraMatrixChanged;
            newCamera.ProjectionMatrixChanged += instance.OnCameraMatrixChanged;

            var context = instance.mRenderContext;
            if (context is null)
            {
                return;
            }

            context.ViewMatrix       = newCamera.ViewMatrix;
            context.ProjectionMatrix = newCamera.ProjectionMatrix;
        }

        instance.InvalidateSurface();
    }

    private void OnCameraMatrixChanged(object? sender, EventArgs e)
    {
        if (sender is not IProjectionCamera camera)
        {
            return;
        }

        var context = mRenderContext;
        if (context is null)
        {
            return;
        }

        context.ViewMatrix       = camera.ViewMatrix;
        context.ProjectionMatrix = camera.ProjectionMatrix;

        InvalidateSurface();
    }

    private static void OnModelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not OglViewport instance)
        {
            return;
        }

        if (e.OldValue is ObservableCollection<IGeometryModel> oldList)
        {
            oldList.CollectionChanged -= instance.OnModelsCollectionChanged;
        }

        if (e.NewValue is ObservableCollection<IGeometryModel> newList)
        {
            newList.CollectionChanged += instance.OnModelsCollectionChanged;
        }

        instance.InvalidateSurface();
    }

    private void OnModelsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    private void InvalidateSurface()
    {
        if (Child is GLControl surface)
        {
            surface.Invalidate();
        }
    }
}
