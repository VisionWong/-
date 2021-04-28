using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using VFramework.UIManager;
using DG.Tweening;

public class TurnTipBoard : BasePanel
{
    public Color playerTurn;
    public Color enemyTurn;

    private GameObject _playerField;
    private GameObject _enemyField;
    private Text _txtTurnChangeNotice;
    private Text _txtTurns;
    private int _turns = 1;

    private void Awake()
    {
        var upField = transform.Find("UpField");
        _playerField = upField.Find("PlayerTurnField").gameObject;
        _enemyField = upField.Find("EnemyTurnField").gameObject;
        Hide();

        _txtTurns = upField.Find("txtTurns").GetComponent<Text>();
        _txtTurns.text = "第" + _turns.ToString() + "回合";
        _txtTurnChangeNotice = transform.Find("txtTurnChangeNotice").GetComponent<Text>();

        MessageCenter.Instance.AddListener(MessageType.OnPlayerTurn, OnPlayerTurn);
        MessageCenter.Instance.AddListener(MessageType.OnEnemyTurn, OnEnemyTurn);
        MessageCenter.Instance.AddListener(MessageType.OnEnemyTurnEnd, OnEnemyTurnEnd);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.OnPlayerTurn, OnPlayerTurn);
        MessageCenter.Instance.RemoveListener(MessageType.OnEnemyTurn, OnEnemyTurn);
        MessageCenter.Instance.RemoveListener(MessageType.OnEnemyTurnEnd, OnEnemyTurnEnd);
    }

    private void OnPlayerTurn()
    {
        _txtTurnChangeNotice.color = playerTurn;
        _txtTurnChangeNotice.text = "我方回合";
        _txtTurnChangeNotice.DOFade(0, 1.5f);
        _enemyField.SetActive(false);
        _playerField.SetActive(true);
    }
    private void OnEnemyTurn()
    {
        _txtTurnChangeNotice.color = enemyTurn;
        _txtTurnChangeNotice.text = "敌方回合";
        _txtTurnChangeNotice.DOFade(0, 1.5f);
        _playerField.SetActive(false);
        _enemyField.SetActive(true);
    }
    private void Hide()
    {
        _playerField.SetActive(false);
        _enemyField.SetActive(false);
    }

    private void OnEnemyTurnEnd()
    {
        _turns++;
        _txtTurns.text = "第" + _turns.ToString() + "回合";
    }
}
