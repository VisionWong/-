﻿using System.Collections;
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

public enum SkillEffectType
{
    SuckBlood,  //吸血
    Sleep,      //睡眠,
    Burn,       //烧伤,
    Poison,     //中毒,
    Paralysis,  //麻痹,
    Freeze,     //冰冻,
    Confusion,  //混乱
    Fear,       //害怕
    //击退,
    FixedDamage, //固定伤害,
    AtkUp,      //上升攻击,
    DefUp,      //上升防御,
    APUp,       //上升行动力,
    AtkDown,    //下降攻击,
    DefDown,    //下降防御,
    APDown,     //下降行动力,
    AllUp,      //全上升
    SelfDamage, //自损
    Charge,     //蓄力
    OHK,        //一击必杀
}

/// <summary>
/// 二维坐标
/// </summary>
public struct Coord
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
    public bool isDebuff = false;
    public float fixedPercent;  //固定百分比
    public int hitTimes = 1; //攻击次数
    public int hitRate; //命中率,代表百分数
    public List<Coord> range; //范围数组，表示坐标差值
    public int targetNum;
    public List<SkillEffect> effects;
    public string audioPath;
    public string effectPath;
}


public class SkillEffect
{
    public SkillEffectType effectType;

    public bool isSelf = false; //是否作用自身
    public float probability;   //概率
    public int fixedDamage;     //固定伤害
    public float fixedPercent;  //固定百分比
    public int effectLevel;     //作用的能力等级
    public int effectTurns;     //作用的回合数
}

public class Skill
{
    public SkillData Data { get; set; }

    protected Transform _chessTrans;
    protected ChessAnimator _anim;
    protected IChess _chess;

    protected int _hitTimes;
    protected List<IChess> _effectableList = new List<IChess>();
    protected int _damage = 0;

    private int _lockNum = 0;
    private int _lockTarget = 0;

    public Skill(SkillData data)
    {
        Data = data;
        _hitTimes = data.hitTimes;
    }

    public void SetChess(IChess chess, Transform chessTrans)
    {
        _chessTrans = chessTrans;
        _chess = chess;
        _anim = _chessTrans.GetComponent<ChessAnimator>();
    }

    public void UseSkill(List<IChess> targets, Direction dir)
    {
        _hitTimes = Data.hitTimes;
        UseSkillRepeat(targets, dir);
    }

    protected void UseSkillRepeat(List<IChess> targets, Direction dir)
    {
        if (_hitTimes-- == 0 || targets.Count == 0)
        {
            MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd);
            return;
        }
        _effectableList = new List<IChess>(targets);
        SkillEffect(targets, dir);
        MonoMgr.Instance.StartCoroutine(PlayAnimation(dir));
    }

    public virtual void SkillEffect(List<IChess> targets, Direction dir)
    {
        switch (Data.skillType)
        {
            case SkillType.Damage:
                _lockTarget = targets.Count;
                foreach (var target in targets)
                {
                    if (!BattleSystem.Instance.IsChessAlive(target) || target.CanAvoid(_chess, Data, dir))
                    {
                        _effectableList.Remove(target);
                        ReadyToDoEffect(dir);
                    }
                    else
                    {
                        var damage = Formulas.CalSkillDamage(Data, _chess, target);
                        if (damage.type == DamageType.NoEffect)
                        {
                            target.Avoid(dir);
                            _effectableList.Remove(target);
                            ReadyToDoEffect(dir);
                        }
                        else
                        {
                            _damage = damage.num;
                            target.TakeDamage(damage.num, damage.type, dir, () => ReadyToDoEffect(dir));
                        }
                    }
                }
                break;
            case SkillType.Heal:
                foreach (var target in targets)
                {
                    if (Random.Range(0, 100) < Data.hitRate)
                        target.Healing(Data.power == 0 ? Data.fixedPercent : Data.power);
                }
                MonoMgr.Instance.StartCoroutine(DoSkillEffect(dir));
                //DoSkillEffect(dir);
                break;
            case SkillType.Effect:
                if (Data.rangeType != SkillRangeType.自身)
                {
                    foreach (var target in targets)
                    {
                        var damage = Formulas.CalSkillDamage(Data, _chess, target, false);
                        if (damage.type == DamageType.NoEffect)
                        {
                            target.Avoid(dir);
                            _effectableList.Remove(target);
                        }
                    }
                }
                MonoMgr.Instance.StartCoroutine(DoSkillEffect(dir));
                //DoSkillEffect(dir);
                break;
            default:
                Debug.LogError("该类型尚未实现" + Data.skillType);
                break;
        }
        
    }

    /// <summary>
    /// 获取技能将要造成的结果，伤害或者回复
    /// </summary>
    public virtual (int num, DamageType type) GetPreview(IChess target)
    {
        switch (Data.skillType)
        {
            case SkillType.Damage:
                //TODO 
                return Formulas.CalSkillDamage(Data, _chess, target, false);
            case SkillType.Heal:
                return ((int)(target.Attribute.MaxHP * (Data.power == 0 ? Data.fixedPercent : Data.power)), 0);
            case SkillType.Effect:
                return (0, 0);
            default:
                Debug.LogError("该类型尚未实现" + Data.skillType);
                return (0, 0);
        }
    }

    protected virtual IEnumerator PlayAnimation(Direction dir)
    {
        //播放默认动画和默认音效
        _anim.ChangeForward(dir);
        if (Data.skillType == SkillType.Damage)
        {
            var tee = _chessTrans.DOPunchPosition(EnumTool.DirToVector3(dir) * 0.7f, 1f, 3, 0.2f);
            yield return tee.WaitForCompletion();
        }
        else
        {
            var tee = _chessTrans.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1, 0.5f);
            yield return tee.WaitForCompletion();
            //MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd);
        }
    }

    private void ReadyToDoEffect(Direction dir)
    {
        _lockNum++;
        if (_lockNum == _lockTarget)
        {
            _lockNum = 0;
            //DoSkillEffect(dir);
            MonoMgr.Instance.StartCoroutine(DoSkillEffect(dir));
        }
    }

    //TODO 让特效演出完成再结算下一个特效 使用协程
    private IEnumerator DoSkillEffect(Direction dir)
    {
        //等伤害结算完处理技能特效
        if (Data.effects != null)
        {
            foreach (var effect in Data.effects)
            {
                yield return new WaitForSeconds(0.5f);
                switch (effect.effectType)
                {
                    case SkillEffectType.Sleep:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new SleepBuff(_chess, 2));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new SleepBuff(target, 2));
                            }
                        }
                        break;
                    case SkillEffectType.Freeze:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new FreezeBuff(_chess, 99));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new FreezeBuff(target, 99));
                            }
                        }
                        break;
                    case SkillEffectType.Confusion:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ConfusionBuff(_chess, effect.effectTurns));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ConfusionBuff(target, effect.effectTurns));
                            }
                        }
                        break;
                    case SkillEffectType.Paralysis:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ParalysisBuff(_chess, effect.effectTurns));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ParalysisBuff(target, effect.effectTurns));
                            }
                        }
                        break;
                    case SkillEffectType.Poison:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new PoisonBuff(_chess, 99));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new PoisonBuff(target, 99));
                            }
                        }
                        break;
                    case SkillEffectType.Burn:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new BurnBuff(_chess, 99));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new BurnBuff(target, 99));
                            }
                        }
                        break;
                    case SkillEffectType.Fear:
                        foreach (var target in _effectableList)
                        {
                            if (BattleSystem.Instance.IsChessAlive(target) && Random.Range(0, 1f) < effect.probability)
                                target.AddBuff(new FearBuff(target, 1));
                        }
                        break;
                    case SkillEffectType.SuckBlood:
                        _chess.Healing((int)(_damage * effect.fixedPercent));
                        break;
                    case SkillEffectType.AtkUp:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeAttackBuff(_chess, effect.effectTurns, effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeAttackBuff(target, effect.effectTurns, effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.AtkDown:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeAttackBuff(_chess, effect.effectTurns, -effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeAttackBuff(target, effect.effectTurns, -effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.DefUp:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeDefenceBuff(_chess, effect.effectTurns, effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeDefenceBuff(target, effect.effectTurns, effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.DefDown:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeDefenceBuff(_chess, effect.effectTurns, -effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeDefenceBuff(target, effect.effectTurns, -effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.APUp:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeAPBuff(_chess, effect.effectTurns, effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeAPBuff(target, effect.effectTurns, effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.APDown:
                        if (effect.isSelf)
                        {
                            if (Random.Range(0, 1f) < effect.probability)
                                _chess.AddBuff(new ChangeAPBuff(_chess, effect.effectTurns, -effect.effectLevel));
                        }
                        else
                        {
                            foreach (var target in _effectableList)
                            {
                                if (Random.Range(0, 1f) < effect.probability)
                                    target.AddBuff(new ChangeAPBuff(target, effect.effectTurns, -effect.effectLevel));
                            }
                        }
                        break;
                    case SkillEffectType.SelfDamage:
                        _chess.TakeDamage((int)(_damage * effect.fixedPercent), DamageType.Common, EnumTool.GetOppositeDir(dir));
                        break;
                    case SkillEffectType.FixedDamage:
                        //
                        break;
                    case SkillEffectType.OHK:
                        //foreach (var target in _effectableList)
                        //{
                        //    if (Random.Range(0, 1f) < 0.3f)
                        //        target.TakeDamage(target.Attribute.HP, DamageType.Common, dir);
                        //}
                        break;
                    default:
                        Debug.LogError("该类型尚未实现:" + effect.effectType);
                        break;
                }
            }
        }
        UseSkillRepeat(_effectableList, dir);
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
