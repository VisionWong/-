using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBuff : IDebuff
{
    public BurnBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Burn;
        chess.NoticeWord("烧伤了");
    }

    public override void OnBuffBegin()
    {
        //解除中毒特效
        _chess.Attribute.isBurned = true;
    }

    public override void OnBuffEnd()
    {
        _chess.Attribute.isBurned = false;
        //解除中毒特效
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        //每回合开始掉血
        _chess.Burned();
    }
}
