using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公式计算类
/// </summary>
public static class Formulas
{
    public static int CalSkillDamage(SkillData attr, IChess user, IChess target, bool canCrit = true)
    {
        float damage = 1;
        if (attr.pmType == user.Attribute.PMType1 || attr.pmType == user.Attribute.PMType2)
        {
            damage *= 1.5f;
        }
        //计算属性克制
        damage *= GetPMTypeRestraint(attr.pmType, target.Attribute.PMType1);
        if (target.Attribute.PMType2 != PMType.None) damage *= GetPMTypeRestraint(attr.pmType, target.Attribute.PMType2);
        if (damage == 0) return 0;
        //判断是否暴击
        if (canCrit && Random.Range(0, 100) < user.Attribute.CritRate) damage *= 1.5f;
        float num = user.Attribute.Attack - target.Attribute.Defence;
        if (num <= 0) num = 1;
        damage *= attr.power * num / 50;
        return (int)damage;
    }

    public static float GetPMTypeRestraint(PMType userType, PMType targetType)
    {
        return 1;
    }
}
