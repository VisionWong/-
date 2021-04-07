using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Direction
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class SkillDirButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Direction _dir = Direction.None;
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(() => BattleSystem.Instance.UseSkillToChoosedDir(_dir));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_btn.enabled)
            BattleSystem.Instance.HighlightAttackableGrids(_dir);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_btn.enabled)
            BattleSystem.Instance.CancelHighlightAttackableGrids(_dir);
    }
}
