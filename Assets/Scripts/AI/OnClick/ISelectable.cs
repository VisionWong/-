using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public abstract class ISelectable : MonoBehaviour
{
    public bool Selectable { get; set; }

    protected bool m_isSelected = false;

    public virtual void Start()
    {
        Selectable = true;
        MessageCenter.Instance.AddListener(MessageType.GlobalCantSelect, OnCantSelect);
        MessageCenter.Instance.AddListener(MessageType.GlobalCanSelect, OnCanSelect);
    }

    public void OnMouseDown()
    {
        if (Selectable)
        {
            BattleSystem.Instance.SetSelected(this);
            Selected();
        }
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.GlobalCantSelect, OnCantSelect);
        MessageCenter.Instance.RemoveListener(MessageType.GlobalCanSelect, OnCanSelect);
    }

    public virtual void Selected()
    {
        m_isSelected = true;
    }

    public virtual void CancelSelect()
    {
        m_isSelected = false;
    }

    private void OnCantSelect()
    {
        Selectable = false;
    }

    private void OnCanSelect()
    {
        Selectable = true;
    }
}
