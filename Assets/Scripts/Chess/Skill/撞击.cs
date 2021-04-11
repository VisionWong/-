using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 撞击 : Skill
{
    public 撞击(SkillData data, IChess chess, Transform chessTrans) : base(data, chess, chessTrans)
    {
    }

    public override void UseSkill(List<IChess> targets, Direction dir)
    {
        base.UseSkill(targets, dir);
        foreach (var target in targets)
        {
            target.TakeDamage(Formulas.CalSkillDamage(Data, _chess, target), dir);
        }
    }

    public override int GetPreview(IChess target)
    {
        return Formulas.CalSkillDamage(Data, _chess, target);
    }
}
