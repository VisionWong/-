using System;
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

    private Map _map;
    private List<IChess> _playerList = new List<IChess>();
    private List<IChess> _enemyList = new List<IChess>();
    private AutoActionController _enemyController;
    private int _actionedNum;//已经行动完毕的玩家棋子数

    private ISelectable _curSelected = null;
    private PlayerChess _curPlayerChess = null;

    private GameObject _virtualChess = null;
    private MapGrid _curWalkableGrid = null;
    private MapGrid _lastOriginGrid = null;

    private Skill _curUsedSkill = null;

    #region 战斗准备
    public void StartBattle()
    {
        LoadMap();
        LoadPlayerChess();
        LoadEnemyChess();
        LoadVirtualChess();
        //TODO 我方布阵
        //TODO 敌方特性触发
        //TODO 我方特性触发
        //我方回合开始
        OnPlayerTurn();
        

        RegisterAll();
    }

    private void LoadMap()
    {
        Map map = ResourceMgr.Instance.Load<Map>("Map/Map001");
        _map = Instantiate(map, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        //TODO 根据关卡信息生成地图或随机生成
    }

    private void LoadPlayerChess()
    {
        LoadPlayerChess(5, 4, 252);
        LoadPlayerChess(4, 3, 252);
        LoadPlayerChess(5, 3, 252);
        LoadPlayerChess(6, 3, 252);
        LoadPlayerChess(4, 6, 255);
        //TODO 根据玩家背包里的信息生成棋子，位置则根据关卡默认位置，玩家后续可在区域内调整
    }   

    private void LoadPlayerChess(int x, int y, int id)
    {
        var chess = ChessFactory.ProducePlayer(id);
        chess.LearnSkill(SkillFactory.Produce(5));
        chess.LearnSkill(SkillFactory.Produce(6));
        chess.LearnSkill(SkillFactory.Produce(4));
        //chess.LearnSkill(SkillFactory.Produce(5));
        MapGrid grid = _map.GetGridByCoord(x, y);
        chess.SetStayGrid(grid);
        _playerList.Add(chess);
    }

    public void LoadEnemyChess()
    {
        //TODO 据关卡信息生成

        _enemyController = new AutoActionController(_playerList, _enemyList);
    }
    #endregion

    #region 棋子操作
    public void SetSelected(ISelectable item)
    {
        _curSelected?.CancelSelect();
        _curSelected = item;
    }

    public PlayerChess GetCurPlayerChess()
    {
        return _curPlayerChess;
    }

    //取消棋子选中移动行为
    private void CancelMoving()
    {
        if (_virtualChess.activeSelf) _virtualChess.SetActive(false);
        _curWalkableGrid = null;

        _curPlayerChess.ChangeToIdle();
        _curPlayerChess = null;

        _map.CancelLastHighlightGrids();
    }

    //选中未进行行动的棋子
    private void OnSelectWalkableChess(PlayerChess chess)
    {
        if (BattleState == BattleState.WaitSelect || BattleState == BattleState.WaitMove)
        {
            //如果之前选中了棋子正在寻路，则取消上个棋子的寻路操作
            if (_curPlayerChess != null)
            {
                CancelMoving();
            }
            _curPlayerChess = chess;
            _map.HighlightWalkableGrids(chess);
            //将虚拟棋子设为当前的棋子样式
            _virtualChess.GetComponent<SpriteRenderer>().sprite = ResourceMgr.Instance.Load<Sprite>(_curPlayerChess.PathPack.SpritePath);
            BattleState = BattleState.WaitMove;
            _curPlayerChess.ChangeToWaitMove();
        }
    }

    private void OnSelectUnwalkableChess()
    {
        if (BattleState == BattleState.WaitSelect || BattleState == BattleState.WaitMove)
        {
            //如果之前选中了棋子正在寻路，则取消上个棋子的寻路操作
            if (_curPlayerChess != null)
            {
                CancelMoving();
            }
            _curPlayerChess = null;
            BattleState = BattleState.WaitSelect;
        }
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
            if (grid == _curPlayerChess.StayGrid || _curWalkableGrid == grid) //两次选择了同一个高亮格子或棋子选择停留原地
            {
                ConfirmWalkableGrid(grid);
            }
            else if (_curWalkableGrid == null || _curWalkableGrid != grid)
            {
                LoadVirtualChessOnGrid(grid);
            }
            _curWalkableGrid = grid;
        }
    }

    /// <summary>
    /// 棋子行动回合结束,处理格子停留事件，回合转至下一个棋子
    /// </summary>
    public void OnChessActionEnd()
    {
        if (BattleState == BattleState.EnemyTurn)
        {
            _enemyController.NextAction();
        }
        else//玩家回合
        {
            _curPlayerChess.SetStayGrid(_curWalkableGrid);
            _curPlayerChess.ChangeToActionEnd();
            _curPlayerChess.OnActionEnd();
            _curWalkableGrid = null;
            _curPlayerChess = null;
            //观察我方是否还有可以行动的棋子
            if (++_actionedNum == _playerList.Count)
            {
                OnEnemyTurn();
            }
            else
            {
                BattleState = BattleState.WaitSelect;
                MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
            }
        }
    }

    /// <summary>
    /// 将虚像解除，将棋子送往该格子位置，期间无法选中其他格子
    /// </summary>
    /// <param name="grid"></param>
    private void ConfirmWalkableGrid(MapGrid grid)
    {
        if (_curPlayerChess != null)
        {
            _virtualChess.SetActive(false);
            _map.CancelLastHighlightGrids(false);
            //记录初始位置
            _lastOriginGrid = _curPlayerChess.StayGrid;
            //设置棋子的新位置
            _curWalkableGrid = grid;
            //获取路径，然后让棋子按路径走过去，走的过程屏蔽操作
            List<MapGrid> path = _map.PathFinding(_curPlayerChess, _lastOriginGrid, grid, new AStarPathFinding());
            MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
            _curPlayerChess.Move(path, OnChessMoveComplet);          
        }
    }

    /// <summary>
    /// 在指定格子生成棋子虚像
    /// </summary>
    private void LoadVirtualChessOnGrid(MapGrid grid)
    {
        if (_curPlayerChess != null)
        {
            _virtualChess.transform.position = grid.transform.position;
            _virtualChess.SetActive(true);
        }
    }
    private void LoadVirtualChess()
    {
        _virtualChess = Instantiate(Resources.Load<GameObject>("Chess/001"));
        SpriteRenderer m_virtualRenderer = _virtualChess.GetComponent<SpriteRenderer>();
        m_virtualRenderer.color = new Color(m_virtualRenderer.color.r, m_virtualRenderer.color.g, m_virtualRenderer.color.b, 0.5f);
        _virtualChess.SetActive(false);
    }
    
    private void OnChessMoveComplet()
    {      
        MessageCenter.Instance.Broadcast(MessageType.OnChessAction);
        BattleState = BattleState.WaitAttack;
        _curPlayerChess.ChangeToWaitAttack();
        _curPlayerChess.SetStayGrid(_curWalkableGrid);
    }

    //取消当前棋子的移动操作
    public void OnCancelMove()
    {
        //将棋子送往原来的位置，重新高亮寻路网络
        _curWalkableGrid = null;
        _curPlayerChess.SetStayGrid(_lastOriginGrid);
        _curPlayerChess.ChangeToWaitMove();
        _curPlayerChess.CancelMove();
        _map.ShowLastHighlightGrids();
        BattleState = BattleState.WaitMove;
        MessageCenter.Instance.Broadcast(MessageType.OnCancelMove);
        MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        Camera.main.GetComponent<CameraController>().MoveToTarget(_curPlayerChess.StayGrid.transform.position);
    }
    #endregion

    #region 使用技能
    private void OnClickSkillBtn(Skill skill)
    {
        _curUsedSkill = skill;
        //通知地图寻找该技能能攻击到的目标
        _map.SearchAttackableTarget(_curPlayerChess, skill);
    }

    public bool IsUpCanAttack()
    {
        return _map.upAttackableTargets.Count > 0;
    }
    public bool IsDownCanAttack()
    {
        return _map.downAttackableTargets.Count > 0;
    }
    public bool IsLeftCanAttack()
    {
        return _map.leftAttackableTargets.Count > 0;
    }
    public bool IsRightCanAttack()
    {
        return _map.rightAttackableTargets.Count > 0;
    }
    
    public void HighlightAttackableGrids(Direction dir)
    {
        List<MapGrid> targetGirds = null;
        switch (dir)
        {
            case Direction.Up:
                _map.HighlightGrids(_map.upAttackableGrids);
                targetGirds = _map.upAttackableGrids;
                break;
            case Direction.Down:
                _map.HighlightGrids(_map.downAttackableGrids);
                targetGirds = _map.downAttackableGrids;
                break;
            case Direction.Left:
                _map.HighlightGrids(_map.leftAttackableGrids);
                targetGirds = _map.leftAttackableGrids;
                break;
            case Direction.Right:
                _map.HighlightGrids(_map.rightAttackableGrids);
                targetGirds = _map.rightAttackableGrids;
                break;
            default:
                Debug.LogError("该方向的功能尚未实现！" + dir.ToString());
                break;
        }
        foreach (var grid in targetGirds)
        {
            grid.StayedChess?.ShowPreview(_curUsedSkill.GetPreview(grid.StayedChess), _curUsedSkill);
        }
    }
    public void CancelHighlightAttackableGrids(Direction dir)
    {
        List<MapGrid> targetGirds = null;
        switch (dir)
        {
            case Direction.Up:
                _map.CancelHighlightGrids(_map.upAttackableGrids);
                targetGirds = _map.upAttackableGrids;
                break;
            case Direction.Down:
                _map.CancelHighlightGrids(_map.downAttackableGrids);
                targetGirds = _map.downAttackableGrids;
                break;
            case Direction.Left:
                _map.CancelHighlightGrids(_map.leftAttackableGrids);
                targetGirds = _map.leftAttackableGrids;
                break;
            case Direction.Right:
                _map.CancelHighlightGrids(_map.rightAttackableGrids);
                targetGirds = _map.rightAttackableGrids;
                break;
            default:
                Debug.LogError("该方向的功能尚未实现！" + dir.ToString());
                break;
        }
        foreach (var grid in targetGirds)
        {
            grid.StayedChess?.HidePreview();
        }
    }

    public void UseSkillToChoosedDir(Direction dir)
    {
        List<IChess> targets = null;
        switch (dir)
        {
            case Direction.Up:
                targets = _map.upAttackableTargets;
                break;
            case Direction.Down:
                targets = _map.downAttackableTargets;
                break;
            case Direction.Left:
                targets = _map.leftAttackableTargets;
                break;
            case Direction.Right:
                targets = _map.rightAttackableTargets;
                break;
            default:
                Debug.LogError("该方向的功能尚未实现！" + dir.ToString());
                break;
        }
        _curUsedSkill.UseSkill(targets, dir);
        _curUsedSkill = null;
    }
    #endregion

    #region AI使用技能
    //public List<IChess> GetAttackableTargetsByBFS(IChess chess, Skill skill, string tag)
    //{
    //    return _map.GetAttackableTargetsByBFS(chess, skill, tag);
    //}
    #endregion

    #region 流程控制
    public void OnEnemyDead(IChess chess)
    {
        _enemyList.Remove(chess);
        if (_enemyList.Count == 0)
        {
            //游戏胜利
            Victory();
        }
    }
    public void OnPlayerDead(IChess chess)
    {
        _playerList.Remove(chess);
        if (_playerList.Count == 0)
        {
            //游戏失败
            Defeat();
        }
    }
    public bool IsChessAlive(IChess chess)
    {
        if (chess.Tag == TagDefine.ENEMY) return _enemyList.Contains(chess);
        else return _playerList.Contains(chess);
    }

    private void OnPlayerTurn()
    {
        MessageCenter.Instance.Broadcast(MessageType.OnPlayerTurn);
        _actionedNum = 0;
        foreach (var chess in _playerList)
        {
            chess.OnTurnStart();
        }
        BattleState = BattleState.WaitSelect;
    }
    private void OnEnemyTurn()
    {
        MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        MessageCenter.Instance.Broadcast(MessageType.OnEnemyTurn);
        BattleState = BattleState.EnemyTurn;
        foreach (var chess in _enemyList)
        {
            chess.OnTurnStart();
        }
        //启动敌人策略AI

    }

    private void Victory()
    {
        Debug.Log("游戏胜利");
        MessageCenter.Instance.Broadcast(MessageType.OnVictory);
    }
    private void Defeat()
    {
        Debug.Log("游戏失败");
        MessageCenter.Instance.Broadcast(MessageType.OnDefeat);
    }
    #endregion

    #region 事件注册
    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.AddListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
        MessageCenter.Instance.AddListener<Skill>(MessageType.OnClickSkillBtn, OnClickSkillBtn);
        MessageCenter.Instance.AddListener(MessageType.OnSelectUnwalkableChess, OnSelectUnwalkableChess);
        MessageCenter.Instance.AddListener(MessageType.OnChessActionEnd, OnChessActionEnd);
    }
    private void RemoveAll()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.RemoveListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
        MessageCenter.Instance.RemoveListener<Skill>(MessageType.OnClickSkillBtn, OnClickSkillBtn);
        MessageCenter.Instance.RemoveListener(MessageType.OnSelectUnwalkableChess, OnSelectUnwalkableChess);
        MessageCenter.Instance.RemoveListener(MessageType.OnChessActionEnd, OnChessActionEnd);

    }
    #endregion
}
