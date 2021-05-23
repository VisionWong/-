using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    None,
    Burn,
    Sleep,
    Freeze,
    Posion,
    Paralysis,
    Confusion,
    Fear,
    AtkUp,      //上升攻击,
    DefUp,      //上升防御,
    APUp,       //上升行动力,
    AtkDown,    //下降攻击,
    DefDown,    //下降防御,
    APDown,     //下降行动力,
}

public abstract class IBuff
{
    public BuffType BuffType { get; set; }

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

public abstract class IDebuff : IBuff
{
    public IDebuff(IChess chess, int turns) : base(chess, turns)
    {
    }
}

public abstract class ILevelChange : IBuff
{
    protected ILevelChange(IChess chess, int turns) : base(chess, turns)
    {
    }
}
