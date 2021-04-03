using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

public class BattlePanel : BasePanel
{
    private Transform _gridInfo;
    private Transform _chessInfo;
    private Transform _actionField;

    private Transform _gridDes;
    private Text txtGridDes;
    private Text txtGridType;

    private Button btnSkill;
    private Button btnStay;
    private Button btnCancel;

    public override void OnEnter()
    {
        FindAll();
        HideAll();
        RegisterAll();
    }

    private void FindAll()
    {
        //棋子信息
        _chessInfo = transform.Find("ChessInfo");

        //棋格信息
        _gridInfo = transform.Find("GridInfo");
        _gridDes = _gridInfo.Find("Description");
        txtGridType = _gridInfo.Find("txtGridType").GetComponent<Text>();
        txtGridDes = _gridDes.Find("txtGridDes").GetComponent<Text>();

        //棋子行动
        _actionField = transform.Find("ActionField");
        Transform btnGroup = _actionField.Find("ButtonGroup");
        btnSkill = btnGroup.Find("btnSkill").GetComponent<Button>();
        btnSkill.onClick.AddListener(OnClickSkillBtn);
        btnStay = btnGroup.Find("btnStay").GetComponent<Button>();
        btnStay.onClick.AddListener(OnClickStayBtn);
        btnCancel = btnGroup.Find("btnCancel").GetComponent<Button>();
        btnCancel.onClick.AddListener(OnClickCancelBtn);

    }

    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectGrid, OnSelectGrid);
        MessageCenter.Instance.AddListener(MessageType.OnChessAction, ShowActionField);
        MessageCenter.Instance.AddListener(MessageType.OnCancelMove, HideActionField);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectGrid, OnSelectGrid);
        MessageCenter.Instance.RemoveListener(MessageType.OnChessAction, ShowActionField);
        MessageCenter.Instance.RemoveListener(MessageType.OnCancelMove, HideActionField);
    }

    private void Update()
    {
        if (_gridInfo.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_gridDes.gameObject.activeSelf)
                    _gridDes.gameObject.SetActive(false);
                else _gridDes.gameObject.SetActive(true);
            }
        }
    }

    private void HideAll()
    {
        _gridDes.gameObject.SetActive(false);
        _gridInfo.gameObject.SetActive(false);

        _chessInfo.gameObject.SetActive(false);

        _actionField.gameObject.SetActive(false);
    }

    private void OnSelectGrid(MapGrid grid)
    {
        if (!_gridInfo.gameObject.activeSelf)
            _gridInfo.gameObject.SetActive(true);
        txtGridType.text = grid.TypeName;
        txtGridDes.text = grid.Description;
    }

    private void ShowGridDes()
    {
        _gridDes.gameObject.SetActive(true);
    }

    private void HideGridDes()
    {
        _gridDes.gameObject.SetActive(false);
    }

    private void OnClickSkillBtn()
    {
        //根据棋子的信息呼出技能界面
    }

    private void OnClickStayBtn()
    {
        BattleSystem.Instance.ConfirmStayGrid();
        HideActionField();
    }

    private void OnClickCancelBtn()
    {
        BattleSystem.Instance.OnCancelMove();
        HideActionField();
    }

    private void ShowActionField()
    {
        _actionField.gameObject.SetActive(true);
    }

    private void HideActionField()
    {
        _actionField.gameObject.SetActive(false);
    }
}
