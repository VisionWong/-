using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;

public class SelectChessPanel : BasePanel
{
    private Button btnConfirm;
    private Button btnBack;

    private void Awake()
    {
        btnConfirm = transform.Find("btnConfirm").GetComponent<Button>();
        btnBack = transform.Find("btnBack").GetComponent<Button>();
        btnConfirm.onClick.AddListener(() => { UIManager.Instance.PushPanel(UIPanelType.SelectEnemy); });
        btnBack.onClick.AddListener(() => { UIManager.Instance.PopPanel(); });

    }
}
