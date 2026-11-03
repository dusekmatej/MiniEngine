

using Silk.NET.Input;

namespace MiniEngine.Input;

public class KeyboardInput
{
    private readonly bool[] _currentState;
    private readonly bool[] _previousState;
    

    public KeyboardInput()
    {
        var keys = Enum.GetValues<Key>();
        int maxKey = keys.Max(key => (int)key);
        _currentState = new bool[maxKey+1];
        _previousState = new bool[maxKey +1];
        Console.WriteLine($"Engine W: {(int)Key.W}");
        Console.WriteLine($"Silk W: {(int)Silk.NET.Input.Key.W}");
    }
    public bool IsKeyDown(Key key)
    {
        return _currentState[(int)key] ;
    }
    public bool IsPressed(Key key)
    {
        return _currentState[(int)key] && !_previousState[(int)key];
    }
    public bool IsReleased(Key key)
    {
        return !_currentState[(int)key] && _previousState[(int)key];
    }

    public void UpdateState(IKeyboard keyboard)
    {
        Array.Copy(_currentState, _previousState, _currentState.Length);

        foreach (Key key in Enum.GetValues<Key>())
        {
            if (key == Key.Unknown)
                continue;
            Silk.NET.Input.Key silkKey = (Silk.NET.Input.Key)(int)key;
            _currentState[(int)key] = keyboard.IsKeyPressed(silkKey);
            
        }

    }


}