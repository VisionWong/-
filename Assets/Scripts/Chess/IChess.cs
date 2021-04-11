using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝可梦克制相关的属性
/// </summary>
public enum PMType
{
    None,
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

    public string Tag
    {
        get
        {
            return _gameObject?.tag;
        }
    }

    protected GameObject _gameObject;
    protected HUD _hud;//UI显示
    protected ChessAnimator _anim = null; 

    public IChess(ChessAttr attr, GameObject go)
    {
        Attribute = attr;
        _gameObject = go;
        _hud = _gameObject.AddComponent<HUD>();
        SkillList = new List<Skill>();
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
        if (isChangePos) _gameObject.transform.position = grid.transform.position;
    }

    public void LearnSkill(Skill skill)
    {
        if (SkillList.Count < 3)
        {
            skill.SetChess(this, _gameObject.transform);
            SkillList.Add(skill);
            //TODO 提示UI
        }
        else ForgetSkill(skill);
    }

    public void ForgetSkill(Skill skill) { }

    public void Release()
    {
        GameObject.Destroy(_gameObject);
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

    public void TakeDamage(int damage, Direction dir)
    {
        //判断是否死亡
        if (Attribute.TakeDamage(Formulas.CalRealDamage(damage, this)))
        {

        }
        _anim.TakeDamage(dir);
        _hud.ChangeHPValue(Attribute.HP, Attribute.MaxHP);
    }

    public void Healing(int num)
    {
        Attribute.Healing(num);
        _anim.Healing();
        _hud.ChangeHPValue(Attribute.HP, Attribute.MaxHP);
    }
    public void Healing(float percent)
    {
        Attribute.Healing(percent);
        _anim.Healing();
        _hud.ChangeHPValue(Attribute.HP, Attribute.MaxHP);
    }

    //显示收到技能可能出现的效果
    public void ShowPreview(int num, Skill skill)
    {
        switch (skill.Data.skillType)
        {
            case SkillType.Damage:
                int realNum = Attribute.HP - Formulas.CalRealDamage(num, this);
                _hud.ShowPreview(realNum < 0 ? 0 : realNum, Attribute.MaxHP);
                break;
            case SkillType.Heal:
                //TODO 可能有治疗增益
                int realNum2 = Attribute.HP + num;
                _hud.ShowPreview(realNum2, Attribute.MaxHP);
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

    public void Sleep()
    {
        throw new NotImplementedException();
    }

    public void Paralyzed()
    {
        throw new NotImplementedException();
    }

    public void Burned()
    {
        throw new NotImplementedException();
    }

    public void Poisoned()
    {
        throw new NotImplementedException();
    }

    public void Freezed()
    {
        throw new NotImplementedException();
    }
    #endregion
}
