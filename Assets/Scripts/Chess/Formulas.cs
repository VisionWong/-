using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公式计算类
/// </summary>
public static class Formulas
{
    public static int CalSkillDamage(SkillData attr, IChess user, IChess target)
    {
        float damage = 1;
        if (attr.pmType == user.Attribute.PMType1 || attr.pmType == user.Attribute.PMType2)
        {
            damage *= 1.5f;
        }
        float num = user.Attribute.Attack - target.Attribute.Defence;
        if (num <= 0) num = 1;
        damage *= attr.power * num / 50;
        //TODO 判断是否暴击
        return (int)damage;
    }

    public static int CalRealDamage(int damage, IChess target)
    {
        float realDamage = damage;
        //TODO 计算属性克制和是否回避
        return (int)realDamage;
    }
}
