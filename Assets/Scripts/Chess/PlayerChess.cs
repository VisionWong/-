using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChess : IChess
{
    private SelectablePlayerChess m_selectable;

    public PlayerChess(IChessAttr attr, GameObject go) : base(attr, go)
    {
    }

    public void SetSelectableScript(SelectablePlayerChess script)
    {
        m_selectable = script;
        m_selectable.SetChess(this);
    }

    public void ChangeToIdle()
    {
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
        m_selectable.ChangeToActionEnd();
    }
}
