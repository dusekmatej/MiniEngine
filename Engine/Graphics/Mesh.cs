using Silk.NET.OpenGL;

namespace Engine.Graphics;

public class Mesh
{
    private readonly GL _gl;
    
    public uint VertexArray;
    public readonly uint _vertexBuffer;
    public int VertexCount;
    
    public unsafe Mesh(GL gl, float[] vertices)
    {
        _gl = gl;

        VertexCount = vertices.Length / 4;

        VertexArray = _gl.GenVertexArray();
        _vertexBuffer = _gl.GenBuffer();

        _gl.BindVertexArray(VertexArray);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);

        fixed (float* data = vertices)
        {
            _gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                data,
                BufferUsageARB.StaticDraw
                );
        }

        _gl.VertexAttribPointer(
            0, 2,
            VertexAttribPointerType.Float,
            false,
            4 * sizeof(float),
            (void*)0);

        _gl.EnableVertexAttribArray(0);

        _gl.VertexAttribPointer(
            1, 2,
            VertexAttribPointerType.Float,
            false,
            4 * sizeof(float),
            (void*)(2 * sizeof(float)));

        _gl.EnableVertexAttribArray(1);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindVertexArray(0);
    }


    public void Bind()
    {
        _gl.BindVertexArray(VertexArray);
    }
}