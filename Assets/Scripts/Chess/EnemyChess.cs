using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChess : IChess
{
    private SelectableEnemyChess _selectable;

    public IAIController AI { get; set; }

    public bool isActionEnd = false;

    public EnemyChess(ChessAttr attr, GameObject go) : base(attr, go) { }

    public void SetSelectableScript(SelectableEnemyChess script)
    {
        _selectable = script;
        _selectable.SetChess(this);
    }

    public void ChangeToIdle()
    {
        isActionEnd = false;
    }

    public void ChangeToActionEnd()
    {
        isActionEnd = true;
    }
}
