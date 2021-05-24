using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalysisBuff : IDebuff
{
    public ParalysisBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Paralysis;
        chess.NoticeWord("麻痹了");

    }

    public override void OnBuffBegin()
    {
        _chess.Attribute.isParalyzed = true;
    }

    public override void OnBuffEnd()
    {
        _chess.Attribute.isParalyzed = false;
        //解除中毒特效
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        //每回合1/3概率不能动
        int num = Random.Range(0, 3);
        if (num == 0) _chess.Paralyzed();
    }
}
