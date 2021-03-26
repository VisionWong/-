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
    private List<ISkill> m_skillList = new List<ISkill>();

    public IChess(IChessAttr attr, GameObject go)
    {
        Attribute = attr;
        m_gameObject = go;
    }

    public void SetPathPack(PathPack pack)
    {
        PathPack = pack;
    }

    public void SetStayGrid(MapGrid grid)
    {
        StayGrid = grid;
        StayGrid.StayChess();
    }

    public void LearnSkill() { }

    public void ForgetSkill() { }

    public void Release()
    {
        GameObject.Destroy(m_gameObject);
    }
}
