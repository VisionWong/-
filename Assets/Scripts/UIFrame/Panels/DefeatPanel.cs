﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework.UIManager;
using DG.Tweening;
using VFramework;

public class DefeatPanel : BasePanel
{
    private Button btnBack;

    private void Awake()
    {
        btnBack = transform.Find("Image").Find("btnBack").GetComponent<Button>();
        btnBack.onClick.AddListener(OnClickBackBtn);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        AudioMgr.Instance.PlayBGM(PathDefine.AUDIO_DEFEAT);
    }

    private void OnClickBackBtn()
    {
        GameManager.Instance.EndBattle();
    }
}
