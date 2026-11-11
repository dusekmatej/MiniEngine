namespace MiniEngine.Input;

using System.Numerics;
using Silk.NET.Input;

public class MouseInput
{
    private readonly bool[] _currentState;
    private readonly bool[] _previousState;
    public Vector2 Position { get; private set; }
    public float ScrollX { get; private set; }
    public float ScrollY { get; private set; }

    public MouseInput()
    {
        int buttonCount = Enum.GetValues<MouseButton>().Length;

        _currentState = new bool[buttonCount];
        _previousState = new bool[buttonCount];
        Console.WriteLine($"Engine Left: {(int)MouseButton.Left}");
    }

    public bool IsButtonDown(MouseButton button)
    {
        return _currentState[(int)button];
    }

    public bool IsPressed(MouseButton button)
    {
        return _currentState[(int)button] && !_previousState[(int)button];
    }

    public bool IsReleased(MouseButton button)
    {
        return !_currentState[(int)button] && _previousState[(int)button];
    }
    public void UpdateState(IMouse mouse)
    {
        Array.Copy(_currentState, _previousState, _currentState.Length);
        Position = new Vector2(mouse.Position.X, mouse.Position.Y);

    foreach (MouseButton button in Enum.GetValues<MouseButton>())
        {
            if (button == MouseButton.Unknown)
                continue;

            Silk.NET.Input.MouseButton silkButton =
                (Silk.NET.Input.MouseButton)((int)button - 1);

            _currentState[(int)button] = mouse.IsButtonPressed(silkButton);
        }
    }
    public void Connect(IMouse mouse)
    {
        mouse.Scroll += OnScroll;
    }
    public Vector2 ConsumeScroll()
    {
        Vector2 scroll = new Vector2(ScrollX, ScrollY);

     ScrollX = 0f;
     ScrollY = 0f;

      return scroll;
    }   
    

    private void OnScroll(IMouse mouse, ScrollWheel scroll)
    {
        Console.WriteLine($"Scroll event: X={scroll.X}, Y={scroll.Y}");

        ScrollX += scroll.X;
        ScrollY += scroll.Y;
    }
}