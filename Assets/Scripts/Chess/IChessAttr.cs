using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EvolutionStage
{
    First,
    Second,
    Final,
}

public class ChessData : IData
{
    public string name;
    public PMType pmType1;
    public PMType pmType2 = PMType.None;
    public List<int> abilityIdList;
    public int skillId;//自带的技能ID
    public int evoTaskId;//进化条件
    public EvolutionStage evoStage;
    public int hp;
    public int ap;
    public int attack;
    public int defence;
    public string spritePath;
    public string prefabPath;
    public string audioPath;
}

public class ChessAttr
{
    public int ID { get; private set; }
    public PMType PMType1 { get; private set; }
    public PMType PMType2 { get; private set; }
    public IAbility Ability { get; private set; }

    /// <summary>
    /// 中文名
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 行动力，可移动的步数
    /// </summary>
    public int AP { get; private set; }
    /// <summary>
    /// 性别，0为母，1为公
    /// </summary>
    public int Gender { get; private set; }

    public int HP { get; private set; }
    public int MaxHP { get; private set; }
    public int Attack { get; private set; }
    public int Defence { get; private set; }
    /// <summary>
    /// 暴击率
    /// </summary>
    public int CritRate { get; private set; }
    /// <summary>
    /// 回避率
    /// </summary>
    public int AvoidRate { get; private set; }

    //TODO
    public ChessAttr(ChessData data)
    {
        ID = data.id;
        PMType1 = data.pmType1;
        PMType2 = data.pmType2;
        //选择一个特性
        //选择一个性别
        Name = data.name;
        MaxHP = data.hp;
        HP = MaxHP;
        AP = data.ap;
        Attack = data.attack;
        Defence = data.defence;
        //以下取默认值
        CritRate = 25;
        AvoidRate = 0;
    }

    /// <summary>
    /// 若死亡返回True
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public bool TakeDamage(int damage)
    {
        if (HP - damage <= 0)
        {
            HP = 0;
            return true;
        }
        HP -= damage;
        return false;
    }

    public void Healing(int num)
    {
        HP += num;
        if (HP > MaxHP) HP = MaxHP;
    }
    public void Healing(float percent)
    {
        float num = percent * MaxHP;
        HP += (int)num;
        if (HP > MaxHP) HP = MaxHP;
    }
}
