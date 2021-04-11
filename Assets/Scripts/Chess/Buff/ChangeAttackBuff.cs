using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAttackBuff : IBuff
{
    private int _level;

    public ChangeAttackBuff(IChess chess, int turns, int level) : base(chess, turns)
    {
        _level = level;
    }

    public override void OnBuffBegin()
    {
        _chess.Attribute.ChangeAttackLevel(_level);
    }

    public override void OnBuffEnd()
    {
        _chess.Attribute.ChangeAttackLevel(-_level);
        base.OnBuffEnd();
    }
}
