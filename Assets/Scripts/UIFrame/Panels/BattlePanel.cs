using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using VFramework.UIManager;

public class BattlePanel : BasePanel
{
    private Transform _gridInfo;
    private Transform _chessInfo;
    private Transform _actionField;
    private Transform _skillField;
    private Transform _skillGroup;

    private Transform _gridDes;
    private Text txtGridDes;
    private Text txtGridType;

    private Button btnSkill;
    private Button btnStay;
    private Button btnCancel;

    //技能面板
    private bool isNewAction = true;
    private bool isSkillOpen = false;
    private List<SkillButton> _skillBtns = new List<SkillButton>();
    private Transform _skillDesPanel;

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

        //技能面板
        _skillField = transform.Find("SkillField");
        _skillGroup = _skillField.Find("SkillGroup");
        _skillDesPanel = _skillField.Find("SkillDes");
        for (int i = 0; i < 4; i++)
        {
            //新建技能按钮
            var go = ResourceMgr.Instance.Load<GameObject>(PathDefine.UI_SKILL_BUTTON);
            go.transform.SetParent(_skillGroup, false);
            var skillBtn = go.GetComponent<SkillButton>();
            _skillBtns.Add(skillBtn);
            skillBtn.SkillDesPanel = _skillDesPanel.GetComponent<SkillDesPanel>();
        }
    }

    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectGrid, OnSelectGrid);
        MessageCenter.Instance.AddListener(MessageType.OnChessAction, ShowActionField);
        MessageCenter.Instance.AddListener(MessageType.OnCancelMove, HideActionField);
        MessageCenter.Instance.AddListener(MessageType.OnSearchAttackableEnd, ShowSkillDirPanel);
        MessageCenter.Instance.AddListener(MessageType.OnClickDirCancelBtn, ReShowActionField);
    }
    private void RemoveAll()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectGrid, OnSelectGrid);
        MessageCenter.Instance.RemoveListener(MessageType.OnChessAction, ShowActionField);
        MessageCenter.Instance.RemoveListener(MessageType.OnCancelMove, HideActionField);
        MessageCenter.Instance.RemoveListener(MessageType.OnSearchAttackableEnd, ShowSkillDirPanel);
        MessageCenter.Instance.RemoveListener(MessageType.OnClickDirCancelBtn, ReShowActionField);
    }

    private void OnDestroy()
    {
        RemoveAll();
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
        _skillField.gameObject.SetActive(false);
        _skillDesPanel.gameObject.SetActive(false);
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
        if (isSkillOpen) HideSkillField();
        else ShowSkillField();
    }

    private void OnClickStayBtn()
    {
        BattleSystem.Instance.OnChessActionEnd();
        HideActionField();
    }

    private void OnClickCancelBtn()
    {
        BattleSystem.Instance.OnCancelMove();
        HideActionField();
    }

    private void ShowActionField()
    {
        isNewAction = true;
        _actionField.gameObject.SetActive(true);
    }

    private void ReShowActionField()
    {
        _actionField.gameObject.SetActive(true);
    }

    private void HideActionField()
    {
        _actionField.gameObject.SetActive(false);
        HideSkillField();
    }

    private void HideSkillField()
    {
        if (_skillField.gameObject.activeSelf)
        {
            _skillField.gameObject.SetActive(false);
            isSkillOpen = false;
            if (_skillDesPanel.gameObject.activeSelf)
            {
                _skillDesPanel.gameObject.SetActive(false);
            }
        }
    }

    private void ShowSkillField()
    {
        isSkillOpen = true;
        _skillField.gameObject.SetActive(true);
        if (isNewAction)
        {
            //删除之前的技能信息
            foreach (var item in _skillBtns)
            {
                if (item.gameObject.activeSelf)
                    item.gameObject.SetActive(false);
            }
            //获取该棋子的技能信息
            var skillList = BattleSystem.Instance.GetCurPlayerChess().SkillList;
            for (int i = 0; i < skillList.Count; i++)
            {
                _skillBtns[i].gameObject.SetActive(true);
                _skillBtns[i].Skill = skillList[i];
            }
        }
        isNewAction = false;
    }

    private void ShowSkillDirPanel()
    {
        UIManager.Instance.PushPanel(UIPanelType.SkillDir, true);
        HideActionField();
    }
}
