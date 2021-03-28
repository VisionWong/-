using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;

public class ChessAnimator : MonoBehaviour
{
    /// <summary>
    /// 按照给定的格子路径按指定速度移动，移动动画结束后调用回调函数
    /// </summary>
    /// <param name="grids"></param>
    /// <param name="duration">移动每一格的时间</param>
    /// <param name="callback"></param>
    public void Move(List<MapGrid> grids, float duration = 0.5f, Action callback = null)
    {
        MoveCorou(grids, duration, callback);
    }

    private IEnumerator MoveCorou(List<MapGrid> grids, float duration = 0.5f, Action callback = null)
    {
        for (int i = 0; i < grids.Count; i++)
        {
            var dest = grids[i].transform.position;
            var tweener = transform.DOMove(new Vector3(dest.x, dest.y, transform.position.z), 0.5f);
            yield return tweener.WaitForCompletion();
        }
        callback?.Invoke();
    }
}
