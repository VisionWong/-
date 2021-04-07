using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPack
{
    public string PrefabPath { get; private set; }
    public string SpritePath { get; private set; }

    public PathPack(string prefab, string sprite)
    {
        PrefabPath = prefab;
        SpritePath = sprite;
    }
}
