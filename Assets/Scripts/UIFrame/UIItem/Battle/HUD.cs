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

    public Color BURN_COLOR = new Color(209 / 255f, 48 / 255f, 0f);
    public Color POISON_COLOR = new Color(123 / 255f, 0f, 209 / 255f);
    public Color HEAL_COLOR = Color.green;
    public Color NO_EFFECT_COLOR = Color.gray;
    public Color EFFECTIVE_COLOR = Color.red;
    public Color HALF_EFFECTIVE_COLOR = Color.blue;
    public Color COMMON_DAMAGE_COLOR = Color.yellow;
    public Color ORIGIN_COLOR = Color.white;

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

    public void ChangeHPValueByDamage(float hp, float maxHp, int damage, DamageType type, Action callback = null, float duration = 1f)
    {
        HidePreview();
        switch (type)
        {
            case DamageType.NoEffect:
                NoticeNoEffective();
                break;
            case DamageType.HalfEffective:
                NoticeDamageNum(damage, HALF_EFFECTIVE_COLOR);
                break;
            case DamageType.Common:
                NoticeDamageNum(damage, COMMON_DAMAGE_COLOR);
                break;
            case DamageType.Effective:
                NoticeDamageNum(damage, EFFECTIVE_COLOR);
                break;
            case DamageType.Poison:
                NoticeDamageNum(damage, POISON_COLOR);
                break;
            case DamageType.Burn:
                NoticeDamageNum(damage, BURN_COLOR);
                break;
            default:
                Debug.LogError("该类型还未实现" + type.ToString());
                break;
        }
        StartCoroutine(WaitForHpLose(hp, maxHp, duration, callback));
    }
    public void ChangeHPValueByHealing(float hp, float maxHp, int healNum, Action callback = null, float duration = 1f)
    {
        HidePreview();
        NoticeHealNum(healNum);
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

    public void ShowDamagePreview(float hp, float maxHp, int damage, DamageType type)
    {
        //TODO 根据克制效果显示不同标志
        switch (type)
        {
            case DamageType.NoEffect:
                NoticeNoEffective(false);
                break;
            case DamageType.HalfEffective:
                NoticeDamageNum(damage, HALF_EFFECTIVE_COLOR, false);
                break;
            case DamageType.Common:
                NoticeDamageNum(damage, COMMON_DAMAGE_COLOR, false);
                break;
            case DamageType.Effective:
                NoticeDamageNum(damage, EFFECTIVE_COLOR, false);
                break;
            case DamageType.Poison:
                NoticeDamageNum(damage, POISON_COLOR, false);
                break;
            case DamageType.Burn:
                NoticeDamageNum(damage, BURN_COLOR, false);
                break;
            default:
                Debug.LogError("该类型还未实现" + type.ToString());
                break;
        }
        _lastBar1FillAmount = _hpBar1.fillAmount;
        _hpBar1.fillAmount = hp / maxHp;
        //TODO 若死亡，则出现死亡标志
    }
    public void ShowHealPreview(float hp, float maxHp, int healNum)
    {
        NoticeHealNum(healNum, false);
        _lastBar1FillAmount = _hpBar1.fillAmount;
        _hpBar1.fillAmount = hp / maxHp;
    }
    public void HidePreview()
    {
        HideNotice();
        _hpBar1.fillAmount = _lastBar1FillAmount;
    }

    private void NoticeDamageNum(int damage, Color color, bool isHide = true)
    {
        _txtMsg.enabled = true;
        _txtMsg.color = color;
        _txtMsg.text = "-" + damage.ToString();
        if (isHide) Invoke("HideNotice", 1f);
    }
    public void NoticeNoEffective(bool isHide = true)
    {
        _txtMsg.enabled = true;
        _txtMsg.color = NO_EFFECT_COLOR;
        _txtMsg.text = "没有效果";
        if (isHide) Invoke("HideNotice", 1f);
    }
    public void NoticeHealNum(int healNum, bool isHide = true)
    {
        _txtMsg.enabled = true;
        _txtMsg.color = HEAL_COLOR;
        _txtMsg.text = "+" + healNum.ToString();
        if (isHide) Invoke("HideNotice", 1f);
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
    public void NoticeWords(string str)
    {
        _txtMsg.enabled = true;
        _txtMsg.text = str;
        Invoke("HideNotice", 1.5f);
    }

    public void HideNotice()
    {
        _txtMsg.color = ORIGIN_COLOR;
        _txtMsg.enabled = false;
    }
}
