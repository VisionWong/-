using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using VFramework.UIManager;

public class PausePanel : BasePanel
{
    private Button btnBack;
    private Button btnClose;
    private bool needCanSel = false;

    private void Awake()
    {
        btnBack = transform.Find("Image").Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(OnClickBackBtn);
        btnClose = transform.Find("Image").Find("btnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(OnClickCloseBtn);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //CanvasGroup.alpha = 1;
        //CanvasGroup.interactable = true;
        //CanvasGroup.blocksRaycasts = true;
        if (GameManager.Instance.isCantSel)
        {
            needCanSel = false;
        }
        else
        {
            needCanSel = true;
            MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        }
        Time.timeScale = 0;
    }

    public override void OnExit()
    {
        base.OnExit();
        //CanvasGroup.alpha = 0;
        //CanvasGroup.interactable = false;
        //CanvasGroup.blocksRaycasts = false;
        if (needCanSel)
            MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        Time.timeScale = 1;
        GameManager.Instance.PauseEnd();
    }

    public override void OnClear()
    {
        base.OnClear();
        if (needCanSel)
            MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        Time.timeScale = 1;
        GameManager.Instance.PauseEnd();
    }

    private void OnClickBackBtn()
    {
        GameManager.Instance.EndBattle();
    }

    private void OnClickCloseBtn()
    {
        UIManager.Instance.PopPanel();
        GameManager.Instance.PauseEnd();
    }
}
