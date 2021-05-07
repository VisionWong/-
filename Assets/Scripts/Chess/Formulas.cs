using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    NoEffect,
    HalfEffective,
    Common,
    Effective,
    Poison,
    Burn
}

/// <summary>
/// 公式计算类
/// </summary>
public static class Formulas
{
    /// <summary>
    /// 计算技能伤害，返回伤害数字和伤害克制类别
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="canCrit"></param>
    /// <returns>克制类别0为无效，1为不是很有效，2为正常，3为效果拔群</returns>
    public static (int num, DamageType type) CalSkillDamage(SkillData attr, IChess user, IChess target, bool canCrit = true)
    {
        float damage = 1;
        DamageType type = DamageType.NoEffect;
        //计算属性克制
        damage *= GetPMTypeRestraint(attr.pmType, target.Attribute.PMType1);
        if (target.Attribute.PMType2 != PMType.None) damage *= GetPMTypeRestraint(attr.pmType, target.Attribute.PMType2);
        if (Mathf.Abs(damage) <= float.Epsilon) return (0, type);
        else if (Mathf.Abs(damage - 0.5f) <= float.Epsilon) type = DamageType.HalfEffective;
        else if (Mathf.Abs(damage - 1f) <= float.Epsilon) type = DamageType.Common;
        else type = DamageType.Effective;
        //计算本属性加成
        if (attr.pmType == user.Attribute.PMType1 || attr.pmType == user.Attribute.PMType2)
        {
            damage *= 1.5f;
        }
        //判断是否暴击
        if (canCrit && Random.Range(0, 100) < user.Attribute.CritRate) damage *= 1.5f;
        float num = user.Attribute.Attack - target.Attribute.Defence;
        if (num <= 0) num = 1;
        damage *= attr.power * num / 50f;
        return (Mathf.RoundToInt(damage), type);
    }

    public static int CalHealingNum(SkillData attr, IChess user, IChess target)
    {
        float weight = 1;
        //TODO 计算特性和道具的影响
        if (attr.power != 0) return (int)(attr.power * weight);
        return (int)(attr.fixedPercent * target.Attribute.MaxHP * weight);
    }

    /// <summary>
    /// 获取治疗技能的权重，低于半血时权重超过1，血越低权重越高
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float GetHealingWeight(IChess target)
    {
        var attr = target.Attribute;
        return ((float)attr.MaxHP / attr.HP) * 0.5f;
    }


    //private static Dictionary<PMType, HashSet<PMType>> effectiveDict;
    //private static Dictionary<PMType, HashSet<PMType>> halfEffectiveDict;
    //private static Dictionary<PMType, HashSet<PMType>> noEffectiveDict;
    public static float GetPMTypeRestraint(PMType attackerType, PMType targetType)
    {
        var typeData = PMTypeLib.Instance.GetData((int)attackerType);
        if (typeData.effectiveSet.Contains(targetType))
            return 2f;
        else if (typeData.halfEffectiveSet.Contains(targetType))
            return 0.5f;
        else if (typeData.noEffectiSet.Contains(targetType))
            return 0f;
        return 1f;
    }
}

