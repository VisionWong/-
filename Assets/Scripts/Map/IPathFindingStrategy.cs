using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPathFindingStrategy
{
    public abstract List<MapGrid> PathFinding(IChess chess, MapGrid origin, MapGrid dest, Map map);
}
