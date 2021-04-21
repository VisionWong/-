using System;

public static class SkillFactory
{
    public static Skill Produce(int id)
    {
        var skillData = SkillLib.Instance.GetData(id);
        //Type type = Type.GetType(skillData.name);
        //var skill = Activator.CreateInstance(type, skillData);
        Skill skill = new Skill(skillData);
        return skill;
    }
}
