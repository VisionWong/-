using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAttackBuff : ILevelChange
{
    private int _level;

    public ChangeAttackBuff(IChess chess, int turns, int level) : base(chess, turns)
    {
        _level = level;
        if (_level > 0)
        {
            chess.NoticeWord("攻击力上升" + _level.ToString());
        }
        else
        {
            chess.NoticeWord("攻击力下降" + (-_level).ToString());
        }
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

    public override void OnTurnStart()
    {
        _turns--;
        if (_turns == 0)
        {
            OnBuffEnd();
        }
    }
}
