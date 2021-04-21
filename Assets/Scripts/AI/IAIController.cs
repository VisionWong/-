using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAIController
{
    protected IChess _chess;
    protected IChess _target;
    protected List<IChess> _targetList;
    protected List<IChess> _reachableList;
    protected Map _map;
    
    public IAIController(IChess chess, List<IChess> targetList, Map map)
    {
        _chess = chess;
        _targetList = targetList;
        _reachableList = new List<IChess>();
        _map = map;
    }

    public abstract void SearchTarget();


}
