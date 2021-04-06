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

        txtPMType.text = GetPMTypeName(data.pmType);
        txtSkillType.text = GetSkillTypeName(data.skillType);
        txtRangeType.text = data.rangeType.ToString();
        txtPower.text = data.power.ToString();
        txtHitRate.text = data.hitRate.ToString();
        txtDes.text = data.description;
    }

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
    }

    private string GetPMTypeName(PMType type)
    {
        string str = null;
        switch (type)
        {
            case PMType.None:
                str = "无";
                break;
            case PMType.Grass:
                str = "草";
                break;
            case PMType.Fire:
                str = "火";
                break;
            case PMType.Water:
                str = "水";
                break;
            case PMType.Electric:
                str = "电";
                break;
            case PMType.Fight:
                str = "格斗";
                break;
            case PMType.Ground:
                str = "地面";
                break;
            case PMType.Rock:
                str = "岩石";
                break;
            case PMType.Metal:
                str = "钢";
                break;
            case PMType.Bug:
                str = "虫";
                break;
            case PMType.Poison:
                str = "毒";
                break;
            case PMType.Fly:
                str = "飞行";
                break;
            case PMType.Ghost:
                str = "幽灵";
                break;
            case PMType.Psychic:
                str = "超能";
                break;
            case PMType.Common:
                str = "一般";
                break;
            case PMType.Dragon:
                str = "龙";
                break;
            case PMType.Dark:
                str = "恶";
                break;
            case PMType.Fairy:
                str = "妖精";
                break;
            case PMType.Ice:
                str = "冰";
                break;
            default:
                Debug.LogError("尚不存在该类型" + type.ToString());
                break;
        }
        return str;
    }

    private string GetSkillTypeName(SkillType type)
    {
        string str = null;
        switch (type)
        {
            case SkillType.Damage:
                str = "伤害";
                break;
            case SkillType.Heal:
                str = "治疗";
                break;
            case SkillType.Effect:
                str = "变化";
                break;
            default:
                Debug.LogError("尚不存在该类型" + type.ToString());
                break;
        }
        return str;
    }
}
