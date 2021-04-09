using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;
using VFramework.UIManager;

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
            Camera.main.GetComponent<CameraController>().MoveToTarget(transform.position);
            switch (m_selectedState)
            {
                case SelectedState.Idle:
                    Debug.Log("棋子正在等待操作");
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectWalkableChess, m_chess);
                    break;
                case SelectedState.WaitMove:
                    Debug.Log("棋子选择停留原地");
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectWalkableGrid, m_chess.StayGrid);
                    break;
                case SelectedState.WaitAttack:
                    Debug.Log("正在准备攻击");
                    break;
                case SelectedState.ActionEnd:
                    Debug.Log("该棋子已经行动完毕");
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectUnwalkableChess);
                    break;
                case SelectedState.Unselectable:
                    Debug.Log("该棋子暂时无法选中");
                    break;
                default:
                    Debug.LogWarning("棋子不存在该状态");
                    break;
            }
            MessageCenter.Instance.Broadcast(MessageType.OnSelectChess);
        }
        base.Selected();
    }

    public override void CancelSelect()
    {
        base.CancelSelect();
        //TODO 取消UI显示
        UIManager.Instance.PopPanel();
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
