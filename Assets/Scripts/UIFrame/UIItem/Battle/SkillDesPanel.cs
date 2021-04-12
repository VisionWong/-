using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDesPanel : MonoBehaviour
{
    private Text txtPMType;
    private Text txtSkillType;
    private Text txtRangeType;
    private Text txtPower;
    private Text txtHitRate;
    private Text txtDes;

    private void Awake()
    {
        var field = transform.Find("Attrs");
        txtPMType = field.Find("txtPMType").GetComponent<Text>();
        txtSkillType = field.Find("txtSkillType").GetComponent<Text>();
        txtRangeType = field.Find("txtRangeType").GetComponent<Text>();
        txtPower = field.Find("txtPower").GetComponent<Text>();
        txtHitRate = field.Find("txtHitRate").GetComponent<Text>();
        txtDes = field.Find("txtDes").GetComponent<Text>();
    }

    public void UpdateData(SkillData data)
    {

        txtPMType.text = EnumTool.GetPMTypeName(data.pmType);
        txtSkillType.text = EnumTool.GetSkillTypeName(data.skillType);
        txtRangeType.text = data.rangeType.ToString();
        txtPower.text = data.power == 0 ? "-" : data.power.ToString();
        txtHitRate.text = data.hitRate.ToString();
        txtDes.text = data.description;
    }

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
    }

    
}
