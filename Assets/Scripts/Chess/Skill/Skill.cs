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

public class SkillData : IData
{
    public string name;
    public string description;
    public PMType pmType;
    public SkillType skillType;
    public SkillRangeType rangeType;
    public int power; //威力，变化类为0
    public int hitRate; //命中率,代表百分数
    public List<(int x, int y)> range; //范围数组，表示坐标差值
    public int targetNum;
    public List<SkillEffect> effects;
    public string audioPath;
    public string effectPath;
}


public class SkillEffect
{
    //public enum EffectType
    //{
    //    睡眠,
    //    烧伤,
    //    中毒,
    //    麻痹,
    //    冰冻,
    //    击退,
    //    固定伤害,
    //    上升攻击,
    //    上升防御,
    //    上升行动力,

        
    //}
    //public List<EffectType> effects;
    public int probability; //概率
    public int fixedDamage; //固定伤害
    public int effectLevel; //作用的能力等级
    public int effectTurns; //作用的回合数
}

public abstract class Skill
{
    public SkillData Data { get; set; }

    protected Transform _chessTrans;
    protected IChess _chess;

    public Skill(SkillData data, IChess chess, Transform chessTrans)
    {
        Data = data;
        _chessTrans = chessTrans;
        _chess = chess;
    }

    public virtual void UseSkill(List<IChess> targets)
    {
        PlayAnimation();
    }

    public virtual void PlayAnimation()
    {
        //播放默认动画和默认音效
    }

    public override string ToString()
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append(Data.id + " ");
        str.Append(Data.name + " ");
        str.Append(Data.pmType + " ");
        str.Append(Data.skillType + " ");
        str.Append(Data.rangeType + " ");
        str.Append(Data.power + " ");
        str.Append(Data.range[0].x + " ");
        return str.ToString();
    }
}
