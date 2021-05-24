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
        MessageCenter.Instance.Broadcast(MessageType.GlobalCantSelect);
        Time.timeScale = 0;
    }

    public override void OnExit()
    {
        base.OnExit();
        MessageCenter.Instance.Broadcast(MessageType.GlobalCanSelect);
        Time.timeScale = 1;
    }

    public override void OnClear()
    {
        base.OnClear();
        Time.timeScale = 1;
    }

    private void OnClickBackBtn()
    {
        GameManager.Instance.EndBattle();
    }

    private void OnClickCloseBtn()
    {
        UIManager.Instance.PopPanel();
    }
}
