using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    private Transform _hpBarBg;
    private Image _hpBar;

    private void Awake()
    {
        _hpBarBg = transform.Find("Canvas").Find("bg");
        _hpBar = _hpBarBg.Find("hpBar").GetComponent<Image>();
        _hpBarBg.gameObject.SetActive(false);
    }

    public void ChangeHPValue(float hp, float maxHp, float duration = 1f)
    {
        _hpBarBg.gameObject.SetActive(true);
        StartCoroutine(WaitForHpLose(hp, maxHp, duration));
    }

    private IEnumerator WaitForHpLose(float hp, float maxHp, float duration)
    {
        var tw = _hpBar.DOFillAmount(hp / maxHp, duration);
        yield return tw.WaitForCompletion();
        yield return new WaitForSeconds(0.3f);
        _hpBarBg.gameObject.SetActive(false);
    }
}
