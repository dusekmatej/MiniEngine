using Shader =  MiniEngine.Graphics.Shader;
using Silk.NET.OpenGL;
using System.Numerics;
using MiniEngine.Core;

namespace MiniEngine.Graphics;

public class Renderer
{
    #region Shader sources
    private const string VertexShaderSource = @"
    #version 330 core

    layout(location = 0) in vec2 aPosition;
    layout(location = 1) in vec2 aTexCoord;

    out vec2 TexCoord;

    uniform mat4 uModel;

    void main()
    {
        gl_Position =
            uModel *
            vec4(aPosition,0,1);

        TexCoord = aTexCoord;
    }
    ";

    private const string FragmentShaderSource = @"
    #version 330 core

    out vec4 FragColor;

    in vec2 TexCoord;

    uniform sampler2D uTexture;

    void main()
    {
        FragColor =
            texture(uTexture, TexCoord);
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
            // x, y, u, v
            0f, 0f, 0f, 0f,
            1f, 0f, 1f, 0f,
            1f, 1f, 1f, 1f,
            0f, 1f, 0f, 1f
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

    public Texture CreateTexture(ImageData image)
    {
        return new Texture(_gl, image);
    }

    public unsafe void DrawTexture(Texture texture, float x, float y, float width, float height)
    {
        _shader.Use();

        Matrix4x4 model =
            Matrix4x4.CreateScale(width, height, 1f) *
            Matrix4x4.CreateTranslation(x, y, 0f);

        int modelLoc = _shader.GetUniformLocation("uModel");

        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        texture.Bind();
        _quad.Bind();
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }
}