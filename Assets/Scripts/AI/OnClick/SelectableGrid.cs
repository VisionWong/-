using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public enum GridSelectedState
{
    Unselectable,
    Idle,
    Moveable
}

public class SelectableGrid : ISelectable
{
    public GridSelectedState SelectedState { get; set; }

    private MapGrid m_grid;

    private void Awake()
    {
        m_grid = GetComponent<MapGrid>();
        SelectedState = GridSelectedState.Idle;
    }

    public override void Selected()
    {
        if (Selectable && !m_isSelected)
        {
            MessageCenter.Instance.Broadcast(MessageType.OnSelectGrid, m_grid);
            switch (SelectedState)
            {
                case GridSelectedState.Idle:
                    Debug.Log("当前选中的格子类型为" + m_grid.TerrainType.ToString());
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectIdleGrid, m_grid);
                    break;
                case GridSelectedState.Moveable:
                    MessageCenter.Instance.Broadcast(MessageType.OnSelectWalkableGrid, m_grid);
                    break;
                case GridSelectedState.Unselectable:
                    //什么也不做
                    break;
                default:
                    Debug.LogWarning("当前格子不存在该选中类型" + SelectedState.ToString());
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
}
