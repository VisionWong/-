using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;
using VFramework;

public class ChessAnimator : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 按照给定的格子路径按指定速度移动，移动动画结束后调用回调函数
    /// </summary>
    /// <param name="grids"></param>
    /// <param name="callback"></param>
    public void Move(List<MapGrid> grids, Action callback = null)
    {
        StartCoroutine(MoveCorou(grids, callback));
    }

    public void TakeDamage(Direction dir)
    {
        transform.DOPunchPosition(EnumTool.DirToVector3(dir) * 0.2f, 0.5f, 1, 0);
    }

    private IEnumerator MoveCorou(List<MapGrid> grids, Action callback = null)
    {
        MapGrid lastGrid = grids?[0];
        for (int i = 1; i < grids.Count; i++)
        {
            _anim.SetInteger("x", grids[i].X - lastGrid.X);
            _anim.SetInteger("y", lastGrid.Y - grids[i].Y);
            lastGrid = grids[i];
            var dest = grids[i].transform.position;
            var tweener = transform.DOMove(new Vector3(dest.x, dest.y, transform.position.z), 0.4f);
            MessageCenter.Instance.Broadcast(MessageType.OnChessMoving, dest);
            yield return tweener.WaitForCompletion();
        }
        _anim.SetInteger("x", 0);
        _anim.SetInteger("y", 0);
        callback?.Invoke();
    }
}
