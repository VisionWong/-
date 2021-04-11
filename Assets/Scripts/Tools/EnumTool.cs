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
}
