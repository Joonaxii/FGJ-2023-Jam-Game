using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Directions {
    Left = 0,
    Up = 1,
    Right = 2,
    Down = 3
}
public class SimonSaysMinigame : HackingBase
{
    [Header("Minigame Components")]
    public SpriteRenderer[] arrows;
    public SpriteRenderer[] lights;
    [Space(5), Header("Colors")]
    public Color arrowDefault;
    public Color arrowHighlight;
    public Color lightDefault;
    public Color lightHighlight;

    [Space(10), Header("Display")]
    public float timeBetweenFlashes;
    public float flashTime;

    private Directions[] _simonInputs;
    private Directions[] _playerInputs;
    private bool _isPlayerTurn;

    private void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        var Inputs = GameManager.Instance.Inputs;

        if (Inputs.IsDown(InputHandler.InputType.MoveLeft) && _isPlayerTurn)
        {
            arrows[(int)Directions.Left].color = arrowHighlight;
        }
        if (Inputs.IsDown(InputHandler.InputType.MoveUp) && _isPlayerTurn)
        {
            arrows[(int)Directions.Up].color = arrowHighlight;
        }
        if (Inputs.IsDown(InputHandler.InputType.MoveRight) && _isPlayerTurn)
        {
            arrows[(int)Directions.Right].color = arrowHighlight;
        }
        if (Inputs.IsDown(InputHandler.InputType.MoveDown) && _isPlayerTurn)
        {
            arrows[(int)Directions.Down].color = arrowHighlight;
        }

        // TODO: Compare Inputs
    }
    public override void Cancel() => throw new System.NotImplementedException();
    public override void Finish() => throw new System.NotImplementedException();
    public override void Initialize()
    {
        foreach (var arrow in arrows)
            arrow.color = arrowDefault;
        foreach (var light in lights)
            light.color = lightDefault;

        _isPlayerTurn = true;
    }
}
