using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChess : IChess
{
    public SelectablePlayerChess SelectableSript { get; set; }

    public PlayerChess(IChessAttr attr, GameObject go) : base(attr, go)
    {
    }

    public void ChangeToIdle()
    {
        SelectableSript.ChangeToIdle();
    }
}
