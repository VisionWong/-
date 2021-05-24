using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChess : IChess
{
    [SerializeField]
    public bool isActionEnd = false;

    private SelectablePlayerChess m_selectable;

    public PlayerChess(ChessAttr attr, GameObject go) : base(attr, go)
    {
    }

    public void SetSelectableScript(SelectablePlayerChess script)
    {
        m_selectable = script;
        m_selectable.SetChess(this);
    }

    public void ChangeToIdle()
    {
        isActionEnd = false;
        m_selectable.ChangeToIdle();
    }

    public void ChangeToWaitMove()
    {       
        m_selectable.ChangeToWaitMove();
    }

    public void ChangeToWaitAttack()
    {
        m_selectable.ChangeToWaitAttack();
    }

    public void ChangeToActionEnd()
    {
        isActionEnd = true;
        m_selectable.ChangeToActionEnd();
    }
}
