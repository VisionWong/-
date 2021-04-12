using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumTool
{
    public static Vector3 DirToVector3(Direction dir)
    {
        Vector3 v3 = default;
        switch (dir)
        {
            case Direction.None:
                v3 = new Vector3(0, 0, 0);
                break;
            case Direction.Up:
                v3 = new Vector3(0, 1, 0);
                break;
            case Direction.Down:
                v3 = new Vector3(0, -1, 0);
                break;
            case Direction.Left:
                v3 = new Vector3(-1, 0, 0);
                break;
            case Direction.Right:
                v3 = new Vector3(1, 0, 0);
                break;
            default:
                break;
        }
        return v3;
    }

    public static Direction GetOppositeDir(Direction dir)
    {
        Direction ret = 0;
        switch (dir)
        {
            case Direction.None:
                ret = Direction.None;
                break;
            case Direction.Up:
                ret = Direction.Down;
                break;
            case Direction.Down:
                ret = Direction.Up;
                break;
            case Direction.Left:
                ret = Direction.Right;
                break;
            case Direction.Right:
                ret = Direction.Left;
                break;
            default:
                break;
        }
        return ret;
    }

    public static string GetPMTypeName(PMType type)
    {
        string str = null;
        switch (type)
        {
            case PMType.None:
                str = "无";
                break;
            case PMType.Grass:
                str = "草";
                break;
            case PMType.Fire:
                str = "火";
                break;
            case PMType.Water:
                str = "水";
                break;
            case PMType.Electric:
                str = "电";
                break;
            case PMType.Fight:
                str = "格斗";
                break;
            case PMType.Ground:
                str = "地面";
                break;
            case PMType.Rock:
                str = "岩石";
                break;
            case PMType.Metal:
                str = "钢";
                break;
            case PMType.Bug:
                str = "虫";
                break;
            case PMType.Poison:
                str = "毒";
                break;
            case PMType.Fly:
                str = "飞行";
                break;
            case PMType.Ghost:
                str = "幽灵";
                break;
            case PMType.Psychic:
                str = "超能";
                break;
            case PMType.Common:
                str = "一般";
                break;
            case PMType.Dragon:
                str = "龙";
                break;
            case PMType.Dark:
                str = "恶";
                break;
            case PMType.Fairy:
                str = "妖精";
                break;
            case PMType.Ice:
                str = "冰";
                break;
            default:
                Debug.LogError("尚不存在该类型" + type.ToString());
                break;
        }
        return str;
    }

    public static string GetSkillTypeName(SkillType type)
    {
        string str = null;
        switch (type)
        {
            case SkillType.Damage:
                str = "伤害";
                break;
            case SkillType.Heal:
                str = "治疗";
                break;
            case SkillType.Effect:
                str = "变化";
                break;
            default:
                Debug.LogError("尚不存在该类型" + type.ToString());
                break;
        }
        return str;
    }
}
