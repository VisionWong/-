using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFramework;

public class DataLibsManager : MonoSingleton<DataLibsManager>
{
    public void InitAllLibs()
    {
        SkillLib.Instance.ParseInit(PathDefine.CONFIG_SKILL_DATA);
        
    }
}
