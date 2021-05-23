using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessIcon : MonoBehaviour, IPointerDownHandler
{
    public int chessId;
    public int skillId1;
    public int skillId2;
    public int skillId3;
    public Text selectedText;
    public Button selectBtn;
    public Button cancelBtn;
    public SelectChessPanel panel;
    [HideInInspector]
    public bool IsSelect = false;

    private void Awake()
    {
        selectBtn.onClick.AddListener(OnSelect);
        cancelBtn.onClick.AddListener(OnCancel);

        cancelBtn.gameObject.SetActive(false);
        selectBtn.gameObject.SetActive(false);
    }

    public void OnChangeClick()
    {
        cancelBtn.gameObject.SetActive(false);
        selectBtn.gameObject.SetActive(false);
    }

    private void OnSelect()
    {
        panel.OnSelectChess();
        selectedText.enabled = true;
        IsSelect = true;
        selectBtn.gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        panel.OnCancelSelectChess();
        selectedText.enabled = false;
        IsSelect = false;
        cancelBtn.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsSelect)
        {
            panel.OnClickChessIcon(this);
            cancelBtn.gameObject.SetActive(true);
        }
        else
        {
            if (panel.OnClickChessIcon(this))
                selectBtn.gameObject.SetActive(true);
        }
    }
}
