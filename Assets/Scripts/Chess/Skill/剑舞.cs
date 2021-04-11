using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class 剑舞 : Skill
{
    public 剑舞(SkillData data) : base(data)
    {
    }

    public override int GetPreview(IChess target)
    {
        return 0;
    }

    public override void SkillEffect(List<IChess> targets, Direction dir)
    {
        targets[0].Attribute.ChangeAttackLevel(Data.effects[0].effectLevel);
    }

    protected override IEnumerator PlayAnimation(Direction dir)
    {
        _anim.ChangeForward(dir);
        var tee = _chessTrans.DOPunchScale(new Vector3(1, 1, 1), 0.5f, 1, 0.5f);
        yield return tee.WaitForCompletion();
        MessageCenter.Instance.Broadcast(MessageType.OnChessActionEnd);
    }
}
