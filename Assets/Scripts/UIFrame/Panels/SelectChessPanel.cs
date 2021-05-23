using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;

public class SelectChessPanel : BasePanel
{
    private Button btnConfirm;
    private Button btnBack;

    private Transform _infoField;
    private ChessInfoField _chessInfoPanel;

    private int _selectedNum = 0;
    private ChessIcon[] _chessIcons;
    private ChessIcon _curIcon = null;

    private void Awake()
    {
        btnConfirm = transform.Find("btnConfirm").GetComponent<Button>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnConfirm.onClick.AddListener(OnClickConfirmBtn);
        btnBack.onClick.AddListener(() => { UIManager.Instance.PopPanel(); });
        btnConfirm.gameObject.SetActive(false);

        
        _infoField = transform.Find("InfoField");
        _chessInfoPanel = _infoField.GetComponent<ChessInfoField>();

        var iconList = transform.Find("IconList");
        _chessIcons = iconList.GetComponentsInChildren<ChessIcon>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _infoField.gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        base.OnResume();
        _infoField.gameObject.SetActive(false);
    }

    private void OnClickConfirmBtn()
    {
        UIManager.Instance.PushPanel(UIPanelType.SelectEnemy);
        //记录三个棋子
        foreach (var item in _chessIcons)
        {
            BattleSystem.Instance.AddPlayerChessToLoad(item.chessId);
        }
    }

    public bool OnClickChessIcon(ChessIcon icon)
    {
        if (_curIcon != null)
        {
            _curIcon.OnChangeClick();
        }
        _curIcon = icon;
        ShowChessInfo(icon);
        if (_selectedNum == 3) return false;
        return true;
    }

    public void OnSelectChess()
    {
        _selectedNum++;
        if (_selectedNum == 3)
        {
            btnConfirm.gameObject.SetActive(true);
        }
    }

    public void OnCancelSelectChess()
    {
        _curIcon = null;
        if (_selectedNum == 3)
        {
            btnConfirm.gameObject.SetActive(false);
        }
        _selectedNum--;
    }

    public void ShowChessInfo(ChessIcon icon)
    {
        _infoField.gameObject.SetActive(true);
        _chessInfoPanel.ShowChessInfo(icon);
    }

    //public void HideChessInfo()
    //{
    //    _infoField.gameObject.SetActive(false);
    //}
}
