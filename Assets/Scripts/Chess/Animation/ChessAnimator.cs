using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;
using VFramework;

public class ChessAnimator : MonoBehaviour
{
    private Animator _anim;
    private float lastX, lastY;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        MessageCenter.Instance.AddListener(MessageType.OnCancelMove, OnCancelMove);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.OnCancelMove, OnCancelMove);
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
        ChangeForward(EnumTool.GetOppositeDir(dir));
    }

    public void Healing()
    {
        transform.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1, 0.5f);
    }

    /// <summary>
    /// 改变朝向
    /// </summary>
    public void ChangeForward(Direction dir)
    {
        var v3 = EnumTool.DirToVector3(dir);
        _anim.SetFloat("x", v3.x);
        _anim.SetFloat("y", v3.y);
    }

    private IEnumerator MoveCorou(List<MapGrid> grids, Action callback = null)
    {
        lastX = _anim.GetFloat("x");
        lastY = _anim.GetFloat("y");
        _anim.SetBool("isMove", true);
        MapGrid lastGrid = grids?[0];
        for (int i = 1; i < grids.Count; i++)
        {
            _anim.SetFloat("x", grids[i].X - lastGrid.X);
            _anim.SetFloat("y", lastGrid.Y - grids[i].Y);
            lastGrid = grids[i];
            var dest = grids[i].transform.position;
            var tweener = transform.DOMove(new Vector3(dest.x, dest.y, transform.position.z), 0.4f);
            MessageCenter.Instance.Broadcast(MessageType.OnChessMoving, dest);
            yield return tweener.WaitForCompletion();
        }
        _anim.SetBool("isMove", false);
        //获取倒二两个格子变更最终停留的朝向
        if (grids.Count >= 2)
        {
            var x = lastGrid.X - grids[grids.Count - 2].X;
            var y = grids[grids.Count - 2].Y - lastGrid.Y;
            _anim.SetFloat("x", x);
            _anim.SetFloat("y", y);
        }
        callback?.Invoke();
    }

    private void OnCancelMove()
    {
        _anim.SetFloat("x", lastX);
        _anim.SetFloat("y", lastY);
    }
}
