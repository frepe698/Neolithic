using UnityEngine;
using System.Collections;

public class SkillManager {

    private readonly Skill[] skills = new Skill[]{
        new Skill("melee"), 
        new Skill("mining"), 
        new Skill("building"),
        new Skill("crafting")};

    private Unit unit;

    public SkillManager(Unit unit)
    {
        this.unit = unit;
    }

    public void giveExperience(int skill, int experience)
    {
        if (skill >= skills.Length) return;
        skills[skill].grantExperience(experience);
    }

    public Skill[] getSkills()
    {
        return skills;
    }

    public Skill getSkill(int skill)
    {
        if(skill < skills.Length)
            return skills[skill];
        return null;
    }
    
}
