using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitCapacitor : Unit
{
    public int baseCapacityAdd = 16;

    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        ref var tile = ref _mngr.Board[_boardID];
        _mngr.GetStats.AddCapacity(tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? baseCapacityAdd * 3 : baseCapacityAdd);
    }
}
