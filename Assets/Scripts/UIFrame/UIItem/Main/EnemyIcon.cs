using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyIcon : MonoBehaviour, IPointerDownHandler
{
    public int chessId1;
    public int chessId2;
    public int chessId3;
    public Text txtSelected;
    public SelectEnemyPanel panel;
    [HideInInspector]
    public bool IsSelect = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        txtSelected.enabled = true;
        panel.OnClickEnemyIcon(this);
    }

    public void OnChangeClick()
    {
        txtSelected.enabled = false;
    }
}
