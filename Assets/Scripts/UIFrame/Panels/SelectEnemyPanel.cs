using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;

public class SelectEnemyPanel : BasePanel
{
    private Button btnConfirm;
    private Button btnBack;

    private Transform _chessIconField;
    private EnemyChessIcon[] _chessIcons;

    private Transform _infoField;
    private ChessInfoField _chessInfoPanel;

    private EnemyIcon _curEnemyIcon = null;
    private EnemyChessIcon _curChessIcon = null;

    private void Awake()
    {
        btnConfirm = transform.Find("btnConfirm").GetComponent<Button>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);
        btnBack.onClick.AddListener(() => { UIManager.Instance.PopPanel(); });
        btnConfirm.gameObject.SetActive(false);

        _chessIconField = transform.Find("PMIconList");
        _chessIcons = _chessIconField.GetComponentsInChildren<EnemyChessIcon>();
        _chessIconField.gameObject.SetActive(false);

        _infoField = transform.Find("InfoField");
        _chessInfoPanel = _infoField.GetComponent<ChessInfoField>();
        _infoField.gameObject.SetActive(false);
    }

    public void OnClickEnemyIcon(EnemyIcon icon)
    {
        btnConfirm.gameObject.SetActive(true);
        if (_curEnemyIcon != null && _curEnemyIcon != icon)
        {
            _curEnemyIcon.OnChangeClick();
        }
        _curEnemyIcon = icon;
        ShowChessIconList(icon);
    }

    public void OnClickEnemyChessIcon(EnemyChessIcon icon)
    {
        if (_curChessIcon != null && _curChessIcon != icon)
        {
            _curChessIcon.OnChangeClick();
        }
        _curChessIcon = icon;
        ShowChessInfo(icon);
    }

    public void ShowChessInfo(EnemyChessIcon icon)
    {
        _infoField.gameObject.SetActive(true);
        _chessInfoPanel.ShowChessInfo(icon);
    }

    private void ShowChessIconList(EnemyIcon icon)
    {
        _chessIconField.gameObject.SetActive(true);
        foreach (var item in _chessIcons)
        {
            item.OnChangeClick();
        }
        _curChessIcon = null;
        _infoField.gameObject.SetActive(false);
        _chessIcons[0].SetIcon(icon.chessId1);
        _chessIcons[1].SetIcon(icon.chessId2);
        _chessIcons[2].SetIcon(icon.chessId3);
    }

    private void OnClickConfirmBtn()
    {
        GameManager.Instance.AddEnemyChessToLoad(_curEnemyIcon.chessId1);
        GameManager.Instance.AddEnemyChessToLoad(_curEnemyIcon.chessId2);
        GameManager.Instance.AddEnemyChessToLoad(_curEnemyIcon.chessId3);
        GameManager.Instance.SetBgmPath(_curEnemyIcon.bgmPath);
        GameManager.Instance.StartBattle();
        //UIManager.Instance.Clear();
        //UIManager.Instance.PushPanel(UIPanelType.Battle);
        //BattleSystem.Instance.StartBattle();
    }
}
