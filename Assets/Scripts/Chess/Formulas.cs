using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公式计算类
/// </summary>
public static class Formulas
{
    public static int CalSkillDamage(SkillAttr attr, IChess user)
    {
        float damage = 1;
        if (attr.PMType == user.Attribute.PMType)
        {
            damage *= 1.5f;
        }
        damage *= attr.Power * user.Attribute.Attack / 100;
        //TODO 判断是否暴击
        return (int)damage;
    }

    public static int CalRealDamage(int damage, IChess target)
    {
        float realDamage = 1;
        //TODO 计算属性克制和是否回避
        return (int)realDamage;
    }
}
