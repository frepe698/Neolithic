using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitStats {
	
	private Unit unit;
	
	private readonly int maxLevel = 100;
	private int level;
    private int skillLevel;
	private int exp;
	private readonly int skillsToLevel = 3;  //multiplied by level
	
	private BaseStat[] stats;
	
	public UnitStats(Unit unit, int level, int baseHealth, int levelHealth,
					int baseMana, int levelMana, int healthRegen, int manaRegen){
		this.level = level;
		this.unit = unit;
		exp = (int)getSkillsToLevel(level);
		
		stats = new BaseStat[] {
				new Vital("Health", baseHealth, levelHealth),
				new Vital("Energy", baseMana, levelMana),
				new DefStat("Armor", 0.8f),
				new BaseStat("Health regen", healthRegen),
				new BaseStat("Energy regen", manaRegen),
				new BaseStat("Increased Damage", 1),
				new BaseStat("Movespeed", 1),
				
		};
		
		updateStats();
		setVitals();	
		
	}

	public void updateStats(){
		for(int s = 0; s < stats.Length; s++){
			//set to base + per level
			stats[s].reset(level);
		}
		
		//get stats from unit mods
        
		Skill[] skills = unit.getSkillManager().getSkills();
        foreach(Skill skill in skills)
        {
            StatChange[] statChanges = skill.getStatChanges();
            for(int i = 0; i < skill.getUnlockedLevel(); i++)
            {
                StatChange stat = statChanges[i];
                stat.applyChange(this);
            }
        }
		
		
		for(int s = 0; s < stats.Length; s++){
			stats[s].multiply();
		}
	}

    private int getSkillsToLevel(int level)
    {
        return (level + 1) * skillsToLevel;
    }

    private void setVitals() {
		if(stats[Stat.Health] is Vital)((Vital) stats[Stat.Health]).setCurValue(stats[Stat.Health].getValue());
		if(stats[Stat.Energy] is Vital)((Vital) stats[Stat.Energy]).setCurValue(stats[Stat.Energy].getValue());
	}

    public void addToStat(int stat, float value)
    {
        stats[stat].addValue(value);
    }

    public void increaseSkillLevel()
    {
        skillLevel++;
        if(skillLevel >= getSkillsToLevel(level))
        {
            levelUp();
        }
    }


    private void levelUp()
    {
        level++;
        getHealth().setCurValue(getHealth().getValue());
        getMana().setCurValue(getMana().getValue());
        updateStats();
        Debug.Log("You are now level " + level + "!");

        //unit.onLevelUp();
    }

    public BaseStat getStat(int stat) { return stats[stat]; }
    public float getStatV(int stat) { return stats[stat].getValue(); }
    public float getStatMV(int stat) { return stats[stat].getMultiValue(); }

    public Vital getHealth() { return ((Vital)stats[Stat.Health]); }
    public Vital getMana() { return ((Vital)stats[Stat.Energy]); }

    public float getMaxHealth() { return stats[Stat.Health].getValue(); }
    public float getCurHealth() { return ((Vital)stats[Stat.Health]).getCurValue(); }
    public float getMaxMana() { return stats[Stat.Energy].getValue(); }
    public float getCurMana() { return ((Vital)stats[Stat.Energy]).getCurValue(); }
}
