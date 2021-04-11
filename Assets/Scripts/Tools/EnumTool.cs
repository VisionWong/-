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
}
