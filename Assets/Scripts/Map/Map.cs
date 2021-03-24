using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapGrid[] grids;
    public int row;
    public int col;

    private List<MapGrid> lastGrids = new List<MapGrid>();

    public MapGrid GetGridByCoord(int x, int y)
    {
        int index = y * col + x;
        return grids[index];
    }

    public (int x, int y) GetCoordByGrid(MapGrid grid)
    {
        int index = GetIndexOfGrid(grid);
        int x = index % col;
        int y = index / col;
        return (x, y);
    }

    /// <summary>
    /// 根据棋子所在的棋格和棋子的行动力，高亮所有可移动的格子
    /// </summary>
    /// <param name="chess"></param>
    public void HighlightWalkableGrids(IChess chess)
    {
        //根据该棋子的属性 搜索可以走的格子
        MapGrid originGrid = chess.StayGrid;
        int pathNum = chess.Attribute.AP;
        //广搜 高亮所有可走的格子
        Queue<MapGrid> open = new Queue<MapGrid>();
        //HashSet<MapGrid> close = new HashSet<MapGrid>();
        HashSet<(int, int)> close = new HashSet<(int, int)>();
        open.Enqueue(originGrid);
        var oriCoord = GetCoordByGrid(originGrid);
        close.Add((oriCoord.x, oriCoord.y));
        for (int i = 0; i < pathNum; i++)
        {
            int gridNum = open.Count;
            for (int j = 0; j < gridNum; j++)
            {
                MapGrid grid = open.Dequeue();
                var coord = GetCoordByGrid(grid);
                int x = coord.x;
                int y = coord.y;
                if (x + 1 < col) HighlightTheWalkableGridByCoord(chess, open, close, x + 1, y);
                if (y + 1 < row) HighlightTheWalkableGridByCoord(chess, open, close, x, y + 1);
                if (x - 1 >= 0) HighlightTheWalkableGridByCoord(chess, open, close, x - 1, y);
                if (y - 1 >= 0) HighlightTheWalkableGridByCoord(chess, open, close, x, y - 1);
            }
        }
        Debug.Log("寻路完毕，可通行格子数量为：" + lastGrids.Count);
    }

    /// <summary>
    /// 隐藏上一次高亮的寻路网络
    /// </summary>
    public void CancelLastHighlightGrids()
    {
        foreach (var grid in lastGrids)
        {
            grid.CancelHighlight();
        }
        lastGrids.Clear();
    }

    /// <summary>
    /// 根据棋子的属性和访问格子的类型判断是否可以通行
    /// </summary>
    /// <param name="chess"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    private bool IsWalkable(IChess chess, MapGrid grid)
    {
        if (!grid.CanMove || grid.TerrainType == TerrainType.Obstacle) return false;
        return true;
    }

    //TODO 可以用公式优化一下O（1）
    private int GetIndexOfGrid(MapGrid grid)
    {
        int index = 0;
        foreach (var item in grids)
        {
            if (grid == item)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    private void HighlightTheWalkableGridByCoord(IChess chess, Queue<MapGrid> open, HashSet<(int, int)> close, int x, int y)
    {
        if (close.Contains((x, y))) return;
        MapGrid temp = GetGridByCoord(x, y);
        close.Add((x, y));
        if (IsWalkable(chess, temp))
        {
            temp.HighlightWalkable();
            lastGrids.Add(temp);
            open.Enqueue(temp);
        }
    }
}
