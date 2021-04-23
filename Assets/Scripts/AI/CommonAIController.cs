using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通AI搜寻能造成最大伤害的敌人攻击;
/// 使用技能的优先级为伤害，治疗，变化
/// </summary>
public class CommonAIController : IAIController
{
    private HashSet<Skill> _buffSkillSet;
    private Dictionary<Skill, int> _buffToTimesDict;//增益技能限制使用3次

    public CommonAIController(IChess chess, List<IChess> targetList) : base(chess, targetList)
    {
        _buffSkillSet = new HashSet<Skill>();
        _buffToTimesDict = new Dictionary<Skill, int>();
    }

    public override void SearchTarget()
    {
        Skill skillToUse = null;
        int curWeight = 0;
        //根据拥有的技能搜索可以作用到的目标，并计算权重，决定使用哪个技能
        foreach (var skill in _chess.SkillList)
        {
            switch (skill.Data.skillType)
            {
                case SkillType.Damage:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高伤害，权重为1
                    var targets = BattleSystem.Instance.GetAttackableTargetsByBFS(_chess, skill, TagDefine.PLAYER);
                    if (targets == null) break;
                    int maxDamage = int.MinValue;
                    foreach (var target in targets)
                    {
                        int damage = Formulas.CalSkillDamage(skill.Data, _chess, target);
                        if (damage > maxDamage)
                        {
                            maxDamage = damage;
                            _target = target;
                        }
                    }
                    if (maxDamage > curWeight)
                    {
                        curWeight = maxDamage;
                        skillToUse = skill;
                    }
                    break;
                case SkillType.Heal:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高治疗，越残血权重越高
                    var healTargets = BattleSystem.Instance.GetAttackableTargetsByBFS(_chess, skill, TagDefine.ENEMY);
                    int maxHeal = int.MinValue;
                    if (healTargets == null) break;
                    foreach (var target in healTargets)
                    {
                        int heal = (int)(Formulas.CalHealingNum(skill.Data, _chess, target) * Formulas.GetHealingWeight(target));
                        if (heal > maxHeal)
                        {
                            maxHeal = heal;
                            _target = target;
                        }
                    }
                    if (maxHeal > curWeight)
                    {
                        curWeight = maxHeal;
                        skillToUse = skill;
                    }
                    break;
                case SkillType.Effect:
                    //自身作用的buff技能先加入后备列表，若无法攻击敌人再使用
                    if (skill.Data.rangeType == SkillRangeType.自身)
                    {
                        if (!_buffSkillSet.Contains(skill))
                        {
                            _buffSkillSet.Add(skill);
                            _buffToTimesDict.Add(skill, 0);
                        }                       
                    }
                    else
                    {
                        //TODO Buff技能同样限定次数
                        //Debuff技能与伤害技能一样，固定权重(暂定为自身血量一半
                        var effectTargets = BattleSystem.Instance.GetAttackableTargetsByBFS(_chess, skill, TagDefine.PLAYER);
                        var effectType = skill.Data.effects[0].effectType;
                        foreach (var target in effectTargets)
                        {
                            //TODO 没有该状态的优先
                            
                        }
                        int effectWeight = _chess.Attribute.MaxHP / 2;
                        if ( effectWeight > curWeight)
                        {
                            curWeight = effectWeight;
                            skillToUse = skill;
                        }
                    }
                    break;
                default:
                    Debug.LogError("该类型尚未实现" + skill.Data.skillType);
                    break;
            }
            if (skillToUse != null)
            {
                Debug.Log(string.Format("当前计算的技能为{0},其权重为{1}", skill.Data.name, curWeight));
            }
        }//确定使用哪个技能
        //若没有能使用的技能，则选择使用增益技能或者靠近目标
        if (skillToUse == null)
        {
            
        }
        //否则前往目标使用技能

    }
}
