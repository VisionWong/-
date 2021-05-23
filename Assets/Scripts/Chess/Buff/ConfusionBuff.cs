using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionBuff : IDebuff
{
    public ConfusionBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Confusion;
    }

    public override void OnBuffBegin()
    {
    }

    public override void OnBuffEnd()
    {
        //解除中毒特效
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        //每回合1/3概率不能动且攻击自己
        int num = Random.Range(0, 3);
        if (num == 0) _chess.Confused();
    }
}
