using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VFramework;

public class EnemyChessIcon : MonoBehaviour, IPointerDownHandler
{
    public int chessId;
    public Text txtSelected;
    public SelectEnemyPanel panel;

    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        txtSelected.enabled = true;
        panel.OnClickEnemyChessIcon(this);
    }

    public void OnChangeClick()
    {
        txtSelected.enabled = false;
    }

    public void SetIcon(int id)
    {
        chessId = id;
        var data = ChessLib.Instance.GetData(id);
        img.sprite = ResourceMgr.Instance.Load<Sprite>("Icon/" + id.ToString());
    }
}
