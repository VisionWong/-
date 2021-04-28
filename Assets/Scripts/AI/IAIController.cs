using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAIController
{
    protected IChess _chess;
    protected List<IChess> _targetList;
    protected Map _map;

    protected (List<IChess> chessList, MapGrid grid, Direction dir) _target;
    protected Skill _skillToUse = null;
    protected MapGrid _stayGrid = null;//用于储存追逐目标但无法到达的寻路路径终点格子

    public IAIController(List<IChess> targetList, Map map)
    {
        _targetList = targetList;
        _map = map;
    }

    public void SetChess(IChess chess)
    {
        _chess = chess;
    }

    //行动逻辑 寻敌、技能选择、移动、(使用技能)
    public virtual void StartAction()
    {
        Camera.main.GetComponent<CameraController>().MoveToTarget(_chess.GameObject.transform.position);
    }

    public void OnActionEnd()
    {
        if (_stayGrid != null)
            _chess.SetStayGrid(_stayGrid);
        _chess.OnActionEnd();

        _skillToUse = null;
        _stayGrid = null;
    }

    /// <summary>
    /// 寻敌逻辑，返回该技能能作用到的所有目标群与其对应的使用位置和使用方向
    /// </summary>
    /// <param name="chess"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    protected abstract List<(List<IChess> chessList, MapGrid grid, Direction dir)> SearchTarget(IChess chess, Skill skill);

    /// <summary>
    /// 获取无法使用技能时选择靠近的目标
    /// </summary>
    /// <returns></returns>
    protected abstract IChess GetTheTargetToClose();

    protected void UseSkill(Skill skill)
    {
        if (skill.Data.rangeType == SkillRangeType.自身)
            skill.UseSkill(new List<IChess> { _chess }, Direction.Down);
        else
            skill.UseSkill(_target.chessList, _target.dir);
    }
}
