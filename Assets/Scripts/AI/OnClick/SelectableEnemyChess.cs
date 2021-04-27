using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class SelectableEnemyChess : ISelectable
{
    public enum SelectedState
    {
        Idle,
        OnAction,
        Unselectable,
    }
    private EnemyChess _chess;
    private SelectedState _selectedState;

    public void SetChess(EnemyChess chess)
    {
        _chess = chess;
    }

    public override void Selected()
    {
        if (Selectable && !m_isSelected)
        {
            Camera.main.GetComponent<CameraController>().MoveToTarget(transform.position);
            switch (_selectedState)
            {
                case SelectedState.Idle:
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectChess, _chess);
                    break;
                case SelectedState.OnAction:
                    Debug.Log("该棋子因行动中暂时无法选中");
                    break;
                default:
                    Debug.LogWarning("棋子不存在该状态");
                    break;
            }
        }
        base.Selected();
    }

    public void ChangeToIdle()
    {
        _selectedState = SelectedState.Idle;
    }
    public void ChangeToOnAction()
    {
        _selectedState = SelectedState.OnAction;
    }

}
