using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class DataLibsManager : MonoSingleton<DataLibsManager>
{
    public void InitAllLibs()
    {
        ChessLib.Instance.ParseInit(PathDefine.CONFIG_CHESS_DATA);
        SkillLib.Instance.ParseInit(PathDefine.CONFIG_SKILL_DATA);
        PMTypeLib.Instance.ParseInit(PathDefine.CONFIG_PMTYPE_DATA);
    }
}
