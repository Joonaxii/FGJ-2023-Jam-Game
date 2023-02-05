using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : Unit
{
    public float baseCapa = 32;
    public float baseBps = 1.5f;
    public float baseCPU = 0.5f;
    public float scanSlow = 0.25f;

    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        ref var tile = ref _mngr.Board[_boardID];
        _mngr.GetStats.AddBPS(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseBps * 3 : baseBps);
        _mngr.GetStats.AddCapacity(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseCapa * 3 : baseCapa);
        _mngr.GetStats.AddCPU(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseCPU * 3 : baseCPU);
        _mngr.GetStats.AddScanMult(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? scanSlow * 3 : scanSlow);
    }
}
