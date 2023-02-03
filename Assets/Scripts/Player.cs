using System.Collections.Generic;
using UnityEngine;

public class Player
{
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

    public bool AttemptMove(GameBoard gameBoard, Vector2Int direction)
    {
        Vector2Int nextPos = _gridPosition + direction;
        if (!gameBoard.IsInBounds(nextPos)) { return false;  }

        if (_path.Count > 0)
        {
            if (_path[_path.Count-1] == nextPos)
            {
                _path.RemoveAt(_path.Count-1);
                _gridPosition = nextPos;
                return true;
            }
        }
        int index = IndexOfPointOnPath(nextPos);

        if (index < 0) { return false; }
        _gridPosition = nextPos;
        return true;
    }
}