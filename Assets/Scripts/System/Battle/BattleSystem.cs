using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public enum BattleState
{
    Pause,
    WaitSelecting,
    WaitMoving,
    WaitMoveConfirm,
    WaitAttacking,
    WaitAttackConfirm,
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

    private GameObject m_virtualChess = null;

    public void StartBattle()
    {
        LoadMap();
        LoadPlayerChess();
        LoadEnemyChess();
        LoadVirtualChess();
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
        PathPack pathPack = new PathPack(1, "Chess/001", "Sprite/Chess/001");

        GameObject go = Instantiate(Resources.Load<GameObject>("Chess/001"));
        PlayerChess chess = new PlayerChess(new PlayerAttr(), go);
        chess.SetPathPack(pathPack);
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
        m_curPlayerChess.ChangeToIdle();
        m_curPlayerChess = null;
        m_map.CancelLastHighlightGrids();
        BattleState = BattleState.WaitSelecting;
    }

    public void OnSelectWalkableChess(PlayerChess chess)
    {
        //如果之前选中了棋子正在寻路，则取消上个棋子的寻路操作
        if (m_curPlayerChess != null)
        {
            CancelMoving();
        }
        HighlightWalkableGrids(chess);
        //将虚拟棋子设为当前的棋子样式
        m_virtualChess.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(m_curPlayerChess.PathPack.SpritePath);
    }

    /// <summary>
    /// 在指定格子生成棋子虚像
    /// </summary>
    public void OnSelectWalkableGrid(MapGrid grid)
    {
        if (m_curPlayerChess != null)
        {
            m_virtualChess.transform.position = grid.transform.position;
            m_virtualChess.SetActive(true);
        }
    }

    /// <summary>
    /// 将虚像解除，将棋子送往该格子位置，期间无法选中其他格子
    /// </summary>
    /// <param name="grid"></param>
    public void OnConfirmWalkableGrid(MapGrid grid)
    {
        if (m_curPlayerChess != null)
        {
            m_virtualChess.SetActive(false);

        }
    }

    private void LoadVirtualChess()
    {
        m_virtualChess = Instantiate(Resources.Load<GameObject>("Chess/001"));
        SpriteRenderer m_virtualRenderer = m_virtualChess.GetComponent<SpriteRenderer>();
        m_virtualRenderer.color = new Color(m_virtualRenderer.color.r, m_virtualRenderer.color.g, m_virtualRenderer.color.b, 0.5f);
        m_virtualChess.SetActive(false);
    }
}
