using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using VFramework.UIManager;

public class ChessInfoPanel : BasePanel
{
    private Text txtName;
    private Text txtPMType;
    //private Text txtSex;
    //private Text txtAbility;
    private Text txtHP;
    private Text txtAtk;
    private Text txtDef;
    private Text txtAP;
    //private Text txtSkill;


    private void Awake()
    {
        var bg = transform.Find("bg");
        var attrValueField = bg.Find("attrValueField");
        txtName = attrValueField.Find("txtName").GetComponent<Text>();
        txtPMType = attrValueField.Find("txtPMType").GetComponent<Text>();
        //txtSex = attrValueField.Find("txtSex").GetComponent<Text>();
        //txtAbility = attrValueField.Find("txtAbility").GetComponent<Text>();
        txtHP = attrValueField.Find("txtHP").GetComponent<Text>();
        txtAtk = attrValueField.Find("txtAtk").GetComponent<Text>();
        txtDef = attrValueField.Find("txtDef").GetComponent<Text>();
        txtAP = attrValueField.Find("txtAP").GetComponent<Text>();
        //txtSkill = attrValueField.Find("txtSkill").GetComponent<Text>();

        MessageCenter.Instance.AddListener<IChess>(MessageType.OnSelectChess, ShowPanel);
        MessageCenter.Instance.AddListener(MessageType.OnCancelSelectChess, HidePanel);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener<IChess>(MessageType.OnSelectChess, ShowPanel);
        MessageCenter.Instance.RemoveListener(MessageType.OnCancelSelectChess, HidePanel);
    }

    private void SetInfo(ChessAttr data)
    {
        txtName.text = data.Name;
        txtPMType.text = GetPMTypeName(data);
        txtAtk.text = data.Attack.ToString();
        txtDef.text = data.Defence.ToString();
        txtHP.text = data.HP.ToString() + " / " + data.MaxHP.ToString();
        txtAP.text = data.AP.ToString();
        
    }

    private string GetPMTypeName(ChessAttr data)
    {
        StringBuilder sb = new StringBuilder(PMTypeLib.Instance.GetData((int)data.PMType1).name);       
        if (data.PMType2 != PMType.None)
        {
            sb.Append(" + " + PMTypeLib.Instance.GetData((int)data.PMType2).name);
        }
        return sb.ToString();
    }

    private void ShowPanel(IChess chess)
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.interactable = true;
        SetInfo(chess.Attribute);
    }
    private void HidePanel()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.interactable = false;
    }
}
