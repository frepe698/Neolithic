using UnityEngine;
using System.Collections;

public class Skill {

    public readonly string name;

    public readonly int MAXLEVEL;
    private readonly int[] requiredExp;
    private int experience;
    private int level;
    private int unlockedLevel;

    public Skill(string name, int maxLevel)
    {
        this.name = name;
        this.MAXLEVEL = maxLevel;
        SkillData data = DataHolder.Instance.getSkillData(name);
        this.requiredExp = data.requiredExp;
    }

    public void grantExperience(int experience)
    {
        if (level >= MAXLEVEL) return;
        this.experience += experience;

    }

	
}
