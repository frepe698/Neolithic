using UnityEngine;
using System.Collections;
using System;

public class SkillManager {

    private readonly Skill[] skills;

    private int abilityPoints;

    private Unit unit;

    public event EventHandler onSkillsUpdatedListener;
    public event LevelUpDisplayEventHandler onLevelUpDisplayListener;

    public delegate void LevelUpDisplayEventHandler(object sender, TextArgs e);
    public class TextArgs : EventArgs
    {
        public TextArgs(string skill) { this.text = text; }
        public string text;
    }

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

    private void onSkillsUpdated()
    {
        if (onSkillsUpdatedListener != null)
            onSkillsUpdatedListener(this, EventArgs.Empty);
    }

    private void levelUpDisplay(string text)
    {
        if (onLevelUpDisplayListener != null)
            onLevelUpDisplayListener(this, new TextArgs(text));
    }

    public void giveExperience(int skill, int experience)
    {
        if (skills[skill].grantExperience(experience))
        {
            levelUpDisplay(((Skills)skill).ToString() + " is now level " + skills[skill].getLevel());
        }
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

    public void grantAbilityPoint()
    {
        abilityPoints++;
        //onSkillsUpdated();
    }

    public void removeAbilityPoint()
    {
        abilityPoints--;
        onSkillsUpdated();
    }

    public int getAbilityPoints() { return abilityPoints; }

    public void increaseLevel()
    {
        unit.increaseSkillLevel();
        onSkillsUpdated();
    }

    public void learnAbility(string name)
    {
        GameMaster.getGameController().requestLearnAbility(name, unit.getID());
    }
    
}
