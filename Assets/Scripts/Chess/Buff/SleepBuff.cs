using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBuff : IDebuff
{
    public SleepBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Sleep;
        chess.NoticeWord("睡着了");

    }

    public override void OnBuffBegin()
    {
        //添加睡眠特效
    }

    public override void OnBuffEnd()
    {
        //取消睡眠特效
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        //每回合50%苏醒
        int num = Random.Range(0, 100);
        if (num < 50) OnBuffEnd();
        else _chess.Sleep();
    }
}
