using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public enum BattleState
{
    Pause,
    WaitSelecting,
    WaitMoving,
    WaitAttacking,
    EnemyTurn,
}

public class BattleSystem : MonoSingleton<BattleSystem>
{
    public BattleState BattleState { get; private set; }

    private Map m_map;
    private List<IChess> m_playerList = new List<IChess>();
    private List<IChess> m_enemyList = new List<IChess>();

    private ISelectable m_curSelected = null;
    private PlayerChess m_curPlayerChess = null;

    public void StartBattle()
    {
        LoadMap();
        LoadPlayerChess();
        LoadEnemyChess();
        BattleState = BattleState.WaitSelecting;
    }

    public void LoadMap()
    {
        Map map = Resources.Load<Map>("Map/Map001");
        m_map = Instantiate(map, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        //TODO 根据关卡信息生成地图或随机生成
    }

    public void LoadPlayerChess()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Chess/001"));
        PlayerChess chess = new PlayerChess(new PlayerAttr(), go);
        go.AddComponent<SelectablePlayerChess>().SetChess(chess);
        MapGrid grid = m_map.GetGridByCoord(5, 4);
        chess.SetStayGrid(grid);
        chess.Attribute.AP = 4;
        go.transform.position = grid.transform.position;
        m_playerList.Add(chess);
        //TODO 根据玩家背包里的信息生成棋子，位置则根据关卡默认位置，玩家后续可在区域内调整
    }

    public void LoadEnemyChess()
    {
        //TODO 据关卡信息生成
    }

    public void SetSelected(ISelectable item)
    {
        m_curSelected?.CancelSelect();
        m_curSelected = item;
    }

    public void HighlightWalkableGrids(PlayerChess chess)
    {
        m_curPlayerChess = chess;
        m_map.HighlightWalkableGrids(chess);
        BattleState = BattleState.WaitMoving;
    }

    public void CancelMoving()
    {
        m_curPlayerChess = null;
        m_map.CancelLastHighlightGrids();
        BattleState = BattleState.WaitSelecting;
    }

    public void OnSelectWalkableGrid()
    {
        if (m_curPlayerChess != null)
        {

        }
    }
}
