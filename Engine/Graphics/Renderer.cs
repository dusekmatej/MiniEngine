using Shader =  Engine.Graphics.Shader;
using Silk.NET.OpenGL;
using System.Numerics;

namespace Engine.Graphics;

public class Renderer
{
    #region Shader sources
    private const string VertexShaderSource = @"
    #version 330 core

    layout (location = 0) in vec2 aPosition;

    uniform mat4 uModel;

    void main()
    {
        gl_Position =
            uModel *
            vec4(aPosition,0.0,1.0);
    }
    ";

    private const string FragmentShaderSource = @"
    #version 330 core

    out vec4 FragColor;

    void main()
    {
        FragColor =
            vec4(1.0,0.0,0.0,1.0);
    }
    ";
    #endregion

    private readonly GL _gl;

    private Mesh _quad;

    private Shader _shader;

    public Renderer(GL gl)
    {
        _gl = gl;

        CreateResources();
    }

    private unsafe void CreateResources()
    {
        float[] vertices =
        {
            // x, y
            0f, 0f,
            1f, 0f,
            1f, 1f,
            0f, 1f
        };

        _quad = new Mesh(_gl, vertices);

        _shader = new Shader(
            _gl,
            VertexShaderSource,
            FragmentShaderSource
        );
    }

    public void Clear()
    {
        _gl.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    public unsafe void DrawRectangle(float x, float y, float width, float height)
    {
        _shader.Use();

        Matrix4x4 model =
            Matrix4x4.CreateScale(width, height, 1f) *
            Matrix4x4.CreateTranslation(x, y, 0f);

        int modelLoc = _shader.GetUniformLocation("uModel");

        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        _quad.Bind();
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }
}