using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework.UIManager;

public class GameRoot : MonoBehaviour
{
    void Start()
    {
        DataLibsManager.Instance.InitAllLibs();
        UIManager.Instance.PushPanel(UIPanelType.MainMenu);
    }
}
