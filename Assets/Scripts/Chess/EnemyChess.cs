using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChess : IChess
{
    private SelectableEnemyChess _selectable;

    public IAIController AI { get; set; }

    public EnemyChess(ChessAttr attr, GameObject go) : base(attr, go) { }

    public void SetSelectableScript(SelectableEnemyChess script)
    {
        _selectable = script;
        _selectable.SetChess(this);
    }
}
