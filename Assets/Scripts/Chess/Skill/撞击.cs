using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 撞击 : Skill
{
    public 撞击(SkillData data, IChess chess, Transform chessTrans) : base(data, chess, chessTrans)
    {
    }

    public override void UseSkill(List<IChess> targets)
    {
        targets[0].TakeDamage(Formulas.CalSkillDamage(Data, _chess));
        PlayAnimation();
    }
}
