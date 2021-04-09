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

    public void TakeDamage(int damage)
    {
        Attribute.TakeDamage(Formulas.CalRealDamage(damage, this));
        //TODO 判断是否死亡
        _hud.ChangeHPValue(Attribute.HP, Attribute.MaxHP);
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
