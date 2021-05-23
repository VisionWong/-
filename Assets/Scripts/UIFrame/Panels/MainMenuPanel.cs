using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;

public class MainMenuPanel : BasePanel
{
    private Transform leftField;
    private Button btnStart;
    private Button btnQuit;

    private void Awake()
    {
        leftField = transform.Find("LeftField");
        btnStart = leftField.Find("btnStart").GetComponent<Button>();
        btnQuit = leftField.Find("btnQuit").GetComponent<Button>();

        btnStart.onClick.AddListener(()=> UIManager.Instance.PushPanel(UIPanelType.SelectChess));
        btnQuit.onClick.AddListener(() => Application.Quit());
    }

    public override void OnPause()
    {
        base.OnPause();
        //leftField.gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        base.OnResume();
        //leftField.gameObject.SetActive(true);
    }
}
