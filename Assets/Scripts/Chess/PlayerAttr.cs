using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttr : ChessAttr
{
    private string EvoCondition;

    public PlayerAttr(ChessData data) : base(data)
    {
        //EvoCondition = data.evoTaskId;
    }
}
