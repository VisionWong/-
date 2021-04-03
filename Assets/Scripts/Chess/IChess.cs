using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝可梦克制相关的属性
/// </summary>
public enum PMType
{
    None,
    Common,
    Grass,
    Fire,
    Water,

}
public abstract class IChess
{
    public PathPack PathPack { get; private set; }
    public IChessAttr Attribute { get; set; }
    public MapGrid StayGrid { get; private set; }

    protected GameObject m_gameObject;
    protected List<ISkill> m_skillList = new List<ISkill>();
    protected ChessAnimator m_anim = null; 

    public IChess(IChessAttr attr, GameObject go)
    {
        Attribute = attr;
        m_gameObject = go;
    }

    public void SetPathPack(PathPack pack)
    {
        PathPack = pack;
    }

    public void SetAnimator(ChessAnimator anim)
    {
        m_anim = anim;
    }

    /// <summary>
    /// 让上一个棋子取消停留，让新的棋子执行停留方法
    /// </summary>
    /// <param name="grid"></param>
    public void SetStayGrid(MapGrid grid)
    {
        StayGrid?.ChessAway();
        StayGrid = grid;
        StayGrid.StayChess();
        m_gameObject.transform.position = grid.transform.position;
    }

    public void LearnSkill() { }

    public void ForgetSkill() { }

    public void Release()
    {
        GameObject.Destroy(m_gameObject);
    }

    public void Move(List<MapGrid> grids, Action callback = null)
    {
        m_anim.Move(grids, callback);
    }
}
