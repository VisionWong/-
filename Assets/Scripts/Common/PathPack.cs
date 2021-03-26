using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPack
{
    public int ID { get; private set; }
    public string PrefabPath { get; private set; }
    public string SpritePath { get; private set; }

    public PathPack(int id, string prefab, string sprite)
    {
        ID = id;
        PrefabPath = prefab;
        SpritePath = sprite;
    }
}
