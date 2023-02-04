using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Vector2Int Coord => _gridPosition;

    public List<Vector2Int> GetPath => _path;

    private Vector2Int _gridPosition;
    private List<Vector2Int> _path = new List<Vector2Int>();
    private int _prevPos;

    public int IndexOfPointOnPath(Vector2Int point)
    {
        for (int i = 0; i < _path.Count - 1; i++)
        {
            if (_path[i] == point)
            {
                return i;
            }
        }
        return -1;
    }

    public int AttemptMove(GameBoard gameBoard, Vector2Int direction, out Vector2Int curPos, out Vector2Int newPos)
    {
        newPos = _gridPosition + direction;
        curPos = _gridPosition;
        if (!gameBoard.IsInBounds(newPos)) { return 0;  }

        if (_path.Count > 0)
        {
            if (_path[_path.Count - 1] == newPos)
            {
                return 2;
            }
        }
        int index = IndexOfPointOnPath(newPos);
        return index < 0 ? 1 : 0;
    }

    public void ResetPlayer(GameBoard gameBoard)
    {
        _path.Clear();
        _prevPos = 0;
        _gridPosition = new Vector2Int(gameBoard.Width >> 1, gameBoard.Height >> 1);

        GameManager.Instance.UpdatePath();
    }

    public int MakeMove(GameBoard gameBoard)
    {


        return 0;
    }

    public void Update(float delta)
    {
        var input = GameManager.Instance.Inputs;
        Vector2Int dir = default;

        if (input.IsDown(InputHandler.InputType.MoveLeft))  { dir.x--; }
        if (input.IsDown(InputHandler.InputType.MoveRight)) { dir.x++; }

        if (input.IsDown(InputHandler.InputType.MoveUp))    { dir.y++; }
        if (input.IsDown(InputHandler.InputType.MoveDown))  { dir.y--; }

        if(dir != Vector2Int.zero)
        {
            var board = GameManager.Instance.Board;
            switch (AttemptMove(board, dir, out var curPos, out var newPos))
            {
                case 1:
                    _path.Add(curPos);
                    _gridPosition = newPos;

                    GameManager.Instance.UpdatePath();
                    GameManager.Instance.CameraController.SetPosition(board.GridToWorld(newPos));
                    break;

                case 2:
                    _path.RemoveAt(_path.Count - 1);
                    _gridPosition = newPos;

                    GameManager.Instance.UpdatePath();
                    GameManager.Instance.CameraController.SetPosition(board.GridToWorld(newPos));
                    break;
            }
        }
    }
}