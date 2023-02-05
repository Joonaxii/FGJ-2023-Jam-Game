using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState
    {
        None,
        BeingHacked,
        Capturing,
        CleaningUp,
        Fail,
        Done
    }

    public UnitState State => _state;

    public Renderer[] rends;

    protected UnitState _state;
    protected int _boardID;
    protected GameManager _mngr;

    protected MaterialPropertyBlock _props;
    private Action<int> _cb;
    protected Animator _anim;

    public void Setup(int id)
    {
        _anim = GetComponentInChildren<Animator>();

        _props = new MaterialPropertyBlock();
        _props.SetFloat("_Blend", 0.0f);

        _mngr = GameManager.Instance;
        _boardID = id;
        RefreshHackState();
    }

    public float GetTileBlend(ref GameBoard.BoardTile tile)
    {
        if (tile.IsHacked())
        {
            return tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? 2.0f : 1.0f;
        }
        return tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? -2.0f : 0.0f;
    }

    public void Access(Action<int> callback)
    {
        _cb = null;
        ref var tile = ref _mngr.Board[_boardID];
        if (tile.IsHacked())
        {
            callback?.Invoke(1);
            return;
        }
        
        switch (_state)
        {
            case UnitState.None:
                _cb = callback;
                SetUnitState(UnitState.BeingHacked);
                break;
        }
    }

    public void RefreshHackState()
    {
        ref var tile = ref _mngr.Board[_boardID];
        _props.SetFloat("_Palette_Blend", GetTileBlend(ref tile));
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].SetPropertyBlock(_props);
        }

        if(_anim != null)
        {
            _anim.speed = tile.flags.HasFlag(GameBoard.TileFlags.Overclocked) ? 6.0f : 1.0f;
        }
    }

    public int ScanUnit()
    {
        return 0; 
    }

    public virtual void Tick(float delta)
    {

    }

    public virtual void OnHackSuccess()
    {

    }

    public void SetUnitState(UnitState state)
    {
        _state = state;
        EvaluateState();
    }

    protected virtual void EvaluateState()
    {
        switch (_state)
        {
            case UnitState.BeingHacked:
                GameManager.Instance.Hacking.BeginHacking((bool valid) => {
                    if (valid)
                    {
                        SetUnitState(UnitState.Done);
                        return;
                    }
                    SetUnitState(UnitState.Fail);

                });
                break;

            case UnitState.Done:

                ref var tile = ref _mngr.Board[_boardID];
                _mngr.Board.SetTileState(ref tile, true);

                OnHackSuccess();
                _mngr.GetStats.UpdateStats();
                _cb?.Invoke(1);
                RefreshHackState();
                break;

            case UnitState.Fail:

                ref var tileB = ref _mngr.Board[_boardID];
                _mngr.Board.SetTileState(ref tileB, false);

                _mngr.GetStats.UpdateStats();
                _cb?.Invoke(0);
                RefreshHackState();
                _state = UnitState.None;
                break;
        }
    }
}