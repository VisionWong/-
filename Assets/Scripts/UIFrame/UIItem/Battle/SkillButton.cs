using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SkillData _data;
    public SkillData SkillData
    {
        get { return _data; }
        set
        {
            _data = value;
            GetComponentInChildren<Text>().text = _data.name;
        }
    }

    public SkillDesPanel SkillDesPanel { private get; set; }

    private void Start()
    {
        var btn = GetComponent<Button>();    
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowSkillDes();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideSkillDes();
    }

    private void ShowSkillDes()
    {
        SkillDesPanel.gameObject.SetActive(true);
        //更改信息和更新位置
        SkillDesPanel.UpdateData(SkillData);
        SkillDesPanel.UpdatePosition(transform.position);
    }

    private void HideSkillDes()
    {
        SkillDesPanel.gameObject.SetActive(false);
    }
}
