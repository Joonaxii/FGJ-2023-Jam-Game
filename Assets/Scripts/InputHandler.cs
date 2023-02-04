using System;
using UnityEngine;

public class InputHandler
{
    private InputState[] _inputs = new InputState[9];
    
    public enum InputType
    {
        MoveLeft,
        MoveUp,
        MoveRight,
        MoveDown,

        Confirm,
        Cancel,

        Action,
        RotateLeft,
        RotateRight,
    }

    public void Update()
    {
        Span<bool> states = stackalloc bool[_inputs.Length];

        states[0] = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        states[1] = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        states[2] = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        states[3] = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        states[4] = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        states[5] = Input.GetKey(KeyCode.Escape);

        states[6] = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);
        states[7] = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow);
        states[8] = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow);

        for (int i = 0; i < 4; i++)
        {
            _inputs[i].Update(states[i]);
        }
    }

    public bool IsDown(InputType type) => _inputs[(int)type].down;
    public bool IsHeld(InputType type) => _inputs[(int)type].held;
    public bool IsUp(InputType type)   => _inputs[(int)type].up;

    private struct InputState
    {
        public bool down;
        public bool held;
        public bool up;

        public void Update(bool state)
        {
            up = (held | down) & !state;
            down = !down & state;
            held = state;
        }
    }
}