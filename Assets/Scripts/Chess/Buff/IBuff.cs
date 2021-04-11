using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBuff
{
    protected IChess _chess;
    protected int _turns;
    protected int _leftTurns;
    private int turns;

    protected IBuff(IChess chess, int turns)
    {
        _chess = chess;
        _turns = turns;
        _leftTurns = _turns;
    }

    public abstract void OnBuffBegin();
    public virtual void OnBuffEnd()
    {
        _chess.RemoveBuff(this);
    }
    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd()
    {
        _leftTurns--;
        if (_leftTurns == 0)
        {
            OnBuffEnd();            
        }
    }
}
