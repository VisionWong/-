using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VFramework;
using VFramework.UIManager;

public class GameManager : MonoSingleton<GameManager>
{
    private bool isBattle = false;

    private List<int> _playerIdList = new List<int>();
    private List<int> _enemyIdList = new List<int>();
    private GameObject go;

    public void Init()
    {
        DataLibsManager.Instance.InitAllLibs();
        UIManager.Instance.PushPanel(UIPanelType.MainMenu);

        MessageCenter.Instance.AddListener(MessageType.OnVictory, OnBattleEnd);
        MessageCenter.Instance.AddListener(MessageType.OnDefeat, OnBattleEnd);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.OnVictory, OnBattleEnd);
        MessageCenter.Instance.RemoveListener(MessageType.OnDefeat, OnBattleEnd);
    }

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
        //加载战斗场景
        StartCoroutine(WaitForLoadScene());
    }

    private IEnumerator WaitForLoadScene()
    {
        var ar = SceneManager.LoadSceneAsync(2);
        while (ar.progress < 1)
        {
            yield return ar.progress;
        }
        UIManager.Instance.Clear();
        UIManager.Instance.PushPanel(UIPanelType.Battle);
        go = BattleSystem.Instance.GameObject;
        BattleSystem.Instance.SetChessIdList(_playerIdList, _enemyIdList);
        BattleSystem.Instance.StartBattle();
        isBattle = true;
    }

    public void EndBattle()
    {
        isBattle = false;
        Destroy(go);
        _playerIdList.Clear();
        _enemyIdList.Clear();
        //加载主界面
        SceneManager.LoadScene(1);
        UIManager.Instance.Clear();
        UIManager.Instance.PushPanel(UIPanelType.MainMenu);
    }

    private void Update()
    {
        if (isBattle && Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.PushPanel(UIPanelType.Pause, true);
        }
    }

    private void OnBattleEnd()
    {
        isBattle = false;
    }
}
