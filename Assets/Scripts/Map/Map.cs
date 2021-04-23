using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class Map : MonoBehaviour
{
    public MapGrid[] grids;
    public int row;
    public int col;

    private List<MapGrid> lastGrids = new List<MapGrid>();

    public void Start()
    {
        foreach (var grid in grids)
        {
            var coord = GetCoordByGrid(grid);
            grid.SetCoord(coord.x, coord.y);
        }
        //设置战斗摄像机
        float left = grids[0].transform.position.x;
        float right = grids[col - 1].transform.position.x;
        float up = grids[0].transform.position.y;
        float down = GetGridByCoord(0, row - 1).transform.position.y;
        Camera.main.gameObject.AddComponent<CameraController>().SetBorder(left, right, up, down);
    }

    public MapGrid GetGridByCoord(int x, int y)
    {
        int index = y * col + x;
        return index >= grids.Length || index < 0 ? null : grids[index];
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
        lastGrids.Clear();
        //根据该棋子的属性 搜索可以走的格子
        MapGrid originGrid = chess.StayGrid;
        int pathNum = chess.Attribute.AP;
        //广搜 高亮所有可走的格子
        Queue<MapGrid> open = new Queue<MapGrid>();
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
        //Debug.Log("寻路完毕，可通行格子数量为：" + lastGrids.Count);
    }

    /// <summary>
    /// 隐藏上一次高亮的寻路网络，可选择是否清空缓存
    /// </summary>
    public void CancelLastHighlightGrids(bool isDelete = true)
    {
        foreach (var grid in lastGrids)
        {
            grid.CancelHighlight();
        }
        if(isDelete) lastGrids.Clear();
    }

    /// <summary>
    /// 重新展示上一次高亮的寻路网络
    /// </summary>
    public void ShowLastHighlightGrids()
    {
        foreach (var grid in lastGrids)
        {
            grid.HighlightWalkable();
        }
    }

    public void HighlightGrids(List<MapGrid> grids)
    {
        foreach (var grid in grids)
        {
            grid.HighlightAttackable();
        }
    }

    public void CancelHighlightGrids(List<MapGrid> grids)
    {
        foreach (var grid in grids)
        {
            grid.CancelHighlight();
        }
    }

    /// <summary>
    /// 使用指定寻路策略寻路，返回起点开始到终点的路径格子列表
    /// </summary>
    /// <param name="origin">起点</param>
    /// <param name="dest">终点</param>
    /// <returns></returns>
    public List<MapGrid> PathFinding(IChess chess, MapGrid origin, MapGrid dest, IPathFindingStrategy strategy)
    {
        return strategy.PathFinding(chess, origin, dest, this);
    }

    #region 获取四周的格子
    public MapGrid GetUpGrid(MapGrid grid)
    {
        if (grid.Y - 1 >= 0)
        {
            return GetGridByCoord(grid.X, grid.Y - 1);
        }
        return null;
    }
    public MapGrid GetDownGrid(MapGrid grid)
    {
        if (grid.Y + 1 < row)
        {
            return GetGridByCoord(grid.X, grid.Y + 1);
        }
        return null;
    }
    public MapGrid GetLeftGrid(MapGrid grid)
    {
        if (grid.X - 1 >= 0)
        {
            return GetGridByCoord(grid.X - 1, grid.Y);
        }
        return null;
    }
    public MapGrid GetRightGrid(MapGrid grid)
    {
        if (grid.X + 1 < col)
        {
            return GetGridByCoord(grid.X + 1, grid.Y);
        }
        return null;
    }
    #endregion

    /// <summary>
    /// 根据棋子的属性和访问格子的类型判断是否可以通行
    /// </summary>
    /// <param name="chess"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public bool IsWalkable(IChess chess, MapGrid grid)
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

    #region 技能寻敌
    public List<MapGrid> upAttackableGrids = new List<MapGrid>();
    public List<MapGrid> downAttackableGrids = new List<MapGrid>();
    public List<MapGrid> leftAttackableGrids = new List<MapGrid>();
    public List<MapGrid> rightAttackableGrids = new List<MapGrid>();
    public List<IChess> upAttackableTargets = new List<IChess>();
    public List<IChess> downAttackableTargets = new List<IChess>();
    public List<IChess> leftAttackableTargets = new List<IChess>();
    public List<IChess> rightAttackableTargets = new List<IChess>();


    private void ClearAttackableGrids()
    {
        upAttackableGrids.Clear();
        downAttackableGrids.Clear();
        leftAttackableGrids.Clear();
        rightAttackableGrids.Clear();
        upAttackableTargets.Clear();
        downAttackableTargets.Clear();
        leftAttackableTargets.Clear();
        rightAttackableTargets.Clear();
    }

    /// <summary>
    /// 高亮该技能能打到的目标的格子，并通知UI出现选项界面
    /// </summary>
    /// <param name="chess"></param>
    /// <param name="skill"></param>
    public void SearchAttackableTarget(IChess chess, Skill skill)
    {//TODO 目前仅仅实现了非指向性和自身的技能寻标，还有指向性的针对-1坐标做的特殊处理
        ClearAttackableGrids();
        MapGrid origin = chess.StayGrid;
        var rangeList = skill.Data.range;
        foreach (var pos in rangeList)
        {
            //从四个方向分别搜寻可攻击的目标格子并记录
            {//up
                var grid = GetGridByCoord(origin.X + pos.x, origin.Y - pos.y);
                if (grid != null)
                {
                    upAttackableGrids.Add(grid);
                    if (grid.StayedChess != null) upAttackableTargets.Add(grid.StayedChess);
                }
            }
            {//down
                var grid = GetGridByCoord(origin.X - pos.x, origin.Y + pos.y);
                if (grid != null)
                {
                    downAttackableGrids.Add(grid);
                    if (grid.StayedChess != null) downAttackableTargets.Add(grid.StayedChess);
                }
            }
            {//left
                var grid = GetGridByCoord(origin.X - pos.y, origin.Y - pos.x);
                if (grid != null)
                {
                    leftAttackableGrids.Add(grid);
                    if (grid.StayedChess != null) leftAttackableTargets.Add(grid.StayedChess);
                }
            }
            {//right
                var grid = GetGridByCoord(origin.X + pos.y, origin.Y + pos.x);
                if (grid != null)
                {
                    rightAttackableGrids.Add(grid);
                    if (grid.StayedChess != null) rightAttackableTargets.Add(grid.StayedChess);
                }
            }
        }//搜寻完毕
        //通知UI寻敌完毕
        MessageCenter.Instance.Broadcast(MessageType.OnSearchAttackableEnd);
    }

    /// <summary>
    /// 高亮该技能能打到的目标的格子，并通知UI出现选项界面
    /// </summary>
    /// <param name="chess"></param>
    /// <param name="skill"></param>
    public void SearchAttackableTargetWithTag(IChess chess, Skill skill, string tag)
    {//TODO 目前仅仅实现了非指向性和自身的技能寻标，还有指向性的针对-1坐标做的特殊处理
        ClearAttackableGrids();
        MapGrid origin = chess.StayGrid;
        var rangeList = skill.Data.range;
        foreach (var pos in rangeList)
        {
            //从四个方向分别搜寻可攻击的目标格子并记录
            {//up
                var grid = GetGridByCoord(origin.X + pos.x, origin.Y - pos.y);
                if (grid != null && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                {
                    upAttackableGrids.Add(grid);
                }
            }
            {//down
                var grid = GetGridByCoord(origin.X - pos.x, origin.Y + pos.y);
                if (grid != null && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                {
                    downAttackableGrids.Add(grid);
                }
            }
            {//left
                var grid = GetGridByCoord(origin.X - pos.y, origin.Y - pos.x);
                if (grid != null && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                {
                    leftAttackableGrids.Add(grid);
                }
            }
            {//right
                var grid = GetGridByCoord(origin.X + pos.y, origin.Y + pos.x);
                if (grid != null && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                {
                    rightAttackableGrids.Add(grid);
                }
            }
        }//搜寻完毕
        //通知UI寻敌完毕
        MessageCenter.Instance.Broadcast(MessageType.OnSearchAttackableEnd);
    }

    public List<IChess> GetAttackableTargetsByBFS(IChess chess, Skill skill, string tag)
    {
        List<IChess> targetList = new List<IChess>();
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
                var coord = GetCoordByGrid(curGrid);
                int x = coord.x;
                int y = coord.y;
                #region 技能搜索
                foreach (var pos in rangeList)
                {
                    //从四个方向分别搜寻可攻击的目标格子并记录
                    {//up
                        var grid = GetGridByCoord(curGrid.X + pos.x, curGrid.Y - pos.y);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add(grid.StayedChess);
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//down
                        var grid = GetGridByCoord(curGrid.X - pos.x, curGrid.Y + pos.y);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add(grid.StayedChess);
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//left
                        var grid = GetGridByCoord(curGrid.X - pos.y, curGrid.Y - pos.x);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add(grid.StayedChess);
                            close.Add((grid.X, grid.Y));
                        }
                    }
                    {//right
                        var grid = GetGridByCoord(curGrid.X + pos.y, curGrid.Y + pos.x);
                        if (grid != null && !close.Contains((grid.X, grid.Y)) && grid.StayedChess != null && grid.StayedChess.Tag == tag)
                        {
                            targetList.Add(grid.StayedChess);
                            close.Add((grid.X, grid.Y));
                        }
                    }
                }
                #endregion
                //向外广搜
                if (i == pathNum - 1) break;
                if (x + 1 < col) SearchTheWalkableGridByCoord(chess, open, close, x + 1, y);
                if (y + 1 < row) SearchTheWalkableGridByCoord(chess, open, close, x, y + 1);
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
        MapGrid temp = GetGridByCoord(x, y);
        close.Add((x, y));
        if (IsWalkable(chess, temp))
        {
            open.Enqueue(temp);
        }
    }
    #endregion
}
