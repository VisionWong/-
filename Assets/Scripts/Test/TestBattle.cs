﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.UIManager;

public class TestBattle : MonoBehaviour
{
    void Start()
    {
        BattleSystem.Instance.StartBattle();
        UIManager.Instance.PushPanel(UIPanelType.Battle, true);
    }

}
