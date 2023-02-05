using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitGenerator : Unit
{
    public float baseBps = 0.5f;

    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        ref var tile = ref _mngr.Board[_boardID];
        _mngr.GetStats.AddBPS(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseBps * 3 : baseBps);
    }
}
