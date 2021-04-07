using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using VFramework.UIManager;

public class SkillDirPanel : BasePanel
{
    private Button btnUp;
    private Button btnDown;
    private Button btnLeft;
    private Button btnRight;
    private Button btnCancel;

    private void Awake()
    {
        btnUp = transform.Find("btnUp").GetComponent<Button>();
        btnDown = transform.Find("btnDown").GetComponent<Button>();
        btnLeft = transform.Find("btnLeft").GetComponent<Button>();
        btnRight = transform.Find("btnRight").GetComponent<Button>();
        btnCancel = transform.Find("btnCancel").GetComponent<Button>();

        MessageCenter.Instance.AddListener(MessageType.OnClickDirCancelBtn, OnClickCancelBtn);
        btnCancel.onClick.AddListener(() => MessageCenter.Instance.Broadcast(MessageType.OnClickDirCancelBtn));
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.OnClickDirCancelBtn, OnClickCancelBtn);
    }

    public override void OnEnter()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        //获取可以攻击的朝向
        btnUp.interactable = BattleSystem.Instance.IsUpCanAttack();
        btnDown.interactable = BattleSystem.Instance.IsDownCanAttack();
        btnLeft.interactable = BattleSystem.Instance.IsLeftCanAttack();
        btnRight.interactable = BattleSystem.Instance.IsRightCanAttack();
        if (!(btnUp.enabled || btnDown.enabled || btnLeft.enabled || btnRight.enabled))
        {
            //TODO 通知玩家这个技能作用不到任何目标
            Debug.Log("这个技能作用不到任何目标");
        }
    }

    public override void OnExit()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
    }

    private void OnClickCancelBtn()
    {
        UIManager.Instance.PopPanel();
    }
}
