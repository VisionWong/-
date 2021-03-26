using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EvolutionStage
{
    First,
    Second,
    Final,
}

public abstract class IChessAttr
{
    public PMType PMType { get; set; }
    public IAbility Ability { get; set; }

    /// <summary>
    /// 中文名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 行动力，可移动的步数
    /// </summary>
    public int AP { get; set; }
    /// <summary>
    /// 性别，0为母，1为公
    /// </summary>
    public int Gender { get; set; }

    private int m_hp;
    private int m_maxHp;
    private int m_attack;
    private int m_defence;
    /// <summary>
    /// 暴击率
    /// </summary>
    private float m_critRate;
    /// <summary>
    /// 回避率
    /// </summary>
    private float m_avoidRate;

    //TODO
    public IChessAttr() { }
}
