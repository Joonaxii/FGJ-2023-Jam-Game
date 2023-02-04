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
        Done
    }

    public UnitState State => _state;

    protected UnitState _state;
    protected HackingBase _hacking;
    protected int _boardID;
    
    public void Setup(int id)
    {

    }

    public virtual void Tick(float delta)
    {

    }

    public void SetUnitState(UnitState state)
    {
        _state = state;
        EvaluateState();
    }

    protected virtual void EvaluateState()
    {

    }
}