using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISelectable : MonoBehaviour
{
    public bool Selectable { get; set; }

    protected bool m_isSelected = false;

    public void OnMouseDown()
    {
        BattleSystem.Instance.SetSelected(this);
        Selected();
    }

    public virtual void Selected()
    {
        m_isSelected = true;
    }

    public virtual void CancelSelect()
    {
        m_isSelected = false;
    }
}
