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

        //战斗相关
        OnSelectGrid,
        OnSelectIdleGrid,
        OnSelectWalkableGrid,
        OnSelectIdleChess,
        OnSelectWalkableChess,
        OnChessAction,
        OnCancelMove,
    }
}
