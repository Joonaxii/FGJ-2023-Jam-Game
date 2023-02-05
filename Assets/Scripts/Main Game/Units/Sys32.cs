using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sys32 : Unit
{
    public override void OnHackSuccess()
    {
        base.OnHackSuccess();

        _mngr.Board.RootsCorrupted++;
    }
}
