﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBuff : IBuff
{
    private float _bloodPer;

    public PoisonBuff(IChess chess, int turns, float bloodPer) : base(chess, turns)
    {
        _bloodPer = bloodPer;
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
        //每回合开始掉血
        _chess.Poisoned(_bloodPer);
    }
}
