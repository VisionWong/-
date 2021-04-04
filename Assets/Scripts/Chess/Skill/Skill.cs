using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillRangeType
{ 
    指向性,
    非指向性,
    自身
}

public enum SkillType
{
    Damage, //伤害
    Heal,   //治疗
    Effect, //变化
}

public class SkillAttr
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public PMType PMType {get;set;}
    public SkillType SkillType { get; set; }
    public SkillRangeType RangeType { get; set; }
    public int Power { get; set; } //威力，变化类为0
    public int HitRate { get; set; } //命中率,代表百分数
    public List<(int, int)> Range { get; set; } //范围数组，表示坐标差值
    public int TargetNum { get; set; }
    public List<SkillEffect> Effects { get; set; }
    public string AudioPath { get; set; }
    public string EffectPath { get; set; }
}


public class SkillEffect
{
    public enum EffectType
    {
        睡眠,
        烧伤,
        中毒,
        麻痹,
        冰冻,
        击退,
        固定伤害,
        上升攻击,
        上升防御,
        上升行动力,

        
    }

    public List<EffectType> effects;
    public int probability;
    public int fixedDamage; //固定伤害
    public int effectLevel; //作用的能力等级
    public int effectTurns; //作用的回合数
}

public abstract class Skill
{
    public SkillAttr Attr { get; set; }

    private Transform _chessTrans;
    private IChess _chess;

    public Skill(SkillAttr attr, IChess chess, Transform chessTrans)
    {
        Attr = attr;
        _chessTrans = chessTrans;
        _chess = chess;
    }

    public abstract void Effect(List<IChess> targets);

    public virtual void PlayAnimation()
    {
        //播放默认动画和默认音效
    }
}
