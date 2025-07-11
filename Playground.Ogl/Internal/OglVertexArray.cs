using OpenTK.Graphics.OpenGL4;
using Playground.Essentials;

namespace Playground.Ogl.Internal;

internal sealed class OglVertexArray
    : OglObject
{
    public OglVertexArray()
    {
        GL.GenVertexArrays(1, out int handle);
        OglUtils.AssertSucceeded(GL.GetError());

        try
        {
            Handle = handle;
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

        GL.DeleteVertexArrays(1, ref handle);

        Handle = 0;
    }

    public IDisposable Bind()
    {
        GL.BindVertexArray(AssertNotDisposed());
        OglUtils.AssertSucceeded(GL.GetError());

        return new DisposeAction(Unbind);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void Link(
        OglVertexBuffer              vertexBuffer,
        int                          vertexSize,
        OglVertexAttributeDescriptor attributeDescriptor)
    {
        ArgumentNullException.ThrowIfNull(vertexBuffer);
        ArgumentOutOfRangeException.ThrowIfNegative(vertexSize);
        ArgumentNullException.ThrowIfNull(attributeDescriptor);

        using (Bind())
        using (vertexBuffer.Bind())
        {
            if (attributeDescriptor.PointerType is not null 
             && attributeDescriptor.IntegerType is null 
             && attributeDescriptor.DoubleType  is null)
            {
                GL.VertexAttribPointer(
                    attributeDescriptor.Location,
                    attributeDescriptor.Dimension,
                    attributeDescriptor.PointerType.Value,
                    attributeDescriptor.Normalized,
                    vertexSize,
                    attributeDescriptor.Offset);
                OglUtils.AssertSucceeded(GL.GetError());
            }
            else
            if (attributeDescriptor.PointerType is null
             && attributeDescriptor.IntegerType is not null
             && attributeDescriptor.DoubleType  is null)
            {
                GL.VertexAttribIPointer(
                    attributeDescriptor.Location, 
                    attributeDescriptor.Dimension, 
                    attributeDescriptor.IntegerType.Value,
                    vertexSize,
                    nint.Zero);
                OglUtils.AssertSucceeded(GL.GetError());
            }
            else
            if (attributeDescriptor.PointerType is null
             && attributeDescriptor.IntegerType is null
             && attributeDescriptor.DoubleType  is not null)
            {
                GL.VertexAttribLPointer(
                    attributeDescriptor.Location,
                    attributeDescriptor.Dimension,
                    attributeDescriptor.DoubleType.Value,
                    vertexSize,
                    nint.Zero);
                OglUtils.AssertSucceeded(GL.GetError());
            }
            else
            {
                throw new NotSupportedException("Unsupported vertex attribute.");
            }

            GL.EnableVertexAttribArray(attributeDescriptor.Location);
            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void EnableAttributePointer(OglVertexAttributeDescriptor attributeDescriptor, bool enable)
    {
        ArgumentNullException.ThrowIfNull(attributeDescriptor);

        using (Bind())
        {
            if (enable)
            {
                GL.EnableVertexAttribArray(attributeDescriptor.Location);
            }
            else
            {
                GL.DisableVertexAttribArray(attributeDescriptor.Location);
            }

            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void Draw(PrimitiveType primitiveType, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        using (Bind())
        {
            GL.DrawArrays(primitiveType, 0, count);
            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Binding.
    /// </remarks>
    public void Draw(PrimitiveType primitiveType, OglIndexBuffer indexBuffer)
    {
        ArgumentNullException.ThrowIfNull(indexBuffer);

        using (Bind())
        using (indexBuffer.Bind())
        {
            GL.DrawElements(primitiveType, indexBuffer.IndexCount, indexBuffer.IndexType, nint.Zero);
            OglUtils.AssertSucceeded(GL.GetError());
        }
    }

    private void Unbind()
    {
        if (Handle == 0)
        {
            return;
        }

        GL.BindVertexArray(0);
    }
}
