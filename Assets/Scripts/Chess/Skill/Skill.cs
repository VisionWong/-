using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VFramework;

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

/// <summary>
/// 二维坐标
/// </summary>
public class Coord
{
    public int x, y;
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
    public List<Coord> range; //范围数组，表示坐标差值
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
    protected ChessAnimator _anim;
    protected IChess _chess;

    public Skill(SkillData data, IChess chess, Transform chessTrans)
    {
        Data = data;
        _chessTrans = chessTrans;
        _chess = chess;
        _anim = _chessTrans.GetComponent<ChessAnimator>();
    }

    public virtual void UseSkill(List<IChess> targets, Direction dir)
    {
        MonoMgr.Instance.StartCoroutine(PlayAnimation(dir));
    }

    /// <summary>
    /// 获取技能将要造成的结果，伤害或者回复
    /// </summary>
    public abstract int GetPreview(IChess target);//TODO 可能的效果还有击退等，得封装一个技能的效果

    protected virtual IEnumerator PlayAnimation(Direction dir)
    {
        //播放默认动画和默认音效
        _anim.ChangeForward(dir);
        var tee = _chessTrans.DOPunchPosition(EnumTool.DirToVector3(dir) * 0.7f, 1f, 3, 0.2f);
        yield return tee.WaitForCompletion();
        MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd);
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
        str.Append(Data.range[0].x + "," + Data.range[0].y);
        return str.ToString();
    }
}
