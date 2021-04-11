using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class 治疗 : Skill
{
    public 治疗(SkillData data) : base(data)
    {
    }

    public override void SkillEffect(List<IChess> targets, Direction dir)
    {
        targets[0].Healing(Data.effects[0].fixedPercent);
    }

    protected override IEnumerator PlayAnimation(Direction dir)
    {
        _anim.ChangeForward(dir);
        var tee = _chessTrans.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1, 0.5f);
        yield return tee.WaitForCompletion();
        MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd);
    }

    public override int GetPreview(IChess target)
    {
        return (int)(target.Attribute.MaxHP * Data.effects[0].fixedPercent);
    }
}
