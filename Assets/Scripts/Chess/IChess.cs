﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

/// <summary>
/// 宝可梦克制相关的属性
/// </summary>
public enum PMType
{
    None = 0,
    Common,
    Grass,
    Fire,
    Water,
    Electric,
    Fly,
    Fight,
    Ground,
    Rock,
    Bug,
    Ice,
    Ghost,
    Psychic,
    Poison,
    Dark,
    Dragon,
    Metal,
    Fairy,  
}

public abstract class IChess : IAttackable
{
    public PathPack PathPack { get; private set; }
    public ChessAttr Attribute { get; set; }
    public MapGrid StayGrid { get; private set; }
    public List<Skill> SkillList { get; private set; }

    public GameObject GameObject { get; set; }
    public string Tag
    {
        get
        {
            return GameObject?.tag;
        }
    }

    protected HUD _hud;//UI显示
    protected ChessAnimator _anim;
    private List<IBuff> _buffList;

    public IChess(ChessAttr attr, GameObject go)
    {
        Attribute = attr;
        SkillList = new List<Skill>();
        GameObject = go;
        _hud = GameObject.AddComponent<HUD>();
        _hud.SetHPBarColor(Tag);
        _buffList = new List<IBuff>();
    }

    public void SetPathPack(PathPack pack)
    {
        PathPack = pack;
    }

    public void SetAnimator(ChessAnimator anim)
    {
        _anim = anim;
    }

    /// <summary>
    /// 让上一个棋子取消停留，让新的棋子执行停留方法
    /// </summary>
    /// <param name="grid"></param>
    public void SetStayGrid(MapGrid grid, bool isChangePos = true)
    {
        StayGrid?.ChessAway();
        StayGrid = grid;
        StayGrid.StayChess(this);
        if (isChangePos) GameObject.transform.position = grid.transform.position;
    }

    public void LearnSkill(Skill skill)
    {
        if (SkillList.Count < 4)
        {
            skill.SetChess(this, GameObject.transform);
            SkillList.Add(skill);
            //TODO 提示UI
        }
        else ForgetSkill(skill);
    }

    public void ForgetSkill(Skill skill) { }

    public void Release()
    {
        GameObject.Destroy(GameObject);
    }

    #region 战斗相关
    public void Move(List<MapGrid> grids, Action callback = null)
    {
        _anim.Move(grids, callback);
    }

    public void CancelMove()
    {
        _anim.CancelMove();
    }

    public bool CanAvoid(IChess attacker, SkillData data, Direction dir)
    {
        if (data.effects != null)
        {
            foreach (var item in data.effects)
            {
                if (item.effectType == SkillEffectType.OHK)
                {
                    if (UnityEngine.Random.Range(0, 1f) >= 0.3f)
                    {
                        _anim.AvoidDamage(dir);
                        _hud.NoticeAvoid();
                        return true;
                    }
                    else
                        return false;
                }
            }
        }
        //根据攻击者的命中等级计算自身回避等级
        if (UnityEngine.Random.Range(0, 100) < Attribute.AvoidRate || UnityEngine.Random.Range(0, 100) >= data.hitRate)
        {
            //播放闪避动画
            _anim.AvoidDamage(dir);
            _hud.NoticeAvoid();
            return true;
        }
        return false;
    }

    public void Avoid(Direction dir)
    {
        //播放闪避动画
        _anim.AvoidDamage(dir);
        _hud.NoticeAvoid();
    }

    public void NoticeWord(string str)
    {
        _hud.NoticeWords(str);
    }

    public void TakeDamage(int damage, DamageType type, Direction dir, Action callback = null)
    {
        //判断是否死亡
        if (Attribute.TakeDamage(damage))
        {
            Dead();
            callback?.Invoke();
            callback = null;
        }
        _anim.TakeDamage(dir);
        _hud.ChangeHPValueByDamage(Attribute.HP, Attribute.MaxHP, damage, type, callback);
    }

    public void Healing(int num)
    {
        Attribute.Healing(num);
        _anim.Healing();
        _hud.ChangeHPValueByHealing(Attribute.HP, Attribute.MaxHP, num);
    }
    public void Healing(float percent)
    {
        Attribute.Healing(percent);
        _anim.Healing();
        _hud.ChangeHPValueByHealing(Attribute.HP, Attribute.MaxHP, (int)(percent * Attribute.MaxHP));
    }

    //显示收到技能可能出现的效果
    public void ShowPreview((int num, DamageType type) effect, Skill skill)
    {
        switch (skill.Data.skillType)
        {
            case SkillType.Damage:
                int realNum = Attribute.HP - effect.num;
                _hud.ShowDamagePreview(realNum < 0 ? 0 : realNum, Attribute.MaxHP, effect.num, effect.type);
                break;
            case SkillType.Heal:
                //TODO 可能有治疗增益
                int realNum2 = Attribute.HP + effect.num;
                _hud.ShowHealPreview(realNum2, Attribute.MaxHP, effect.num);
                break;
            case SkillType.Effect:
                //暂时不做任何提示
                break;
            default:
                Debug.LogError("该技能类型还未实现");
                break;
        }
        //进一步判断可能有击退等效果

    }
    public void HidePreview()
    {
        _hud.HidePreview();
    }

    public void OnUseSkill(Skill skill)
    {
        _hud.NoticeUseSkill(skill.Data.name);
    }

    #region BUFF处理
    public void AddBuff(IBuff buff)
    {
        //TODO 不能出现重复异常BUFF
        if (buff is IDebuff)
        {
            Debug.Log(buff.BuffType.ToString());
            if (ContainsBuff(buff.BuffType))
                return;
        }
        Debug.Log(string.Format("buff:{0}添加成功", buff.ToString()));
        _buffList.Add(buff);
        buff.OnBuffBegin();
    }
    public void RemoveBuff(IBuff buff)
    {
        _buffList.Remove(buff);
    }
    public bool ContainsBuff(BuffType type)
    {
        foreach (var buff in _buffList)
        {
            Debug.Log(buff.BuffType.ToString() + " : " + type);
            if (buff.BuffType == type)
                return true;
        }
        return false;
    }
    public void OnTurnStart()
    {
        if (_buffList != null)
        {
            List<IBuff> tempList = new List<IBuff>(_buffList);
            foreach (var buff in tempList)
            {
                buff.OnTurnStart();
            }
        }
    }
    public void OnActionEnd()
    {
        if (_buffList != null)
        {
            List<IBuff> tempList = new List<IBuff>(_buffList);
            foreach (var buff in tempList)
            {
                buff.OnTurnEnd();
            }
        }
    }
    #endregion

    private IEnumerator WaitForDebuffAnim(Action callback = null)
    {
        yield return new WaitForSeconds(1.5f);
        callback?.Invoke();
    }

    public void Fear()
    {
        //跳过该行动回合
        //TODO 播放动画
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        _hud.NoticeWords("害怕不能动");
        BattleSystem.Instance.OnChessActionEnd(this);
    }

    public void Sleep()
    {
        //跳过该行动回合
        //TODO 播放睡眠动画
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        _hud.NoticeWords("正在睡觉zzz");
        BattleSystem.Instance.OnChessActionEnd(this);
    }

    public void Paralyzed()
    {
        //跳过该行动回合
        //TODO 播放动画
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        _hud.NoticeWords("麻痹不能动");
        BattleSystem.Instance.OnChessActionEnd(this);
    }

    public void Burned()
    {
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        //TODO 播放动画
        if (Attribute.TakeDamage(0.0625f))
        {
            Dead();
        }
        //TODO 播放中毒受伤动画
        _hud.NoticeWords("烧伤了");
        _hud.ChangeHPValueByDamage(Attribute.HP, Attribute.MaxHP, (int)(0.0625f * Attribute.MaxHP), DamageType.Burn);
        MonoMgr.Instance.StartCoroutine(WaitForDebuffAnim());
    }

    public void Poisoned(float bloodPer)
    {
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        if (Attribute.TakeDamage(bloodPer))
        {
            Dead();
        }
        //TODO 播放中毒受伤动画
        _hud.NoticeWords("中毒了");
        _hud.ChangeHPValueByDamage(Attribute.HP, Attribute.MaxHP, (int)(bloodPer * Attribute.MaxHP), DamageType.Poison);
        MonoMgr.Instance.StartCoroutine(WaitForDebuffAnim());
    }

    public void Freezed()
    {
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        //跳过该行动回合
        //TODO 播放冰冻动画
        _hud.NoticeWords("被冻住不能动");
        BattleSystem.Instance.OnChessActionEnd(this);
    }

    public void Confused()
    {
        Camera.main.GetComponent<CameraController>().MoveToTarget(GameObject.transform.position);
        //跳过该行动回合
        //攻击自己
        if (Attribute.TakeDamage(0.0625f))
        {
            Dead();
        }
        //TODO 播放中毒受伤动画
        _hud.ChangeHPValueByDamage(Attribute.HP, Attribute.MaxHP, (int)(0.0625f * Attribute.MaxHP), DamageType.Common);
        _hud.NoticeWords("混乱中自损了");
        BattleSystem.Instance.OnChessActionEnd(this);
    }

    public void Dead()
    {
        //TODO 通知战斗系统剔除自己
        BattleSystem.Instance.OnChessDead(this);
        //播放死亡动画，死亡特效，演出结束后消失
        StayGrid.ChessAway();
        _anim.Dead(() => GameObject.SetActive(false));
    }
    #endregion
}
