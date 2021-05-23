using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoBtn : MonoBehaviour
{
    public ChessInfoField field;

    private Button btn;
    private Text txt;
    private SkillData data;

    private void Awake()
    {
        btn = GetComponent<Button>();
        txt = GetComponentInChildren<Text>();
        btn.onClick.AddListener(() => field.OnClickSkillBtn(data));
    }

    public void SetInfo(int id)
    {
        var info = SkillLib.Instance.GetData(id);
        txt.text = info.name;
        data = info;
    }
}
