using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysClock : Unit
{
    public float scanSlow = 0.125f;
     
    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        ref var tile = ref _mngr.Board[_boardID];
        _mngr.GetStats.AddScanMult(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? scanSlow * 3 : scanSlow);
    }
}
