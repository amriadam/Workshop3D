using Playground.Contract.Camera;
using Playground.Contract.Models;
using Playground.Essentials;
using System.Numerics;

namespace Playground.Contract;

public static class Extensions
{
    public static void ZoomIn(this ICamera instance, float value)
    {
        Forward(instance, value);
    }

    public static void ZoomOut(this ICamera instance, float value)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        Forward(instance, -value);
    }

    public static void Forward(this ICamera instance, float value)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var position      = instance.Position;
        var lookDirection = instance.LookDirection;

        position += lookDirection * value;

        instance.Position = position;
    }

    public static IDisposable AppendModelMatrix(this IRenderContext instance, Matrix4x4 newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.ModelMatrix;

        instance.ModelMatrix *= newValue;

        return new DisposeAction(() => instance.ModelMatrix = oldValue);
    }

    public static IDisposable SetModelMatrix(this IRenderContext instance, Matrix4x4 newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.ModelMatrix;

        instance.ModelMatrix = newValue;

        return new DisposeAction(() => instance.ModelMatrix = oldValue);
    }

    public static IDisposable SetPointSize(this IRenderContext instance, float newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.PointSize;
        
        instance.PointSize = newValue;

        return new DisposeAction(() => instance.PointSize = oldValue);
    }

    public static IDisposable SetLineWidth(this IRenderContext instance, float newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.LineWidth;

        instance.LineWidth = newValue;

        return new DisposeAction(() => instance.LineWidth = oldValue);
    }

    public static IDisposable EnablPointSmooth(this IRenderContext instance, bool newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.IsPointSmoothEnabled;

        instance.IsPointSmoothEnabled = newValue;

        return new DisposeAction(() => instance.IsPointSmoothEnabled = oldValue);
    }

    public static IDisposable EnableLineSmooth(this IRenderContext instance, bool newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.IsLineSmoothEnabled;

        instance.IsLineSmoothEnabled = newValue;

        return new DisposeAction(() => instance.IsLineSmoothEnabled = oldValue);
    }

    public static IDisposable EnableLight(this IRenderContext instance, bool newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.IsLightEnabled;

        instance.IsLightEnabled = newValue;

        return new DisposeAction(() => instance.IsLightEnabled = oldValue);
    }

    public static IDisposable SetClearColor(this IRenderContext instance, Color4<float> newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.ClearColor;

        instance.ClearColor = newValue;

        return new DisposeAction(() => instance.ClearColor = oldValue);
    }

    public static IDisposable SetPolygonMode(this IRenderContext instance, MeshMode newValue)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var oldValue = instance.PolygonMode;

        instance.PolygonMode = newValue;

        return new DisposeAction(() => instance.PolygonMode = oldValue);
    }
}
