using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AStarPathFinding : IPathFindingStrategy
{
    class AStarGrid
    {
        public MapGrid grid;
        public AStarGrid lastGrid;
        public int f, g, h;

        public AStarGrid(MapGrid grid, AStarGrid lastGrid, int f = 0, int g = 0, int h = 0)
        {
            this.grid = grid;
            this.lastGrid = lastGrid;
            this.f = f;
            this.g = g;
            this.h = h;
        }

        public void SetGAndH(int g, int h)
        {
            this.g = g;
            this.h = h;
            f = g + h;
        }
    }

    public override List<MapGrid> PathFinding(IChess chess, MapGrid origin, MapGrid dest, Map map)
    {
        int sh = HeuristicFunc(origin, origin, dest).h;
        AStarGrid start = new AStarGrid(origin, null, sh, 0, sh);
        List<AStarGrid> open = new List<AStarGrid>();
        HashSet<MapGrid> close = new HashSet<MapGrid>();
        open.Add(start);
        while (open.Count > 0)
        {
            AStarGrid cur = GetMinFGrid(open);
            close.Add(cur.grid);
            open.Remove(cur);
            //访问相邻的可通行的格子
            if (CheckWalkable(chess, origin, dest, map, map.GetUpGrid(cur.grid), cur, open, close) ||
                CheckWalkable(chess, origin, dest, map, map.GetDownGrid(cur.grid), cur, open, close)||
                CheckWalkable(chess, origin, dest, map, map.GetLeftGrid(cur.grid), cur, open, close)||
                CheckWalkable(chess, origin, dest, map, map.GetRightGrid(cur.grid), cur, open, close))
            {
                //已经到达终点，终点上一个格子即是cur，返回整个路径列表
                List<MapGrid> path = new List<MapGrid>();
                path.Add(dest);
                AStarGrid curGrid = cur;
                while (curGrid.grid != null)
                {
                    path.Add(curGrid.grid);
                    curGrid = curGrid.lastGrid;
                }
                path.Reverse();
                for (int i = 0; i < path.Count; i++)
                {
                    Debug.Log(string.Format("路径第{0}个格子坐标为（{1}，{2}）", i, path[i].X, path[i].Y));
                }
                return path;
            }
        }
        //未能抵达终点，返回空路径
        return null;
    }

    public (int g, int h) HeuristicFunc(MapGrid origin, MapGrid cur, MapGrid dest)
    {
        int g = Math.Abs(cur.X - origin.X) + Math.Abs(cur.Y - origin.Y);
        int h = Math.Abs(dest.X - origin.X) + Math.Abs(dest.Y - origin.Y);
        return (g, h);
    }

    //能移动的格子不多，不需要特地使用堆
    private AStarGrid GetMinFGrid(List<AStarGrid> open)
    {
        AStarGrid min = null;
        int minF = int.MaxValue;
        foreach (var grid in open)
        {
            if (grid.f < minF)
            {
                min = grid;
                minF = grid.f;
            }
        }
        return min;
    }

    private bool CheckWalkable(IChess chess, MapGrid origin, MapGrid dest, Map map, MapGrid grid, AStarGrid dadGrid, List<AStarGrid> open, HashSet<MapGrid> close)
    {
        if (grid != null && !close.Contains(grid) && map.IsWalkable(chess, grid))
        {
            //判断是否到达终点
            if (grid == dest) return true;
            //TODO 若在open中，比对f值是否更小，是则更改, 目前游戏不存在该情况，后续有需求再修改
            if (IsOpenContains(open, grid)) return false;
            //不在则加入open中
            AStarGrid temp = new AStarGrid(grid, dadGrid);
            var gh = HeuristicFunc(origin, temp.grid, dest);
            temp.SetGAndH(gh.g, gh.h);
            open.Add(temp);
            Debug.Log(string.Format("坐标为（{0}，{1}）的格子被加入到open列表中", temp.grid.X, temp.grid.Y));
        }
        return false;
    }

    private bool IsOpenContains(List<AStarGrid> open, MapGrid grid)
    {
        foreach (var item in open)
        {
            if (item.grid == grid)
            {
                return true;
            }
        }
        return false;
    }
}
