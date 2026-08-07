using Silk.NET.OpenGL;

namespace MiniEngine.Engine.Graphics;

public class Shader
{
    private readonly GL _gl;
    public uint Handle { get; private set; }
    
    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl;

        uint vertexShader = CompileShader(
            ShaderType.VertexShader, vertexSource
            );

        uint fragmentShader = CompileShader(
            ShaderType.FragmentShader, fragmentSource
            );

            Handle = _gl.CreateProgram();

            _gl.AttachShader(Handle, vertexShader);
            _gl.AttachShader(Handle, fragmentShader);

            _gl.LinkProgram(Handle);

            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);

        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        return shader;
    }

    public void Use()
        => _gl.UseProgram(Handle);

    public int GetUniformLocation(string name)
        => _gl.GetUniformLocation(Handle, name);

}