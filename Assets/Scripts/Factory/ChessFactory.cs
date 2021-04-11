using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public static class ChessFactory
{
    public static PlayerChess ProducePlayer(int id)
    {
        var data = ChessLib.Instance.GetData(id);

        GameObject go = ResourceMgr.Instance.Load<GameObject>(data.prefabPath);
        PlayerChess chess = new PlayerChess(new PlayerAttr(data), go);
        go.tag = TagDefine.PLAYER;

        PathPack pathPack = new PathPack(data.prefabPath, data.spritePath);
        chess.SetPathPack(pathPack);
        chess.SetAnimator(go.AddComponent<ChessAnimator>());
        chess.SetSelectableScript(go.AddComponent<SelectablePlayerChess>());

        //技能
        chess.LearnSkill(SkillFactory.Produce(data.skillId));
        return chess;
    }


}
