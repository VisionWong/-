using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFramework
{
    /// <summary>
    /// 全局事件类型，需要自定义
    /// </summary>
	public enum MessageType
    {
        GlobalCantSelect,
        GlobalCanSelect,

        //战斗操作
        OnSelectGrid,
        OnSelectIdleGrid,
        OnSelectWalkableGrid,
        OnSelectChess,
        OnCancelSelectChess,
        OnSelectWalkableChess,
        OnSelectUnwalkableChess,
        OnChessAction,
        OnCancelMove,
        OnChessMoving,
        OnClickSkillBtn,
        OnSearchAttackableEnd,
        OnClickDirCancelBtn,
        OnChessActionEnd,

        //战斗回合
        OnPlayerTurn,
        OnEnemyTurn,
        OnVictory,
        OnDefeat,
    }
}
