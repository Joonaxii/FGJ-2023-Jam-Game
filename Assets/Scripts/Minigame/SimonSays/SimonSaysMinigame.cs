using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Direction
{
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

    [SerializeField] private List<Direction> _simonInputs = new List<Direction>();
    [SerializeField] private List<Direction> _playerInputs = new List<Direction>();
    [SerializeField] private bool _isPlayerTurn;
    private bool _isPressing;
    private int _curRound = 0;

    private void Start() => Initialize();

    // Update is called once per frame
    void Update()
    {
        if (!_isPlayerTurn)
            return;

        var Inputs = GameManager.Instance.Inputs;

        if (Inputs.IsDown(InputHandler.InputType.MoveLeft))
            PlayerInput(Direction.Left);
        if (Inputs.IsUp(InputHandler.InputType.MoveLeft))
            PlayerRelease(Direction.Left);

        if (Inputs.IsDown(InputHandler.InputType.MoveUp))
            PlayerInput(Direction.Up);
        if (Inputs.IsUp(InputHandler.InputType.MoveUp))
            PlayerRelease(Direction.Up);

        if (Inputs.IsDown(InputHandler.InputType.MoveRight))
            PlayerInput(Direction.Right);
        if (Inputs.IsUp(InputHandler.InputType.MoveRight))
            PlayerRelease(Direction.Right);

        if (Inputs.IsDown(InputHandler.InputType.MoveDown))
            PlayerInput(Direction.Down);
        if (Inputs.IsUp(InputHandler.InputType.MoveDown))
            PlayerRelease(Direction.Down);

        // TODO: Compare Inputs
        if (_playerInputs.Count == _simonInputs.Count && !_isPressing)
        {
            foreach (var arrow in arrows)
                arrow.color = arrowDefault;

            bool areIdentical = true;
            for (int i = 0; i < _playerInputs.Count; i++)
            {
                if (_playerInputs[i] != _simonInputs[i])
                {
                    areIdentical = false;
                    break;
                }
            }
            if (areIdentical)
            {

                StartCoroutine(SimonSays(false));
            }
            else
            {
                GameManager.Instance.Hacking.CancelHack();
            }
        }else if(_playerInputs.Count > _simonInputs.Count)
        {
            GameManager.Instance.Hacking.CancelHack();
        }
    }
    private void PlayerInput(Direction dir)
    {
        arrows[(int)dir].color = arrowPlayer;
        _playerInputs.Add(dir);
        _isPressing = true;
    }
    private void PlayerRelease(Direction dir)
    {
        arrows[(int)dir].color = arrowDefault;
        _isPressing = false;
    }

    private void NewSimonInput()
    {
        int n = Random.Range(0, 4);
        Direction startDir = (Direction)n;
        _simonInputs.Add(startDir);
    }

    IEnumerator SimonSays(bool isInit)
    {
        if (_curRound == 3)
        {
            Finish();
            yield break;
        }

        _isPlayerTurn = false;
        if (isInit)
            yield return new WaitForSeconds(0.5f);

        if (!isInit)
        {
            NewSimonInput();
            lights[_curRound].color = lightHighlight;
            _curRound++;
        }

        foreach (var input in _simonInputs)
        {
            arrows[(int)input].color = arrowHighlight;
            yield return new WaitForSeconds(flashTime);
            arrows[(int)input].color = arrowDefault;
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        _playerInputs.Clear();
        _isPlayerTurn = true;
    }

    public override void Cancel()
    {
        Debug.Log("You loser!");
        _playerInputs.Clear();
        _simonInputs.Clear();
        _isPlayerTurn = false;
        _isPressing = false;
        _curRound = 0;
    }
    public override void Finish()
    {
        Debug.Log("You win!");
        _playerInputs.Clear();
        _simonInputs.Clear();
        _isPlayerTurn = false;
        _isPressing = false;
        _curRound = 0;
        base.Finish();
    }
    public override void Initialize()
    {
        _isPressing = false;
        _curRound = 0;
        _playerInputs.Clear();
        _simonInputs.Clear();

        foreach (var arrow in arrows)
            arrow.color = arrowDefault;
        foreach (var light in lights)
            light.color = lightDefault;

        NewSimonInput();

        _isPlayerTurn = false;
        StartCoroutine(SimonSays(true));
    }
}
