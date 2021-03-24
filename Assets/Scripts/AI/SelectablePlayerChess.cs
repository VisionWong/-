using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        WaitMoving,
        WaitAttacing,
        Unselectable,
    }

    private SelectedState m_selectedState = SelectedState.Idle;

    public override void Selected()
    {
        if (!m_isSelected)
        {
            switch (m_selectedState)
            {
                case SelectedState.Idle:
                    //通知战斗系统展开寻路网络
                    
                    BattleSystem.Instance.HighlightWalkableGrids(m_chess);
                    break;
                case SelectedState.WaitMoving:
                    //提示是否要停留原地

                    break;
                case SelectedState.WaitAttacing:
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
