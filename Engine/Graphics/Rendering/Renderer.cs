using Silk.NET.OpenGL;
using System.Numerics;

namespace Engine;

public class Renderer
{
    private readonly GL _gl;

    private uint _vao;
    private uint _vbo;
    private uint _shaderProgram;

    public Renderer(GL gl)
    {
        _gl = gl;

        InitTriangleResources();
    }

    private unsafe void InitTriangleResources()
    {
        float[] vertices =
        {
            // x, y
            0f, 0f,
            1f, 0f,
            1f, 1f,
            0f, 1f
        };

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        unsafe
        {
            fixed (float* v = vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint)(vertices.Length * sizeof(float)),
                    v,
                    BufferUsageARB.StaticDraw);
            }
        }

        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);
        _gl.EnableVertexAttribArray(0);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindVertexArray(0);

        _shaderProgram = CreateShader();
    }

    public void Clear()
    {
        _gl.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    public unsafe void DrawRectangle(float x, float y, float width, float height)
    {
        _gl.UseProgram(_shaderProgram);

        Matrix4x4 model =
            Matrix4x4.CreateScale(width, height, 1f) *
            Matrix4x4.CreateTranslation(x, y, 0f);

        int modelLoc = _gl.GetUniformLocation(_shaderProgram, "uModel");
        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }

    private uint CreateShader()
    {
        string vertexShaderCode = @"
#version 330 core

layout (location = 0) in vec2 aPosition;

uniform mat4 uModel;

void main()
{
    gl_Position = uModel * vec4(aPosition, 0.0, 1.0);
}
";

        string fragmentShaderCode = @"
#version 330 core

out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}
";

        uint vertex = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertex, vertexShaderCode);
        _gl.CompileShader(vertex);

        uint fragment = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragment, fragmentShaderCode);
        _gl.CompileShader(fragment);

        uint program = _gl.CreateProgram();
        _gl.AttachShader(program, vertex);
        _gl.AttachShader(program, fragment);
        _gl.LinkProgram(program);

        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);

        return program;
    }
}