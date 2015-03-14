using UnityEngine;
using System.Collections;

public class SkillManager {

    private readonly Skill[] skills;

    private Unit unit;

    public SkillManager(Unit unit)
    {
        this.unit = unit;
        skills = new Skill[]{
        new Skill("melee", this), 
        new Skill("mining", this), 
        new Skill("building", this),
        new Skill("crafting", this),
        new Skill("darkmagic", this),
        new Skill("lightmagic", this),
        new Skill("alchemy", this),
        new Skill("harvesting", this),
        new Skill("hunting", this),
        new Skill("ranged", this),
        new Skill("woodchopping", this)};
    }

    private int totalLevels = 0;

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

    public void increaseLevel()
    {
        unit.increaseSkillLevel();
    }
    
}
