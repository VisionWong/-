using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VFramework;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Skill _skill;
    public Skill Skill
    {
        get { return _skill; }
        set
        {
            _skill = value;
            GetComponentInChildren<Text>().text = _skill.Data.name;
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(()=> MessageCenter.Instance.Broadcast(MessageType.OnClickSkillBtn, Skill));
    }

    public SkillDesPanel SkillDesPanel { private get; set; }

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
        SkillDesPanel.UpdateData(Skill.Data);
        SkillDesPanel.UpdatePosition(transform.position);
    }

    private void HideSkillDes()
    {
        SkillDesPanel.gameObject.SetActive(false);
    }
}
