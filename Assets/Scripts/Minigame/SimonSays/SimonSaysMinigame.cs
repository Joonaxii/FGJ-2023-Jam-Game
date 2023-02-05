using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Direction {
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
    public Color arrowPlayer;

    public Color lightDefault;
    public Color lightHighlight;

    [Space(10), Header("Display")]
    public float timeBetweenFlashes;
    public float flashTime;

    private List<Direction> _simonInputs = new List<Direction>();
    private List<Direction> _playerInputs = new List<Direction>();
    private bool _isPlayerTurn;

    private void Start() => Initialize();

    // Update is called once per frame
    void Update()
    {
        var Inputs = GameManager.Instance.Inputs;

        if (Inputs.IsDown(InputHandler.InputType.MoveLeft) && _isPlayerTurn)
            PlayerInput(Direction.Left);
        if (Inputs.IsDown(InputHandler.InputType.MoveUp) && _isPlayerTurn)
            PlayerInput(Direction.Up);
        if (Inputs.IsDown(InputHandler.InputType.MoveRight) && _isPlayerTurn)
            PlayerInput(Direction.Right);
        if (Inputs.IsDown(InputHandler.InputType.MoveDown) && _isPlayerTurn)
            PlayerInput(Direction.Down);

        // TODO: Compare Inputs
        if (_playerInputs == _simonInputs)
        {
            StartCoroutine(SimonSays());
        }
    }
    private void PlayerInput(Direction dir)
    {
        arrows[(int)dir].color = arrowPlayer;
        _playerInputs.Add(dir);
    }

    IEnumerator SimonSays()
    {
        _isPlayerTurn = false;
        foreach (Direction input in _simonInputs)
        {
            arrows[(int)input].color = arrowHighlight;
            yield return new WaitForSeconds(flashTime);
            arrows[(int)input].color = arrowDefault;
            yield return new WaitForSeconds(timeBetweenFlashes);
        }
        _playerInputs.Clear();
        _isPlayerTurn = true;
    }

    public override void Cancel() => throw new System.NotImplementedException();
    public override void Finish() => throw new System.NotImplementedException();
    public override void Initialize()
    {
        foreach (var arrow in arrows)
            arrow.color = arrowDefault;
        foreach (var light in lights)
            light.color = lightDefault;

        int n = Random.Range(0, 4);
        Direction startDir = (Direction)n;
        _simonInputs.Add(startDir);

        _isPlayerTurn = false;
        StartCoroutine(SimonSays());
    }
}
