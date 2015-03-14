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
	
	public UnitStats(Unit unit, int level, UnitData data){
		this.level = level;
		this.unit = unit;
        
		exp = (int)getSkillsToLevel(level);
		
		stats = new BaseStat[] {
				new Vital("Health", data.health, 20),
				new Vital("Energy", (int)data.energy, 0),
				new BaseStat("Armor", 0),
				new BaseStat("Health regen", (int)data.lifegen),
				new BaseStat("Energy regen", (int)data.energygen),
				new BaseStat("Increased Damage", 1),
				new BaseStat("Movespeed", (int)data.movespeed),
				
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
		if(stats[(int)Stat.Health] is Vital)((Vital) stats[(int)Stat.Health]).setCurValue(stats[(int)Stat.Health].getValue());
		if(stats[(int)Stat.Energy] is Vital)((Vital) stats[(int)Stat.Energy]).setCurValue(stats[(int)Stat.Energy].getValue());
	}

    public void addToStat(int stat, float value)
    {
        stats[stat].addValue(value);
    }

    public void removeFromStat(int stat, float value)
    {
        stats[stat].addValue(-value);
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
        getEnergy().setCurValue(getEnergy().getValue());
        updateStats();
        Debug.Log("You are now level " + level + "!");

        //unit.onLevelUp();
    }

    public BaseStat getStat(int stat) { return stats[stat]; }
    public BaseStat getStat(Stat stat) { return stats[(int)stat]; }
    public float getStatV(int stat) { return stats[stat].getValue(); }
    public float getStatMV(int stat) { return stats[stat].getMultiValue(); }

    public Vital getHealth() { return (Vital)stats[(int)Stat.Health]; }
    public Vital getEnergy() { return (Vital)stats[(int)Stat.Energy]; }
    public BaseStat getMovespeed() { return (BaseStat)stats[(int)Stat.Movespeed]; }
    public BaseStat getArmor() { return (BaseStat)stats[(int)Stat.Armor]; }
    public BaseStat getHealthRegen() { return (BaseStat)stats[(int)Stat.HealthRegen]; }
    public BaseStat getEnergyRegen() { return (BaseStat)stats[(int)Stat.EnergyRegen]; }
    public float getMaxHealth() { return stats[(int)Stat.Health].getValue(); }
    public float getCurHealth() { return ((Vital)stats[(int)Stat.Health]).getCurValue(); }
    public float getMaxEnergy() { return stats[(int)Stat.Energy].getValue(); }
    public float getCurMana() { return ((Vital)stats[(int)Stat.Energy]).getCurValue(); }
}
