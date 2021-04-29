using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class HUD : MonoBehaviour
{
    //public Color PLAYER_COLOR = new Color(34 / 255f, 209 / 255f, 0f);
    public Color PLAYER_COLOR = new Color(0f, 176 / 255f, 209 / 255f);
    public Color ENEMY_COLOR = new Color(255 / 255f, 198 / 255f, 0f);

    private Transform _hpBarBg;
    private Image _hpBar1;//血条上层
    private Image _hpBar2;//血条下层慢放
    private float _lastBar1FillAmount;

    private Text _txtMsg;

    private void Awake()
    {

        var canvas = transform.Find("Canvas");
        _hpBarBg = canvas.Find("hpbg");
        _hpBar1 = _hpBarBg.Find("hpBar1").GetComponent<Image>();
        _hpBar2 = _hpBarBg.Find("hpBar2").GetComponent<Image>();
        _lastBar1FillAmount = _hpBar1.fillAmount;

        _txtMsg = canvas.Find("txtMsg").GetComponent<Text>();
        _txtMsg.enabled = false;
    }
    
    public void SetHPBarColor(string tag)
    {
        if (tag == TagDefine.PLAYER)
            _hpBar1.color = PLAYER_COLOR;
        else
            _hpBar1.color = ENEMY_COLOR;
    }

    public void ChangeHPValue(float hp, float maxHp, Action callback = null, float duration = 1f)
    {
        HidePreview();
        StartCoroutine(WaitForHpLose(hp, maxHp, duration, callback));
    }

    private IEnumerator WaitForHpLose(float hp, float maxHp, float duration, Action callback = null)
    {
        var endValue = hp / maxHp;
        _hpBar1.DOFillAmount(endValue, duration);
        var tw = _hpBar2.DOFillAmount(endValue, duration * 2f);
        _lastBar1FillAmount = endValue;
        yield return tw.WaitForCompletion();
        callback?.Invoke();
    }

    public void ShowPreview(float hp, float maxHp)
    {
        //TODO 根据克制效果显示不同标志
        _lastBar1FillAmount = _hpBar1.fillAmount;
        _hpBar1.fillAmount = hp / maxHp;
        //TODO 若死亡，则出现死亡标志
    }
    public void HidePreview()
    {
        _hpBar1.fillAmount = _lastBar1FillAmount;
    }

    public void NoticeAvoid()
    {
        _txtMsg.enabled = true;
        _txtMsg.text = "闪避";
        Invoke("HideNotice", 1f);
    }
    public void NoticeUseSkill(string name)
    {
        _txtMsg.enabled = true;
        _txtMsg.text = " " + name + "!";
        Invoke("HideNotice", 1.5f);
    }

    public void HideNotice()
    {
        _txtMsg.enabled = false;
    }
}
