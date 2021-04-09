using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;

public class ChessInfoPanel : BasePanel
{
    private Text txtName;
    private Text txtPMType;
    private Text txtSex;
    private Text txtAbility;
    private Text txtHP;
    private Text txtAtk;
    private Text txtDef;
    private Text txtAP;
    private Text txtSkill;


    private void Awake()
    {
        var bg = transform.Find("bg");
        var attrValueField = bg.Find("attrValueField");
        txtName = attrValueField.Find("txtName").GetComponent<Text>();
        txtPMType = attrValueField.Find("txtPMType").GetComponent<Text>();
        txtSex = attrValueField.Find("txtSex").GetComponent<Text>();
        txtAbility = attrValueField.Find("txtAbility").GetComponent<Text>();
        txtHP = attrValueField.Find("txtHP").GetComponent<Text>();
        txtAtk = attrValueField.Find("txtAtk").GetComponent<Text>();
        txtDef = attrValueField.Find("txtDef").GetComponent<Text>();
        txtAP = attrValueField.Find("txtAP").GetComponent<Text>();
        txtSkill = attrValueField.Find("txtSkill").GetComponent<Text>();

    }

    public override void OnEnter()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.interactable = true;
        SetInfo();
    }

    public override void OnExit()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.interactable = false;
    }

    private void SetInfo()
    {
        var data = BattleSystem.Instance.GetCurPlayerChess().Attribute;
        txtName.text = data.Name;
        txtPMType.text = data.PMType1.ToString();
        txtAtk.text = data.Attack.ToString();
        txtDef.text = data.Defence.ToString();
        txtHP.text = data.HP.ToString() + " / " + data.MaxHP.ToString();
        txtAP.text = data.AP.ToString();
        
    }

    private void GetPMTypeName()
    {

    }
}
