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

    private List<int> _playerIdList = new List<int>();
    private List<int> _enemyIdList = new List<int>();

    private Map _map;
    private List<PlayerChess> _playerList = new List<PlayerChess>();
    private List<EnemyChess> _enemyList = new List<EnemyChess>();
    private AutoActionController _enemyController;
    private int _actionedNum;//已经行动完毕的玩家棋子数

    private ISelectable _curSelected = null;
    private PlayerChess _curPlayerChess = null;
    private EnemyChess _curEnemyChess = null;

    private GameObject _virtualChess = null;
    private MapGrid _curWalkableGrid = null;
    private MapGrid _lastOriginGrid = null;

    private Skill _curUsedSkill = null;

    #region 战斗准备
    public void ClearPlayerIdList()
    {
        _playerIdList.Clear();
    }
    public void AddPlayerChessToLoad(int id)
    {
        _playerIdList.Add(id);
    }

    public void AddEnemyChessToLoad(int id)
    {
        _enemyIdList.Add(id);
    }

    public void StartBattle()
    {
        RegisterAll();
        LoadMap();
        LoadPlayerChess();
        LoadEnemyChess();
        LoadVirtualChess();
        //TODO 我方布阵
        //TODO 敌方特性触发
        //TODO 我方特性触发
        //我方回合开始
        //TODO 先环视敌方棋子，再回到自己的棋子
        OnPlayerTurn();
    }

    private void LoadMap()
    {
        Map map = ResourceMgr.Instance.Load<Map>("Map/Map001");
        _map = Instantiate(map, Vector3.zero, Quaternion.identity).GetComponent<Map>();
        //TODO 根据关卡信息生成地图或随机生成
    }

    private void LoadPlayerChess()
    {
        //TODO 根据玩家背包里的信息生成棋子，位置则根据关卡默认位置，玩家后续可在区域内调整
        foreach (var id in _playerIdList)
        {
            int rx = UnityEngine.Random.Range(1, _map.col - 1);
            int ry = UnityEngine.Random.Range(_map.row - 3, _map.row);
            if (_map.GetGridByCoord(rx, ry).StayedChess != null)
            {
                rx = UnityEngine.Random.Range(1, _map.col - 1);
                ry = UnityEngine.Random.Range(_map.row - 3, _map.row);
            }
            LoadPlayerChess(rx, ry, id);
        }
    }   

    private void LoadPlayerChess(int x, int y, int id)
    {
        var chess = ChessFactory.ProducePlayer(id);        
        MapGrid grid = _map.GetGridByCoord(x, y);
        chess.SetStayGrid(grid);
        _playerList.Add(chess);
    }

    public void LoadEnemyChess()
    {
        //TODO 据关卡信息生成
        foreach (var id in _enemyIdList)
        {
            int rx = UnityEngine.Random.Range(1, _map.col - 1);
            int ry = UnityEngine.Random.Range(0, 3);
            if (_map.GetGridByCoord(rx, ry).StayedChess != null)
            {
                rx = UnityEngine.Random.Range(1, _map.col - 1);
                ry = UnityEngine.Random.Range(0, 3);
            }
            LoadEnemyChess(rx, ry, id);
        }
        _enemyController = new AutoActionController(_enemyList, _playerList);
    }

    private void LoadEnemyChess(int x, int y, int id)
    {
        var chess = ChessFactory.ProduceEnemy(id, new CommonAIController(_playerList, _map));
        MapGrid grid = _map.GetGridByCoord(x, y);
        chess.SetStayGrid(grid);
        _enemyList.Add(chess);
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
    private void CancelPlayerMoving()
    {
        if (_virtualChess.activeSelf) _virtualChess.SetActive(false);
        _curWalkableGrid = null;

        _curPlayerChess.ChangeToIdle();
        _curPlayerChess = null;

        _map.CancelLastHighlightGrids();
    }
    public void CancelEnemyHighlight()
    {
        _curEnemyChess = null;
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
                CancelPlayerMoving();
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
                CancelPlayerMoving();
            }
            _curPlayerChess = null;
            BattleState = BattleState.WaitSelect;
        }
    }

    private void OnSelectEnemyChess(EnemyChess chess)
    {
        if (_curPlayerChess != null)
        {
            CancelPlayerMoving();
            BattleState = BattleState.WaitSelect;
        }
        _map.HighlightWalkableGrids(chess);
    }

    private void OnSelectIdleGrid(MapGrid grid)
    {
        if (BattleState == BattleState.WaitMove)
        {
            CancelPlayerMoving();
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

    public void OnChessActionEnd(IChess chess)
    {
        if (chess is PlayerChess)
        {
            var c = chess as PlayerChess;
            c.ChangeToActionEnd();
            c.OnActionEnd();
            _actionedNum++;
        }
        else
        {
            var c = chess as EnemyChess;
            c.ChangeToActionEnd();
            c.OnActionEnd();
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
        _curPlayerChess.OnUseSkill(_curUsedSkill);
        _curUsedSkill.UseSkill(targets, dir);
        _curUsedSkill = null;
    }
    #endregion

    #region 流程控制
    public void OnChessDead(IChess chess)
    {
        if (chess.Tag == TagDefine.PLAYER)
            OnPlayerDead(chess as PlayerChess);
        else
            OnEnemyDead(chess as EnemyChess);
    }
    private void OnEnemyDead(EnemyChess chess)
    {
        _enemyList.Remove(chess);
        if (_enemyList.Count == 0)
        {
            //游戏胜利
            Victory();
        }
    }
    private void OnPlayerDead(PlayerChess chess)
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
        if (chess.Tag == TagDefine.ENEMY) return _enemyList.Contains(chess as EnemyChess);
        else return _playerList.Contains(chess as PlayerChess);
    }

    private void OnPlayerTurn()
    {
        MessageCenter.Instance.Broadcast(MessageType.OnPlayerTurn);
        _actionedNum = 0;
        foreach (var chess in _playerList)
        {
            Debug.Log(chess.GameObject.name);
            chess.ChangeToIdle();
            chess.OnTurnStart();
        }
        if (_actionedNum == _playerList.Count)
        {
            OnEnemyTurn();
        }
        else
        {
            int ran = UnityEngine.Random.Range(0, _playerList.Count);
            Camera.main.GetComponent<CameraController>().MoveToTarget(_playerList[ran].GameObject.transform.position);
            BattleState = BattleState.WaitSelect;
            MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        }
    }
    private void OnEnemyTurn()
    {
        if (_enemyList.Count == 0) return;
        MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        MessageCenter.Instance.Broadcast(MessageType.OnEnemyTurn);
        BattleState = BattleState.EnemyTurn;
        foreach (var chess in _enemyList)
        {
            chess.ChangeToIdle();
            chess.OnTurnStart();
        }
        //启动敌人策略AI
        _enemyController.StartAction();
    }
    private void OnEnemyTurnEnd()
    {
        //TODO 判断是否回合数超出关卡限制，是则判负
        OnPlayerTurn();
    }
    private void Victory()
    {
        Debug.Log("游戏胜利");
        MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        MessageCenter.Instance.Broadcast(MessageType.OnVictory);
        Time.timeScale = 0;
    }
    private void Defeat()
    {
        Debug.Log("游戏失败");
        MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        MessageCenter.Instance.Broadcast(MessageType.OnDefeat);
        Time.timeScale = 0;
    }
    #endregion

    #region 事件注册
    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.AddListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
        MessageCenter.Instance.AddListener<EnemyChess>(MessageType.OnSelectEnemyChess, OnSelectEnemyChess);
        MessageCenter.Instance.AddListener<Skill>(MessageType.OnClickSkillBtn, OnClickSkillBtn);
        MessageCenter.Instance.AddListener(MessageType.OnSelectUnwalkableChess, OnSelectUnwalkableChess);
        MessageCenter.Instance.AddListener(MessageType.OnChessActionEnd, OnChessActionEnd);
        MessageCenter.Instance.AddListener(MessageType.OnEnemyTurnEnd, OnEnemyTurnEnd);
    }
    private void RemoveAll()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectIdleGrid);
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectWalkableGrid, OnSelectWalkableGrid);
        MessageCenter.Instance.RemoveListener<PlayerChess>(MessageType.OnSelectWalkableChess, OnSelectWalkableChess);
        MessageCenter.Instance.RemoveListener<EnemyChess>(MessageType.OnSelectEnemyChess, OnSelectEnemyChess);
        MessageCenter.Instance.RemoveListener<Skill>(MessageType.OnClickSkillBtn, OnClickSkillBtn);
        MessageCenter.Instance.RemoveListener(MessageType.OnSelectUnwalkableChess, OnSelectUnwalkableChess);
        MessageCenter.Instance.RemoveListener(MessageType.OnChessActionEnd, OnChessActionEnd);
        MessageCenter.Instance.RemoveListener(MessageType.OnEnemyTurnEnd, OnEnemyTurnEnd);
    }
    #endregion
}
