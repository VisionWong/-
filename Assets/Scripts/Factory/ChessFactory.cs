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
        go.tag = TagDefine.PLAYER;
        PlayerChess chess = new PlayerChess(new PlayerAttr(data), go);

        PathPack pathPack = new PathPack(data.prefabPath, data.spritePath);
        chess.SetPathPack(pathPack);
        chess.SetAnimator(go.AddComponent<ChessAnimator>());
        chess.SetSelectableScript(go.AddComponent<SelectablePlayerChess>());

        //技能
        foreach (var skillId in data.skillIdList)
        {
            chess.LearnSkill(SkillFactory.Produce(skillId));
        }
        return chess;
    }

    public static EnemyChess ProduceEnemy(int id, IAIController ctrl)
    {
        var data = ChessLib.Instance.GetData(id);

        GameObject go = ResourceMgr.Instance.Load<GameObject>(data.prefabPath);
        EnemyChess chess = new EnemyChess(new ChessAttr(data), go);
        go.tag = TagDefine.ENEMY;

        PathPack pathPack = new PathPack(data.prefabPath, data.spritePath);
        chess.SetPathPack(pathPack);
        chess.SetAnimator(go.AddComponent<ChessAnimator>());
        chess.SetSelectableScript(go.AddComponent<SelectableEnemyChess>());
        //AI
        chess.AI = ctrl;
        chess.AI.SetChess(chess);

        //技能
        foreach (var skillId in data.skillIdList)
        {
            chess.LearnSkill(SkillFactory.Produce(skillId));
        }
        return chess;
    }
}
