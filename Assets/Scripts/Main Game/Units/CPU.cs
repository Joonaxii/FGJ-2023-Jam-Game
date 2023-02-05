using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Unit
{
    public float baseCPU = 0.5f;

    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        ref var tile = ref _mngr.Board[_boardID];
        _mngr.GetStats.AddCPU(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseCPU * 3 : baseCPU);
    }
}
