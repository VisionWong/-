using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;
using VFramework.UIManager;

public class TurnTipBoard : BasePanel
{
    private GameObject _playerField;
    private GameObject _enemyField;

    private void Awake()
    {
        _playerField = transform.Find("PlayerTurnField").gameObject;
        _enemyField = transform.Find("EnemyTurnField").gameObject;
        Hide();

        MessageCenter.Instance.AddListener(MessageType.OnPlayerTurn, OnPlayerTurn);
        MessageCenter.Instance.AddListener(MessageType.OnEnemyTurn, OnEnemyTurn);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.OnPlayerTurn, OnPlayerTurn);
        MessageCenter.Instance.RemoveListener(MessageType.OnEnemyTurn, OnEnemyTurn);
    }

    private void OnPlayerTurn()
    {
        _enemyField.SetActive(false);
        _playerField.SetActive(true);
    }
    private void OnEnemyTurn()
    {
        _playerField.SetActive(false);
        _enemyField.SetActive(true);
    }
    private void Hide()
    {
        _playerField.SetActive(false);
        _enemyField.SetActive(false);
    }
}
