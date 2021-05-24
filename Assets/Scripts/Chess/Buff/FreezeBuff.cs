using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBuff : IDebuff
{
    public FreezeBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Freeze;
        chess.NoticeWord("冰冻了");

    }

    public override void OnBuffBegin()
    {
        //出现中毒特效
    }

    public override void OnBuffEnd()
    {
        //解除中毒特效
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        //每回合25%概率解除
        int num = Random.Range(0, 100);
        if (num < 25) OnBuffEnd();
        else _chess.Freezed();
    }
}
