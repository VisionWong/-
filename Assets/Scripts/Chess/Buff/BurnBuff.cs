using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBuff : IBuff
{
    public BurnBuff(IChess chess, int turns) : base(chess, turns)
    {
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
