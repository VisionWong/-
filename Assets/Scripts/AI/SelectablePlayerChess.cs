using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class SelectablePlayerChess : ISelectable
{
    private PlayerChess m_chess;

    public void SetChess(PlayerChess chess)
    {
        m_chess = chess;
    }

    public enum SelectedState
    {
        Idle,
        WaitMove,
        WaitAttack,
        ActionEnd,
        Unselectable,
    }

    private SelectedState m_selectedState = SelectedState.Idle;

    public override void Selected()
    {
        if (Selectable && !m_isSelected)
        {
            switch (m_selectedState)
            {
                case SelectedState.Idle:
                    //通知战斗系统展开寻路网络
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectWalkableChess, m_chess);
                    break;
                case SelectedState.WaitMove:
                    //提示是否要停留原地
                    Debug.Log("正在准备移动");
                    break;
                case SelectedState.WaitAttack:
                    Debug.Log("正在准备攻击");
                    break;
            }
        }
        base.Selected();
    }

    public override void CancelSelect()
    {
        base.CancelSelect();
        //TODO 取消UI显示
        
    }

    public void ChangeToIdle()
    {
        m_selectedState = SelectedState.Idle;
    }

    public void ChangeToWaitMove()
    {
        m_selectedState = SelectedState.WaitMove;
    }

    public void ChangeToWaitAttack()
    {
        m_selectedState = SelectedState.WaitAttack;
    }

    public void ChangeToActionEnd()
    {
        m_selectedState = SelectedState.ActionEnd;
    }
}
