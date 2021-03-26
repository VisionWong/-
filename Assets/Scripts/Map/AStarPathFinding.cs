using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AStarPathFinding : IPathFindingStrategy
{
    struct AStarGrid
    {
        public MapGrid grid;
        public MapGrid lastGrid;
        public int f, g, h;

        public AStarGrid(MapGrid grid, MapGrid lastGrid, int f = 0, int g = 0, int h = 0)
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
            CheckWalkable(chess, origin, dest, map, map.GetUpGrid(cur.grid), cur.grid, open, close);
            CheckWalkable(chess, origin, dest, map, map.GetDownGrid(cur.grid), cur.grid, open, close);
            CheckWalkable(chess, origin, dest, map, map.GetLeftGrid(cur.grid), cur.grid, open, close);
            CheckWalkable(chess, origin, dest, map, map.GetRightGrid(cur.grid), cur.grid, open, close);
        }
        throw new NotImplementedException();
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
        AStarGrid min = new AStarGrid();
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

    private void CheckWalkable(IChess chess, MapGrid origin, MapGrid dest, Map map, MapGrid grid, MapGrid dadGrid, List<AStarGrid> open, HashSet<MapGrid> close)
    {
        if (grid != null && !close.Contains(grid) && map.IsWalkable(chess, grid))
        {
            //判断是否到达终点
            //若在open中，比对f值是否更小，是则更改
            //不在则加入open中
            AStarGrid temp = new AStarGrid(grid, dadGrid);
            var gh = HeuristicFunc(origin, temp.grid, dest);
            temp.SetGAndH(gh.g, gh.h);
        }
    }
}
