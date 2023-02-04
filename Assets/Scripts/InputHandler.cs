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

        states[(int)InputType.MoveLeft] = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        states[(int)InputType.MoveUp] = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        states[(int)InputType.MoveRight] = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        states[(int)InputType.MoveDown] = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        states[(int)InputType.Confirm] = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
        states[(int)InputType.Cancel] = Input.GetKey(KeyCode.Escape);
              
        states[(int)InputType.Action] = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);
        states[(int)InputType.RotateLeft] = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow);
        states[(int)InputType.RotateRight] = Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow);

        for (int i = 0; i < _inputs.Length; i++)
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
            up = held & !state;
            down = state & !held;
            held = state;
        }
    }
}