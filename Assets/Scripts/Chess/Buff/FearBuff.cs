using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearBuff : IDebuff
{
    public FearBuff(IChess chess, int turns) : base(chess, turns)
    {
        BuffType = BuffType.Fear;
        chess.NoticeWord("害怕了");

    }

    public override void OnBuffBegin()
    {
    }

    public override void OnBuffEnd()
    {
        base.OnBuffEnd();
    }

    public override void OnTurnStart()
    {
        _chess.Fear();
        OnBuffEnd();
    }
}
