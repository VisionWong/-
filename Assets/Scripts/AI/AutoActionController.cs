using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制指定的棋子自动行动
/// </summary>
public class AutoActionController
{
    private List<IChess> _chessList;
    private List<IChess> _enemyList;

    public AutoActionController(List<IChess> chessList, List<IChess> enemyList)
    {
        _chessList = chessList;
        _enemyList = enemyList;
    }

    public void StartAction()
    {
        //获取离敌对方距离最短的棋子先行动
        _chessList.Sort(new DistAscComparer(_enemyList));
        foreach (var chess in _chessList)
        {

        }
        //结束行动

    }
}

public class DistAscComparer : IComparer<IChess>
{
    private List<IChess> _enemyList;

    public DistAscComparer(List<IChess> enemyList)
    {
        _enemyList = enemyList;
    }

    //按距离从近到远排
    public int Compare(IChess x, IChess y)
    {
        int disX = int.MaxValue;
        int disY = int.MaxValue;
        foreach (var chess in _enemyList)
        {
            int curDist = Mathf.Abs(chess.StayGrid.X - x.StayGrid.X) + Mathf.Abs(chess.StayGrid.Y - x.StayGrid.Y);
            if (curDist < disX)
            {
                disX = curDist;
            }
        }
        foreach (var chess in _enemyList)
        {
            int curDist = Mathf.Abs(chess.StayGrid.X - y.StayGrid.X) + Mathf.Abs(chess.StayGrid.Y - y.StayGrid.Y);
            if (curDist < disY)
            {
                disY = curDist;
            }
        }
        if (disX < disY) return -1;
        else if (disX > disY) return 1;
        return 0;
    }
}

