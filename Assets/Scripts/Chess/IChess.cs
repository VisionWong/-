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
    Electric,
    Fly,
    Fight,
    Ground,
    Rock,
    Bug,
    Ice,
    Ghost,
    Psychic,
    Poison,
    Dark,
    Dragon,
    Metal,
    Fairy
}
public abstract class IChess : IAttackable
{
    public PathPack PathPack { get; private set; }
    public IChessAttr Attribute { get; set; }
    public MapGrid StayGrid { get; private set; }

    protected GameObject m_gameObject;
    protected List<Skill> m_skillList = new List<Skill>();
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

    #region 战斗相关
    public void Move(List<MapGrid> grids, Action callback = null)
    {
        m_anim.Move(grids, callback);
    }

    public void TakeDamage(int damage)
    {
        
    }

    public void Sleep()
    {
        throw new NotImplementedException();
    }

    public void Paralyzed()
    {
        throw new NotImplementedException();
    }

    public void Burned()
    {
        throw new NotImplementedException();
    }

    public void Poisoned()
    {
        throw new NotImplementedException();
    }

    public void Freezed()
    {
        throw new NotImplementedException();
    }
    #endregion
}
