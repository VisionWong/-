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

    public CommonAIController(List<PlayerChess> targetList, Map map) : base(targetList, map)
    {
        _buffSkillSet = new HashSet<Skill>();
        _buffToTimesDict = new Dictionary<Skill, int>();
    }

    public override void StartAction()
    {
        base.StartAction();
        int maxWeight = 0;
        //根据拥有的技能搜索可以作用到的目标，并计算权重，决定使用哪个技能
        foreach (var skill in _chess.SkillList)
        {
            (List<IChess> chessList, MapGrid grid, Direction dir) tempTarget = (null, null, Direction.None);
            switch (skill.Data.skillType)
            {
                case SkillType.Damage:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高伤害，权重为1
                    var targets = SearchTarget(_chess, skill);
                    if (targets == null) break;
                    int maxDamage = 0;
                    foreach (var target in targets)//每个target包含这个技能在某个格子朝某个方向能作用到的所有棋子
                    {
                        int damage = 0;
                        foreach (var chess in target.chessList)
                        {
                            if (chess.Tag == TagDefine.PLAYER)
                                damage += Formulas.CalSkillDamage(skill.Data, _chess, chess, false) * skill.Data.hitTimes;
                            else
                                damage -= Formulas.CalSkillDamage(skill.Data, _chess, chess, false) * skill.Data.hitTimes;
                        }
                        if (damage > maxDamage)
                        {
                            maxDamage = damage;
                            tempTarget = target;
                        }
                    }
                    if (maxDamage > maxWeight)
                    {
                        maxWeight = maxDamage;
                        _skillToUse = skill;
                        _target = tempTarget;
                    }
                    break;
                case SkillType.Heal:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高治疗，越残血权重越高
                    var healTargets = SearchTarget(_chess, skill);
                    int maxHeal = 0;
                    if (healTargets == null) break;
                    foreach (var target in healTargets)//决定这个技能能造成最高收益治疗的位置
                    {
                        int heal = 0;
                        foreach (var chess in target.chessList)
                        {
                            if (chess.Tag == TagDefine.ENEMY)
                                heal += (int)(Formulas.CalHealingNum(skill.Data, _chess, chess) * Formulas.GetHealingWeight(chess));
                            else
                                heal -= (int)(Formulas.CalHealingNum(skill.Data, _chess, chess) * Formulas.GetHealingWeight(chess));
                        }
                        if (heal > maxHeal)
                        {
                            maxHeal = heal;
                            tempTarget = target;
                        }
                    }
                    if (maxHeal > maxWeight)
                    {
                        maxWeight = maxHeal;
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
                            //Debuff技能与伤害技能一样，固定权重(暂定为每个单位自身血量四分之一
                            var effectTargets = SearchTarget(_chess, skill);
                            if (effectTargets == null) break;
                            int maxEffectNum = 0;
                            foreach (var target in effectTargets)
                            {
                                int effectNum = 0;
                                foreach (var chess in target.chessList)
                                {
                                    //TODO 棋子是否存在该debuff
                                    foreach (var effect in skill.Data.effects)
                                    {
                                        if (!chess.ContainsBuff(EnumTool.EffectTypeToBuffType(effect.effectType)))
                                        {
                                            if (chess.Tag == TagDefine.PLAYER)
                                                effectNum += 1;
                                            else
                                                effectNum -= 1;
                                            break;//只要有一个debuff状态能作用即可
                                        }
                                    }
                                }
                                if (effectNum > maxEffectNum)
                                {
                                    maxEffectNum = effectNum;
                                    tempTarget = target;
                                }
                            }
                            int effectWeight = maxEffectNum * _chess.Attribute.MaxHP / 4;
                            if (effectWeight > maxWeight)
                            {
                                maxWeight = effectWeight;
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
                Debug.Log(string.Format("当前计算的技能为{0},其权重为{1}", skill.Data.name, maxWeight));
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
            }//没有能使用的增益技能，选择靠近最近的目标
            var path = _map.PathFinding(_chess, _chess.StayGrid, GetTheTargetToClose().StayGrid, new AStarPathFinding(), _chess.Attribute.AP);
            _stayGrid = path[path.Count - 1];//获取终点格子
            _chess.Move(path, () => MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd));
            return;
        }
        //否则前往目标使用技能
        _stayGrid = _target.grid;
        _chess.Move(_map.PathFinding(_chess, _chess.StayGrid, _target.grid, new AStarPathFinding()), OnMoveEnd);
    }

    private void OnMoveEnd()
    {
        UseSkill(_skillToUse);
    }

    protected override IChess GetTheTargetToClose()
    {
        IChess ret = null;
        int minDist = int.MaxValue;
        foreach (var chess in _targetList)
        {
            int dist = Mathf.Abs(chess.StayGrid.X - _chess.StayGrid.X) + Mathf.Abs(chess.StayGrid.Y - _chess.StayGrid.Y);
            if (dist < minDist)
            {
                minDist = dist;
                ret = chess;
            }
        }
        return ret;
    }

    protected override List<(List<IChess> chessList, MapGrid grid, Direction dir)> SearchTarget(IChess chess, Skill skill)
    {
        List<(List<IChess>, MapGrid, Direction)> returnList = new List<(List<IChess>, MapGrid, Direction)>();
        int pathNum = chess.Attribute.AP;
        var rangeList = skill.Data.range;
        Queue<MapGrid> open = new Queue<MapGrid>();
        HashSet<(int x, int y)> close = new HashSet<(int x, int y)>();
        MapGrid oriGrid = chess.StayGrid;
        open.Enqueue(oriGrid);
        close.Add((oriGrid.X, oriGrid.Y));
        for (int i = 0; i <= pathNum; i++)
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
                //从四个方向分别搜寻可攻击的目标格子并记录
                CheckTargetByDir(1, -1, Direction.Up, rangeList, curGrid, returnList);
                CheckTargetByDir(-1, 1, Direction.Down, rangeList, curGrid, returnList);
                CheckTargetByDir(-1, -1, Direction.Left, rangeList, curGrid, returnList, false);
                CheckTargetByDir(1, 1, Direction.Right, rangeList, curGrid, returnList, false);
                #endregion
                //向外广搜
                if (i == pathNum) continue;
                if (x + 1 < _map.col) SearchTheWalkableGridByCoord(chess, open, close, x + 1, y);
                if (y + 1 < _map.row) SearchTheWalkableGridByCoord(chess, open, close, x, y + 1);
                if (x - 1 >= 0) SearchTheWalkableGridByCoord(chess, open, close, x - 1, y);
                if (y - 1 >= 0) SearchTheWalkableGridByCoord(chess, open, close, x, y - 1);
            }
        }
        Debug.Log("技能寻敌完毕，总共搜寻到技能用法个数:" + returnList.Count);
        return returnList;
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
    private void CheckTargetByDir(int x, int y, Direction dir, List<Coord> rangeList, MapGrid curGrid, List<(List<IChess>, MapGrid, Direction)> returnList, bool isUpOrDown = true)
    {
        List<IChess> targetList = new List<IChess>();
        foreach (var pos in rangeList)
        {
            MapGrid grid = null;
            if (isUpOrDown)
                grid = _map.GetGridByCoord(curGrid.X + pos.x * x, curGrid.Y + pos.y * y);
            else
                grid = _map.GetGridByCoord(curGrid.X + pos.y * x, curGrid.Y + pos.x * y);
            if (grid != null && grid.StayedChess != null && grid.StayedChess != _chess)
            {
                targetList.Add(grid.StayedChess);
            }
        }
        if (targetList.Count > 0)
        {
            returnList.Add((targetList, curGrid, dir));
        }
    }
}
