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

    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }
    /// <summary>
    /// 暴击率
    /// </summary>
    public int CritRate { get; set; }
    /// <summary>
    /// 回避率
    /// </summary>
    public int AvoidRate { get; set; }

    //TODO
    public IChessAttr() { }
}
