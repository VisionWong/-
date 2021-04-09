using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class ChessFactory
{
    public PlayerChess ProducePlayer(int id)
    {
        var data = ChessLib.Instance.GetData(id);

        GameObject go = ResourceMgr.Instance.Load<GameObject>(data.prefabPath);
        PlayerChess chess = new PlayerChess(new PlayerAttr(data), go);
        go.tag = TagDefine.ENEMY;

        PathPack pathPack = new PathPack(data.prefabPath, data.spritePath);
        chess.SetPathPack(pathPack);
        chess.SetAnimator(go.AddComponent<ChessAnimator>());
        chess.SetSelectableScript(go.AddComponent<SelectablePlayerChess>());

        //技能
        var skillData = SkillLib.Instance.GetData(data.skillId);
        Type type = Type.GetType(skillData.name);
        var skill = Activator.CreateInstance(type, skillData, chess, go.transform);
        chess.LearnSkill(skill as Skill);
        return chess;
    }


}
