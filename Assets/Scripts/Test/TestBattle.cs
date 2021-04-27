using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.UIManager;

public class TestBattle : MonoBehaviour
{
    void Start()
    {
        DataLibsManager.Instance.InitAllLibs();
        UIManager.Instance.PushPanel(UIPanelType.Battle, true);
        BattleSystem.Instance.StartBattle();

    }

}
