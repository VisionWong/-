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
    #region 私有字段
    private int _maxHp;
    private int _ap;
    private int _atk;
    private int _def;
    private int _critRate;
    private int _avoidRate;
    #endregion

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
    public int AP
    {
        get { return Mathf.Clamp(_ap + addiAttr.AP, 1, int.MaxValue); }
        private set { _ap = value; }
    }
    /// <summary>
    /// 性别，0为母，1为公
    /// </summary>
    public int Gender { get; private set; }

    public int HP { get; private set; }
    public int MaxHP
    {
        get { return _maxHp + addiAttr.HP; }
        private set { _maxHp = value; }
    }
    public int Attack
    {
        get { return (int)(_atk * addiAttr.AtkFactor.ToFloat()); }
        private set { _atk = value; }
    }
    public int Defence
    {
        get { return (int)(_def * addiAttr.DefFactor.ToFloat()); }
        private set { _def = value; }
    }
    /// <summary>
    /// 暴击率
    /// </summary>
    public int CritRate
    {
        get { return Mathf.Clamp(_critRate + addiAttr.CritRate, 0, 100); }
        private set { _critRate = value; }
    }
    /// <summary>
    /// 回避率
    /// </summary>
    public int AvoidRate
    {
        get { return Mathf.Clamp(_avoidRate + addiAttr.AvoidRate, 0, 100); }
        private set { _avoidRate = value; }
    }

    private AdditionalAttr addiAttr = new AdditionalAttr();

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

    #region 额外属性变化
    public void ChangeAttackLevel(int level)
    {
        if (level < 0)
            addiAttr.AtkFactor.LevelDown(-level);
        else if (level > 0)
            addiAttr.AtkFactor.LevelUp(level);
    }
    public void ChangeDefenceLevel(int level)
    {
        if (level < 0)
            addiAttr.DefFactor.LevelDown(-level);
        else if (level > 0)
            addiAttr.DefFactor.LevelUp(level);
    }
    public void ChangeAP(int num)
    {
        addiAttr.AP += num;
    }
    public void ChangeMaxHP(int num)
    {
        addiAttr.HP += num;
        if (HP > MaxHP) HP = MaxHP;
    }
    public void ChangeCritRate(int num)
    {
        addiAttr.CritRate += num;
    }
    public void ChangeAvoidRate(int num)
    {
        addiAttr.CritRate += num;
    }
    #endregion
}

public class AdditionalAttr
{
    public int HP { get; set; }
    public Fraction AtkFactor { get; set; }
    public Fraction DefFactor { get; set; }
    public int CritRate { get; set; }
    public int AvoidRate { get; set; }
    public int AP { get; set; }

    public AdditionalAttr()
    {
        HP = 0;
        AtkFactor = new Fraction();
        DefFactor = new Fraction();
        CritRate = 0;
        AvoidRate = 0;
        AP = 0;
    }

    public void Clear()
    {
        HP = 0;
        AtkFactor.Restore();
        DefFactor.Restore();
        CritRate = 0;
        AvoidRate = 0;
        AP = 0;
    }
}

public class Fraction
{
    private int son;
    private int mom;

    public Fraction()
    {
        Restore();
    }
    public float ToFloat()
    {
        return son / mom;
    }
    public void Restore()
    {
        son = 2;
        mom = 2;
    }
    public void LevelUp(int level)
    {
        son += level;
    }
    public void LevelDown(int level)
    {
        mom += level;
    }
}
