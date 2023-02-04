using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Vector2Int Coord => _gridPosition;

    private Vector2Int _gridPosition;
    private List<Vector2Int> _path = new List<Vector2Int>();

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
        return index < 0 ? 0 : 1;
    }

    public void SetPosition(GameBoard gameBoard)
    {
        _path.Clear();
        _gridPosition= new Vector2Int(gameBoard.Width >> 1, gameBoard.Height >> 1);
    }

    public int MakeMove(GameBoard gameBoard, Vector2Int nextPos)
    {


        return 0;
    }


    public void Update(float delta)
    {

    }
}