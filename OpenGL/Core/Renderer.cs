using Silk.NET.OpenGL;
using System.Numerics;

using MiniEngine.Core;

using MiniEngine.Graphics;

using Shader = MiniEngine.OpenGL.Shaders.Shader;
using Texture = MiniEngine.OpenGL.Textures.Texture;
using Mesh = MiniEngine.OpenGL.Meshes.Mesh;


namespace MiniEngine.OpenGL.Core;

public class Renderer : IGraphicsBackend
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
    private const string TextureFragmentShaderSource = @"
    #version 330 core

    out vec4 FragColor;

    in vec2 TexCoord;

    uniform sampler2D uTexture;
    uniform vec4 uTint;

    void main()
    {
        FragColor = texture(uTexture, TexCoord) * uTint;
    }
    ";
    private const string ColorFragmentShaderSource = @"
    #version 330 core
    
    out vec4 FragColor;
    
    uniform vec4 uColor;
    
    void main()
    {
        FragColor = uColor;
    }
    ";
    private const string TextFragmentShaderSource = @"
    #version 330 core

    out vec4 FragColor;

    in vec2 TexCoord;

    uniform sampler2D uTexture;
    uniform vec4 uColor;

    void main()
    {
        float alpha = texture(uTexture, TexCoord).a;

        FragColor = vec4(
            uColor.rgb,
            uColor.a * alpha
        );
    }
    ";
    #endregion

    private readonly GL _gl;
    private readonly int _viewportWidth;
    private readonly int _viewportHeight;
    private Mesh _quad = null!;
    private uint _shapeVertexArray;
    private uint _shapeVertexBuffer;
    private Shader _textureShader = null!;
    private Shader _colorShader = null!;
    private Shader _textShader = null!;

    private readonly Dictionary<int, Texture> _textures = new();
    private int _nextTextureHandle = 1;

    public Renderer(GL gl, int viewportWidth, int viewportHeight)
    {
        _gl = gl;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;

        CreateResources();
    }

    public void BeginFrame()
    {
        _gl.Viewport(0, 0, (uint)_viewportWidth, (uint)_viewportHeight);
    }

    private unsafe void CreateResources()
    {
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        float[] vertices =
        {
            // x, y, u, v
            0f, 0f, 0f, 1f,
            1f, 0f, 1f, 1f,
            1f, 1f, 1f, 0f,
            0f, 1f, 0f, 0f
        };

        _quad = new Mesh(_gl, vertices);

        _shapeVertexArray = _gl.GenVertexArray();
        _shapeVertexBuffer = _gl.GenBuffer();

        _gl.BindVertexArray(_shapeVertexArray);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _shapeVertexBuffer);
        _gl.VertexAttribPointer(
            0,
            2,
            VertexAttribPointerType.Float,
            false,
            2 * sizeof(float),
            (void*)0
        );
        _gl.EnableVertexAttribArray(0);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindVertexArray(0);

        _textureShader = new Shader(_gl, VertexShaderSource, TextureFragmentShaderSource);
        _colorShader = new Shader(_gl, VertexShaderSource, ColorFragmentShaderSource);
        _textShader = new Shader(_gl, VertexShaderSource, TextFragmentShaderSource);
    }

    public void Clear()
    {
        _gl.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    public BackendTextureHandle CreateTexture(ImageData image)
    {
        var texture = new Texture(_gl, image);

        var handle = new BackendTextureHandle(_nextTextureHandle++);

        _textures.Add(handle.Value, texture);

        return handle;
    }

    public void DestroyTexture(BackendTextureHandle handle)
    {
        if (!_textures.Remove(handle.Value, out var texture))
            throw new InvalidOperationException($"Texture handle {handle.Value} does not exist.");

        texture.Dispose();
    }

    public unsafe void DrawTexture(TextureDrawCommand command)
    {
        if (!_textures.TryGetValue(command.Texture.Value, out var texture))
            throw new InvalidOperationException($"Texture handle {command.Texture.Value} does not exist.");

        _textureShader.Use();

        Matrix4x4 model =
            Matrix4x4.CreateScale(command.Width * command.Scale.X, command.Height * command.Scale.Y, 1f) *
            Matrix4x4.CreateRotationZ(command.Rotation) *
            Matrix4x4.CreateTranslation(command.X, command.Y, 0f);

        int modelLoc = _textureShader.GetUniformLocation("uModel");

        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        texture.Bind();

        int tintLoc = _textureShader.GetUniformLocation("uTint");

        _gl.Uniform4(tintLoc, command.Tint.R, command.Tint.G, command.Tint.B, command.Tint.A);

        _quad.Bind();
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }

    public unsafe void DrawRectangle(RectangleDrawCommand command)
    {
        _colorShader.Use();

        Matrix4x4 model = Matrix4x4.CreateScale(command.Width * command.Scale.X, command.Height * command.Scale.Y, 1f) *
            Matrix4x4.CreateRotationZ(command.Rotation) *
            Matrix4x4.CreateTranslation(command.X, command.Y, 0f);

        int modelLoc = _colorShader.GetUniformLocation("uModel");

        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int colorLoc = _colorShader.GetUniformLocation("uColor");

        _gl.Uniform4(colorLoc, command.Color.R, command.Color.G, command.Color.B, command.Color.A);

        _quad.Bind();
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }

    public unsafe void DrawTriangle(TriangleDrawCommand command)
    {
        float[] vertices =
        {
            0f, 0f,
            1f, 0f,
            0.5f, 1f
        };

        Matrix4x4 model =
            Matrix4x4.CreateScale(command.Width * command.Scale.X, command.Height * command.Scale.Y, 1f) *
            Matrix4x4.CreateRotationZ(command.Rotation) *
            Matrix4x4.CreateTranslation(command.X, command.Y, 0f);

        _colorShader.Use();

        int modelLoc = _colorShader.GetUniformLocation("uModel");
        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int colorLoc = _colorShader.GetUniformLocation("uColor");
        _gl.Uniform4(colorLoc, command.Color.R, command.Color.G, command.Color.B, command.Color.A);

        _gl.BindVertexArray(_shapeVertexArray);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _shapeVertexBuffer);

        fixed (float* data = vertices)
        {
            _gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                data,
                BufferUsageARB.StreamDraw
            );
        }

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public void DrawCircle(CircleDrawCommand command)
    {
        const int segmentCount = 32;
        float[] vertices = new float[(segmentCount + 2) * 2];

        vertices[0] = 0.5f;
        vertices[1] = 0.5f;

        for (int segment = 0; segment <= segmentCount; segment++)
        {
            float angle = MathF.PI * 2f * segment / segmentCount;
            int index = (segment + 1) * 2;

            vertices[index] = 0.5f + MathF.Cos(angle) * 0.5f;
            vertices[index + 1] = 0.5f + MathF.Sin(angle) * 0.5f;
        }

        Matrix4x4 model = Matrix4x4.CreateScale(
                command.Radius * 2f * command.Scale.X,
                command.Radius * 2f * command.Scale.Y,
                1f
            ) *
            Matrix4x4.CreateTranslation(
                command.X - command.Radius * command.Scale.X,
                command.Y - command.Radius * command.Scale.Y,
                0f
            );

        PrimitiveType primitiveType = command.Filled
            ? PrimitiveType.TriangleFan
            : PrimitiveType.LineLoop;

        int vertexCount = command.Filled
            ? segmentCount + 2
            : segmentCount + 1;

        DrawColoredShape(vertices, vertexCount, primitiveType, model, command.Color);
    }

    public void DrawLine(LineDrawCommand command)
    {
        float deltaX = command.EndX - command.StartX;
        float deltaY = command.EndY - command.StartY;
        float length = MathF.Sqrt(deltaX * deltaX + deltaY * deltaY);

        if (length <= 0f)
            return;

        float[] vertices =
        {
            0f, -0.5f,
            1f, -0.5f,
            1f, 0.5f,
            0f, 0.5f
        };

        Matrix4x4 model = Matrix4x4.CreateScale(length, command.Thickness, 1f) *
            Matrix4x4.CreateRotationZ(MathF.Atan2(deltaY, deltaX)) *
            Matrix4x4.CreateTranslation(command.StartX, command.StartY, 0f);

        DrawColoredShape(vertices, 4, PrimitiveType.TriangleFan, model, command.Color);
    }

    private unsafe void DrawColoredShape(
        float[] vertices,
        int vertexCount,
        PrimitiveType primitiveType,
        Matrix4x4 model,
        EngineColor color)
    {
        _colorShader.Use();

        int modelLoc = _colorShader.GetUniformLocation("uModel");
        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int colorLoc = _colorShader.GetUniformLocation("uColor");
        _gl.Uniform4(colorLoc, color.R, color.G, color.B, color.A);

        _gl.BindVertexArray(_shapeVertexArray);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _shapeVertexBuffer);

        fixed (float* data = vertices)
        {
            _gl.BufferData(
                BufferTargetARB.ArrayBuffer,
                (nuint)(vertices.Length * sizeof(float)),
                data,
                BufferUsageARB.StreamDraw
            );
        }

        _gl.DrawArrays(primitiveType, 0, (uint)vertexCount);
    }

    public unsafe void DrawText(TextDrawCommand command)
    {
        if (!_textures.TryGetValue(command.Texture.Value, out var texture))
            throw new InvalidOperationException($"Texture handle {command.Texture.Value} does not exist.");

        _textShader.Use();

        Matrix4x4 model = Matrix4x4.CreateScale(command.Width * command.Scale.X, command.Height * command.Scale.Y, 1f) *
            Matrix4x4.CreateRotationZ(command.Rotation) *
            Matrix4x4.CreateTranslation(command.X, command.Y, 0f);

        int modelLoc = _textShader.GetUniformLocation("uModel");

        _gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);

        int colorLoc = _textShader.GetUniformLocation("uColor");

        _gl.Uniform4(colorLoc, command.Color.R, command.Color.G, command.Color.B, command.Color.A);

        texture.Bind();
        _quad.Bind();
        _gl.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
    }

    public void EndFrame()
    {
    }
}