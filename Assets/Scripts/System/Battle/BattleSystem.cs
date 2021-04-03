using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public enum BattleState
{
    Pause,
    WaitSelect,
    WaitMove,
    WaitMoveConfirm,
    WaitAttack,
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
    private MapGrid m_curWalkableGrid = null;
    private MapGrid m_lastOriginGrid = null;

    public void StartBattle()
    {
        LoadMap();
        LoadPlayerChess();
        LoadEnemyChess();
        LoadVirtualChess();
        BattleState = BattleState.WaitSelect;

        RegisterAll();
    }

    private void LoadMap()
    {
        Map map = ResourceMgr.Instance.Load<Map>("Map/Map001");
        m_map = Instantiate(map, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        //TODO 根据关卡信息生成地图或随机生成
    }

    private void LoadPlayerChess()
    {
        LoadPlayerChess(5, 4);
        LoadPlayerChess(4, 6);
        //TODO 根据玩家背包里的信息生成棋子，位置则根据关卡默认位置，玩家后续可在区域内调整
    }

    private void LoadPlayerChess(int x, int y)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Chess/001"));
        PlayerChess chess = new PlayerChess(new PlayerAttr(), go);

        PathPack pathPack = new PathPack(1, "Chess/001", "Sprite/Chess/001");
        chess.SetPathPack(pathPack);
        chess.SetAnimator(go.AddComponent<ChessAnimator>());
        chess.SetSelectableScript(go.AddComponent<SelectablePlayerChess>()); 

        MapGrid grid = m_map.GetGridByCoord(x, y);
        chess.SetStayGrid(grid);
        chess.Attribute.AP = 4;
        m_playerList.Add(chess);
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

    //高亮可通行网格
    private void HighlightWalkableGrids(PlayerChess chess)
    {
        m_curPlayerChess = chess;
        m_map.HighlightWalkableGrids(chess);
    }

    //取消棋子选中移动行为
    private void CancelMoving()
    {
        if (m_virtualChess.activeSelf) m_virtualChess.SetActive(false);
        m_curWalkableGrid = null;

        m_curPlayerChess.ChangeToIdle();
        m_curPlayerChess = null;

        m_map.CancelLastHighlightGrids();
    }

    //选中未进行行动的棋子
    private void OnSelectWalkableChess(PlayerChess chess)
    {
        if (BattleState == BattleState.WaitSelect || BattleState == BattleState.WaitMove)
        {
            //如果之前选中了棋子正在寻路，则取消上个棋子的寻路操作
            if (m_curPlayerChess != null)
            {
                CancelMoving();
            }
            HighlightWalkableGrids(chess);
            //将虚拟棋子设为当前的棋子样式
            m_virtualChess.GetComponent<SpriteRenderer>().sprite = ResourceMgr.Instance.Load<Sprite>(m_curPlayerChess.PathPack.SpritePath);
            BattleState = BattleState.WaitMove;
            m_curPlayerChess.ChangeToWaitMove();
        }
    }

    //选择让棋子停留原地
    private void OnSelectChessStay()
    {
        ConfirmWalkableGrid(m_curPlayerChess.StayGrid);
    }

    private void OnSelectIdleGrid(MapGrid grid)
    {
        if (BattleState == BattleState.WaitMove)
        {
            CancelMoving();
            BattleState = BattleState.WaitSelect;
        }
    }

    //选中可通行的格子
    private void OnSelectWalkableGrid(MapGrid grid)
    {
        if (BattleState == BattleState.WaitMove)
        {
            if (grid == m_curPlayerChess.StayGrid || m_curWalkableGrid == grid) //两次选择了同一个高亮格子或棋子选择停留原地
            {
                ConfirmWalkableGrid(grid);
            }
            else if (m_curWalkableGrid == null || m_curWalkableGrid != grid)
            {
                LoadVirtualChessOnGrid(grid);
            }
            m_curWalkableGrid = grid;
        }
    }

    /// <summary>
    /// 棋子行动回合结束,处理格子停留事件，回合转至下一个棋子
    /// </summary>
    public void ConfirmStayGrid()
    {
        m_curPlayerChess.SetStayGrid(m_curWalkableGrid);
        m_curPlayerChess.ChangeToActionEnd();
        //TODO 观察我方是否还有可以行动的棋子
        m_curWalkableGrid = null;
        m_curPlayerChess = null;
        BattleState = BattleState.WaitSelect;
    }//未完成

    /// <summary>
    /// 将虚像解除，将棋子送往该格子位置，期间无法选中其他格子
    /// </summary>
    /// <param name="grid"></param>
    private void ConfirmWalkableGrid(MapGrid grid)
    {
        if (m_curPlayerChess != null)
        {
            m_virtualChess.SetActive(false);
            m_map.CancelLastHighlightGrids(false);
            //记录初始位置
            m_lastOriginGrid = m_curPlayerChess.StayGrid;
            //获取路径，然后让棋子按路径走过去，走的过程屏蔽操作
            List<MapGrid> path = m_map.PathFinding(m_curPlayerChess, m_lastOriginGrid, grid, new AStarPathFinding());
            MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
            m_curPlayerChess.Move(path, OnChessMoveComplet);
        }
    }

    /// <summary>
    /// 在指定格子生成棋子虚像
    /// </summary>
    private void LoadVirtualChessOnGrid(MapGrid grid)
    {
        if (m_curPlayerChess != null)
        {
            m_virtualChess.transform.position = grid.transform.position;
            m_virtualChess.SetActive(true);
        }
    }
    private void LoadVirtualChess()
    {
        m_virtualChess = Instantiate(Resources.Load<GameObject>("Chess/001"));
        SpriteRenderer m_virtualRenderer = m_virtualChess.GetComponent<SpriteRenderer>();
        m_virtualRenderer.color = new Color(m_virtualRenderer.color.r, m_virtualRenderer.color.g, m_virtualRenderer.color.b, 0.5f);
        m_virtualChess.SetActive(false);
    }
    
    private void OnChessMoveComplet()
    {
        MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        MessageCenter.Instance.Broadcast(MessageType.OnChessAction);
        BattleState = BattleState.WaitAttack;
        m_curPlayerChess.ChangeToWaitAttack();
    }

    //取消当前棋子的移动操作
    public void OnCancelMove()
    {
        //将棋子送往原来的位置，重新高亮寻路网络
        m_curWalkableGrid = null;
        m_map.ShowLastHighlightGrids();
        m_curPlayerChess.SetStayGrid(m_lastOriginGrid);
        m_curPlayerChess.ChangeToWaitMove();
        BattleState = BattleState.WaitMove;
        MessageCenter.Instance.Broadcast(MessageType.OnCancelMove);
    }

    #region 事件注册
    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.AddListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
    }
    private void RemoveAll()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.RemoveListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
    }
    #endregion
}
