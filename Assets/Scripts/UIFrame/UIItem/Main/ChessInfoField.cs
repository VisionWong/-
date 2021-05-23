using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessInfoField : MonoBehaviour
{
    private Text txtName;
    private Text txtPMType1;
    private Text txtPMType2;
    private Text txtHP;
    private Text txtAP;
    private Text txtAtk;
    private Text txtDef;
    private SkillInfoBtn btnSkill1;
    private SkillInfoBtn btnSkill2;
    private SkillInfoBtn btnSkill3;

    private Transform _skillInfoFiled;
    private Text txtSkillName;
    private Text txtSkillPMType;
    private Text txtSkillType;
    private Text txtSkillPower;
    private Text txtSkillHitRate;
    private Text txtSkillDes;
    private Text txtSkillRange;

    private void Awake()
    {
        var field = transform.Find("ChessInfoField").Find("AttrValues");
        txtName = field.Find("txtName").GetComponent<Text>();
        txtPMType1 = field.Find("txtType1").GetComponent<Text>();
        txtPMType2 = field.Find("txtType2").GetComponent<Text>();
        txtHP = field.Find("txtHp").GetComponent<Text>();
        txtAtk = field.Find("txtAtk").GetComponent<Text>();
        txtDef = field.Find("txtDef").GetComponent<Text>();
        txtAP = field.Find("txtAP").GetComponent<Text>();
        btnSkill1 = field.Find("btnSkill1").GetComponent<SkillInfoBtn>();
        btnSkill2 = field.Find("btnSkill2").GetComponent<SkillInfoBtn>();
        btnSkill3 = field.Find("btnSkill3").GetComponent<SkillInfoBtn>();

        _skillInfoFiled = transform.Find("SkillInfoField");
        var skillInfoFiled = _skillInfoFiled.Find("AttrValues");
        txtSkillName = skillInfoFiled.Find("txtName").GetComponent<Text>();
        txtSkillPMType = skillInfoFiled.Find("txtPMType").GetComponent<Text>();
        txtSkillType = skillInfoFiled.Find("txtType").GetComponent<Text>();
        txtSkillPower = skillInfoFiled.Find("txtPower").GetComponent<Text>();
        txtSkillHitRate = skillInfoFiled.Find("txtHitRate").GetComponent<Text>();
        txtSkillDes = skillInfoFiled.Find("txtDes").GetComponent<Text>();
        txtSkillRange = skillInfoFiled.Find("txtRange").GetComponent<Text>();
        _skillInfoFiled.gameObject.SetActive(false);
    }

    public void ShowChessInfo(ChessIcon icon)
    {
        _skillInfoFiled.gameObject.SetActive(false);

        var info = ChessLib.Instance.GetData(icon.chessId);
        txtName.text = info.name;
        txtPMType1.text = EnumTool.GetPMTypeName(info.pmType1);
        if (info.pmType2 != PMType.None)
            txtPMType2.text = EnumTool.GetPMTypeName(info.pmType2);
        else
            txtPMType2.text = "无";
        txtHP.text = info.hp.ToString();
        txtAtk.text = info.attack.ToString();
        txtDef.text = info.defence.ToString();
        txtAP.text = info.ap.ToString();
        btnSkill1.SetInfo(info.skillIdList[1]);
        btnSkill2.SetInfo(info.skillIdList[2]);
        btnSkill3.SetInfo(info.skillIdList[3]);
    }

    public void ShowChessInfo(EnemyChessIcon icon)
    {
        _skillInfoFiled.gameObject.SetActive(false);

        var info = ChessLib.Instance.GetData(icon.chessId);
        txtName.text = info.name;
        txtPMType1.text = EnumTool.GetPMTypeName(info.pmType1);
        if (info.pmType2 != PMType.None)
            txtPMType2.text = EnumTool.GetPMTypeName(info.pmType2);
        else
            txtPMType2.text = "无";
        txtHP.text = info.hp.ToString();
        txtAtk.text = info.attack.ToString();
        txtDef.text = info.defence.ToString();
        txtAP.text = info.ap.ToString();
        btnSkill1.SetInfo(info.skillIdList[1]);
        btnSkill2.SetInfo(info.skillIdList[2]);
        btnSkill3.SetInfo(info.skillIdList[3]);
    }

    public void OnClickSkillBtn(SkillData data)
    {
        _skillInfoFiled.gameObject.SetActive(true);
        txtSkillName.text = data.name;
        txtSkillPMType.text = EnumTool.GetPMTypeName(data.pmType);
        txtSkillType.text = EnumTool.GetSkillTypeName(data.skillType);
        txtSkillPower.text = data.power.ToString();
        txtSkillHitRate.text = data.hitRate.ToString();
        txtSkillDes.text = data.description;
        string str = "";
        foreach (var item in data.range)
        {
            str += "(";
            str += item.x.ToString();
            str += ",";
            str += item.y.ToString();
            str += ")";
        }
        txtSkillRange.text = str;
    }
}
