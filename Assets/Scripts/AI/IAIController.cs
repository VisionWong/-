using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAIController
{
    protected IChess _chess;
    protected List<IChess> _targetList;
    protected Map _map;

    protected (IChess chess, MapGrid grid) _target;
    protected Skill _skillToUse = null;
    protected MapGrid _stayGrid = null;//用于储存追逐目标但无法到达的寻路路径终点格子

    public IAIController(IChess chess, List<IChess> targetList, Map map)
    {
        _chess = chess;
        _targetList = targetList;
        _map = map;
    }

    //行动逻辑 寻敌、技能选择、移动、(使用技能)
    public virtual void StartAction()
    {

    }

    //寻敌逻辑
    protected abstract List<(IChess chess, MapGrid grid)> SearchTarget(IChess chess, Skill skill, string tag);


}
