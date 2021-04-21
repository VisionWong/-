using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通AI搜寻最近的能攻击的敌人攻击;
/// 使用技能的优先级为伤害，治疗，变化
/// </summary>
public class CommonAIController : IAIController
{
    public CommonAIController(IChess chess, List<IChess> targetList, Map map) : base(chess, targetList, map)
    {
    }

    public override void SearchTarget()
    {
        Skill curSkill = null;
        int curWeight = 0;
        //根据拥有的技能搜索可以作用到的目标，并计算权重，决定使用哪个技能
        foreach (var skill in _chess.SkillList)
        {
            switch (skill.Data.skillType)
            {
                case SkillType.Damage:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高伤害
                    break;
                case SkillType.Heal:
                    //根据技能的范围搜索可以作用的目标，计算可以造成的最高治疗，最残血权重越高
                    break;
                case SkillType.Effect:
                    //根据技能的范围搜索可以作用的目标，等级上升固定权重，异常状态需要判断
                    break;
                default:
                    Debug.LogError("该类型尚未实现" + skill.Data.skillType);
                    break;
            }
        }//确定使用哪个技能
        //若没有能使用的技能，则选择接近目标
        //否则前往目标使用技能

    }
}
