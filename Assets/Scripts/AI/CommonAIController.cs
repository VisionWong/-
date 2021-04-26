using System.Collections.Generic;
using UnityEngine;
using VFramework;

/// <summary>
/// 普通AI搜寻能造成最大伤害的敌人攻击，停留格子不做判断;
/// 使用技能的优先级为伤害，治疗，变化;
/// </summary>
public class CommonAIController : IAIController
{
    private HashSet<Skill> _buffSkillSet;
    private Dictionary<Skill, int> _buffToTimesDict;//增益技能限制使用3次

    public CommonAIController(IChess chess, List<IChess> targetList, Map map) : base(chess, targetList, map)
    {
        _buffSkillSet = new HashSet<Skill>();
        _buffToTimesDict = new Dictionary<Skill, int>();
    }

    public override void StartAction()
    {
        int curWeight = 0;
        //根据拥有的技能搜索可以作用到的目标，并计算权重，决定使用哪个技能
        foreach (var skill in _chess.SkillList)
        {
            (IChess chess, MapGrid grid) tempTarget = (null, null);
            switch (skill.Data.skillType)
            {
                case SkillType.Damage:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高伤害，权重为1
                    var targets = SearchTarget(_chess, skill, TagDefine.PLAYER);
                    if (targets == null) break;
                    int maxDamage = int.MinValue;
                    foreach (var target in targets)
                    {
                        int damage = Formulas.CalSkillDamage(skill.Data, _chess, target.chess);
                        if (damage > maxDamage)
                        {
                            maxDamage = damage;
                            tempTarget = target;
                        }
                    }
                    if (maxDamage > curWeight)
                    {
                        curWeight = maxDamage;
                        _skillToUse = skill;
                        _target = tempTarget;
                    }
                    break;
                case SkillType.Heal:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高治疗，越残血权重越高
                    var healTargets = SearchTarget(_chess, skill, TagDefine.ENEMY);
                    int maxHeal = int.MinValue;
                    if (healTargets == null) break;
                    foreach (var target in healTargets)
                    {
                        int heal = (int)(Formulas.CalHealingNum(skill.Data, _chess, target.chess) * Formulas.GetHealingWeight(target.chess));
                        if (heal > maxHeal)
                        {
                            maxHeal = heal;
                            tempTarget = target;
                        }
                    }
                    if (maxHeal > curWeight)
                    {
                        curWeight = maxHeal;
                        _skillToUse = skill;
                        _target = tempTarget;
                    }
                    break;
                case SkillType.Effect:
                    //自身作用的buff技能先加入后备列表，若无法攻击敌人再使用
                    if (skill.Data.rangeType == SkillRangeType.自身)
                    {
                        if (!_buffSkillSet.Contains(skill))
                        {
                            _buffSkillSet.Add(skill);
                            _buffToTimesDict.Add(skill, 0);
                        }                       
                    }
                    else
                    {
                        if (skill.Data.isDebuff)
                        {
                            //Debuff技能与伤害技能一样，固定权重(暂定为自身血量一半
                            var effectTargets = SearchTarget(_chess, skill, TagDefine.PLAYER);
                            if (effectTargets == null) break;
                            foreach (var target in effectTargets)
                            {
                                //TODO 只要有一个debuff状态能作用即可
                                foreach (var effect in skill.Data.effects)
                                {
                                    if (!target.chess.ContainsBuff(EnumTool.EffectTypeToBuffType(effect.effectType)))
                                    {
                                        tempTarget = target;
                                        break;
                                    }
                                }
                            }
                            //不存在Debuff能作用的棋子
                            if (tempTarget == (null, null)) break;
                            int effectWeight = _chess.Attribute.MaxHP / 2;
                            if (effectWeight > curWeight)
                            {
                                curWeight = effectWeight;
                                _skillToUse = skill;
                                _target = tempTarget;
                            }
                        }
                        else
                        {
                            //TODO 对其他单位的Buff技能需要判断对方是否超出数值

                        }

                    }
                    break;
                default:
                    Debug.LogError("该类型尚未实现" + skill.Data.skillType);
                    break;
            }
            if (_skillToUse != null)
            {
                Debug.Log(string.Format("当前计算的技能为{0},其权重为{1}", skill.Data.name, curWeight));
            }
        }//确定使用哪个技能
        //若没有能使用的技能，则选择使用增益技能或者靠近目标
        if (_skillToUse == null)
        {
            foreach (var item in _buffToTimesDict)
            {
                if (item.Value < 3)
                {
                    _buffToTimesDict[item.Key] += 1;
                    UseSkill(item.Key);
                    return;
                }
            }//没有能使用的增益技能，选择靠近目标
            var path = _map.PathFinding(_chess, _chess.StayGrid, _target.grid, new AStarPathFinding(), _chess.Attribute.AP);
            _stayGrid = path[path.Count - 1];//获取终点格子
            _chess.Move(path, OnActionEnd);
            return;
        }
        //否则前往目标使用技能
        _chess.Move(_map.PathFinding(_chess, _chess.StayGrid, _target.grid, new AStarPathFinding()), OnMoveEnd);
    }

    private void UseSkill(Skill skill)
    {
        if (skill.Data.rangeType == SkillRangeType.自身)
        {
            skill.UseSkill(new List<IChess> { _chess }, Direction.None);
        }
        //技能结束
        //skill.UseSkill()
    }

    private void OnMoveEnd()
    {
        UseSkill(_skillToUse);
    }

    private void OnActionEnd()
    {
        _chess.SetStayGrid(_stayGrid);
        //_chess.ChangeToActionEnd();
        _chess.OnActionEnd();
        MessageCenter.Instance.Broadcast(MessageType.OnEnemyChessActionEnd);
    }

    protected override List<(IChess chess, MapGrid grid)> SearchTarget(IChess chess, Skill skill, string tag)
    {
        List<(IChess, MapGrid)> targetList = new List<(IChess, MapGrid)>();
        int pathNum = chess.Attribute.AP;
        var rangeList = skill.Data.range;
        Queue<MapGrid> open = new Queue<MapGrid>();
        HashSet<(int x, int y)> close = new HashSet<(int x, int y)>();
        MapGrid oriGrid = chess.StayGrid;
        open.Enqueue(oriGrid);
        close.Add((oriGrid.X, oriGrid.Y));
        for (int i = 0; i < pathNum; i++)
        {
            int gridNum = open.Count;
            for (int j = 0; j < gridNum; j++)
            {
                //每次抵达格子，查看技能是否能够作用
                MapGrid curGrid = open.Dequeue();
                var coord = _map.GetCoordByGrid(curGrid);
                int x = coord.x;
                int y = coord.y;
                #region 技能搜索
                foreach (var pos in rangeList)
                {
                    //从四个方向分别搜寻可攻击的目标格子并记录
                    {//up
                        var grid = _map.GetGridByCoord(curGrid.X + pos.x, curGrid.Y - pos.y);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add((grid.StayedChess, curGrid));
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//down
                        var grid = _map.GetGridByCoord(curGrid.X - pos.x, curGrid.Y + pos.y);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add((grid.StayedChess, curGrid));
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//left
                        var grid = _map.GetGridByCoord(curGrid.X - pos.y, curGrid.Y - pos.x);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add((grid.StayedChess, curGrid));
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//right
                        var grid = _map.GetGridByCoord(curGrid.X + pos.y, curGrid.Y + pos.x);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add((grid.StayedChess, curGrid));
                            close.Add((grid.X, grid.Y));
                        }
                    }
                }
                #endregion
                //向外广搜
                if (i == pathNum - 1) break;
                if (x + 1 < _map.col) SearchTheWalkableGridByCoord(chess, open, close, x + 1, y);
                if (y + 1 < _map.row) SearchTheWalkableGridByCoord(chess, open, close, x, y + 1);
                if (x - 1 >= 0) SearchTheWalkableGridByCoord(chess, open, close, x - 1, y);
                if (y - 1 >= 0) SearchTheWalkableGridByCoord(chess, open, close, x, y - 1);
            }
        }
        Debug.Log("技能寻敌完毕，总共搜寻到目标个数:" + targetList.Count);
        return targetList;
    }
    private void SearchTheWalkableGridByCoord(IChess chess, Queue<MapGrid> open, HashSet<(int, int)> close, int x, int y)
    {
        if (close.Contains((x, y))) return;
        MapGrid temp = _map.GetGridByCoord(x, y);
        close.Add((x, y));
        if (_map.IsWalkable(chess, temp))
        {
            open.Enqueue(temp);
        }
    }
}
