using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public int AccumulatedCost => _accumulatedCost;

    public Vector2Int Coord => _gridPosition;

    public float playerSmoothing = 0.25f;
    public Transform playerTR;

    public List<Vector2Int> GetPath => _path;

    private Vector2Int _gridPosition;
    private List<Vector2Int> _path = new List<Vector2Int>();

    private Vector3 _plrVelo;
    private Vector3 _plrPos;
    private Vector3 _plrTgt;

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

    public int AttemptMove(ref int cost, GameBoard gameBoard, Vector2Int direction, out Vector2Int curPos, out Vector2Int newPos)
    {
        newPos = _gridPosition + direction;
        curPos = _gridPosition;
        if (!gameBoard.IsInBounds(newPos)) { return 0;  }

        ref var curTile = ref gameBoard[curPos];
        ref var tile = ref gameBoard[newPos];

        bool beingScanned = tile.IsBeingScanned() | curTile.IsBeingScanned();

        int costC = gameBoard.GetGridPointMoveCost(ref curTile);
        int costT = gameBoard.GetGridPointMoveCost(ref tile);

        if (_path.Count > 0)
        {
            if (_path[_path.Count - 1] == newPos)
            {
                if (beingScanned)
                {
                    GameManager.Instance.GetStats.TakeDamage();
                }
                cost -= costC;
                return 2;
            }
        }

        bool hacked = curTile.IsHacked();
        bool freeMove = hacked && _path.Count < 1 && tile.IsHacked();

        int index = IndexOfPointOnPath(newPos);
        if(!freeMove && index > -1) { return 0; }

        if ((!hacked && curTile.type != GameBoard.TileType.Empty) || tile.type == GameBoard.TileType.MainBase) 
        { 
            return 0; 
        }

        int nextCost = cost + costT;
        if(nextCost > GameManager.Instance.GetStats.Bits) { return 0; }

        cost = nextCost;
        if (beingScanned)
        {
            GameManager.Instance.GetStats.TakeDamage();
        }
        return freeMove ? 3: 1;
    }

    public void ResetPlayer(GameBoard gameBoard)
    {
        _path.Clear();
        _gridPosition = new Vector2Int(gameBoard.Width >> 1, (gameBoard.Height >> 1) - 2);
        _accumulatedCost = 0;
        _plrVelo = Vector3.zero;
        playerTR.position = _plrPos = _plrTgt = gameBoard.GridToWorld(_gridPosition);
        GameManager.Instance.UpdatePath();
    }

    public bool MakeMove(GameBoard gameBoard)
    {
        if(_path.Count < 1) { return false; }

        if(_accumulatedCost > 0)
        {
            GameManager.Instance.GetStats.UseBits(_accumulatedCost);
            _accumulatedCost = 0;
        }

        ref var tile = ref gameBoard[_gridPosition];

        bool beingScanned = tile.IsBeingScanned();
    
        if (!tile.IsHacked() && tile.type != GameBoard.TileType.Empty)
        {
            //TODO: Begin hacking game...

            if(tile.linkTile > -1)
            {
                tile = ref gameBoard[tile.linkTile];
            }

            if(tile.unit != null)
            {
                Vector2Int coord = tile.gridPos;
                tile.unit.Access((int ret) =>
                {
                    switch (ret)
                    {
                        case 1:
                            gameBoard.SetTileState(coord, true);
                            GameManager.Instance.GetStats.SetCorruption(gameBoard.GetCorruption());
                            break;
                    }
                });
            }
        }
        else if(!tile.IsHacked())
        {
            gameBoard.SetTileState(_gridPosition, true);
        }

        foreach (var item in _path)
        {
            ref var tileC = ref gameBoard[item];
            beingScanned |= tileC.IsBeingScanned();

            if (!tileC.IsHacked())
            {
                gameBoard.SetTileState(item, true);

            }
        }
        _path.Clear();
        GameManager.Instance.UpdatePath();

        if (beingScanned)
        {
            GameManager.Instance.GetStats.TakeDamage();
        }
        var stats = GameManager.Instance.GetStats;
        GameManager.Instance.GameUI.UpdateBitAmount(stats.Bits, stats.BitCapacity, stats.BitsPerSecond, _accumulatedCost);
        GameManager.Instance.GetStats.SetCorruption(gameBoard.GetCorruption());
        return false;
    }

    private int _accumulatedCost = 0;
    public void Update(float delta)
    {
        _plrPos = Vector3.SmoothDamp(_plrPos, _plrTgt, ref _plrVelo, playerSmoothing, float.MaxValue, delta);
        _plrPos.y = 0.1f;
        playerTR.position = _plrPos;

        var input = GameManager.Instance.Inputs;
        Vector2Int dir = default;

        if (input.IsDown(InputHandler.InputType.MoveLeft))  { dir.x--; }
        if (input.IsDown(InputHandler.InputType.MoveRight)) { dir.x++; }

        if (input.IsDown(InputHandler.InputType.MoveUp))    { dir.y++; }
        if (input.IsDown(InputHandler.InputType.MoveDown))  { dir.y--; }

        int prev = _accumulatedCost;

        if(dir != Vector2Int.zero)
        {
            var board = GameManager.Instance.Board;
            switch (AttemptMove(ref _accumulatedCost, board, dir, out var curPos, out var newPos))
            {
                case 1:
                    _path.Add(curPos);
                    _gridPosition = newPos;

                    GameManager.Instance.UpdatePath();
                    GameManager.Instance.CameraController.SetPosition(board.GridToWorld(newPos));
                    _plrTgt = board.GridToWorld(_gridPosition);
                    break;

                case 3:
                    _gridPosition = newPos;
                    GameManager.Instance.CameraController.SetPosition(board.GridToWorld(newPos));
                    _plrTgt = board.GridToWorld(_gridPosition);
                    break;

                case 2:
                    _path.RemoveAt(_path.Count - 1);
                    _gridPosition = newPos;

                    GameManager.Instance.UpdatePath();
                    GameManager.Instance.CameraController.SetPosition(board.GridToWorld(newPos));
                    _plrTgt = board.GridToWorld(_gridPosition);
                    break;
            }
        }

        if(prev != _accumulatedCost)
        {
            var stats = GameManager.Instance.GetStats;
            GameManager.Instance.GameUI.UpdateBitAmount(stats.Bits, stats.BitCapacity, stats.BitsPerSecond, _accumulatedCost);
        }
        if (input.IsDown(InputHandler.InputType.Confirm)) 
        {
            MakeMove(GameManager.Instance.Board);
        }
    }
}