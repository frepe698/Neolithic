using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitStats {
	
	protected Unit unit;
	
	private const int MAX_LEVEL = 100;
	protected int level;
    private int skillLevel;
	private int exp;
	private readonly int skillsToLevel = 3;  //multiplied by level
	
	protected BaseStat[] stats;

	
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
                //Increased damage
				new BaseStat("Increased Damage", 1),
                new BaseStat("Melee Damage", 0),
                new BaseStat("Tree Damage", 0),
                new BaseStat("Stone Damage", 0),
                new BaseStat("Ranged Damage", 0),
                new BaseStat("Increased Magic Damage", 1),
                new BaseStat("Attackspeed", 0),

                //Damage conversion
                new BaseStat("TreeToAttack", 0),
                new BaseStat("StoneToAttack", 0),

                //Loot drop rate
                new BaseStat("TreeDropAmount", 1),
                new BaseStat("TreeDropRarity", 0),
                new BaseStat("StoneDropAmount", 1),
                new BaseStat("StoneDropRarity", 0),

				new BaseStat("Movespeed", (int)data.movespeed),
				
		};
		
		//updateStats();
		//setVitals();	
		
	}

	public virtual void updateStats()
    {
		for(int s = 0; s < stats.Length; s++){
			//set to base + per level
			stats[s].reset(level);
		}

        //get damage from equiped weapon
        if (unit.isMelee())
        {
            addToStat(Stat.MeleeDamage, unit.getBaseDamage(DamageType.COMBAT));
        }
        else
        {
            addToStat(Stat.RangedDamage, unit.getBaseDamage(DamageType.COMBAT));
        }
        addToStat(Stat.Attackspeed, unit.getBaseAttackSpeed());

        foreach (Buff buff in unit.getBuffs())
        {
            buff.applyStats(this);
        }

		//Multiply stats
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

    public void addToStat(Stat stat, float value)
    {
        addToStat((int)stat, value);
    }

    public void addToStat(int stat, float value)
    {
        stats[stat].addValue(value);
    }

    public void removeFromStat(Stat stat, float value)
    {
        removeFromStat((int)stat, value);
    }

    public void removeFromStat(int stat, float value)
    {
        stats[stat].addValue(-value);
    }

    public void addMultiplierToStat(Stat stat, float value)
    {
        addMultiplierToStat((int)stat, value);
    }

    public void addMultiplierToStat(int stat, float value)
    {
        stats[stat].addMultiplier(value);
    }


    public void increaseSkillLevel()
    {
        skillLevel++;
        while (level+1 < MAX_LEVEL && skillLevel >= getSkillsToLevel(level))
        {
            levelUp();
        }
        updateStats();
    }


    private void levelUp()
    {
        if (level > MAX_LEVEL) return;
        level++;
        getHealth().setCurValue(getHealth().getValue());
        getEnergy().setCurValue(getEnergy().getValue());
        Debug.Log("You are now level " + level + "!");
        unit.grantAbilityPoint();
        unit.onLevelUp();
    }

    public int getLevel() { return level; }
    public int getDisplayLevel() { return level+1; }
    public BaseStat getStat(int stat) { return stats[stat]; }
    public BaseStat getStat(Stat stat) { return stats[(int)stat]; }
    public float getStatV(int stat) { return stats[stat].getValue(); }
    public float getStatV(Stat stat) { return stats[(int)stat].getValue(); }
    public float getStatMV(int stat) { return stats[stat].getMultiValue(); }
    public float getStatMV(Stat stat) { return stats[(int)stat].getMultiValue(); }

    public Vital getHealth() { return (Vital)stats[(int)Stat.Health]; }
    public Vital getEnergy() { return (Vital)stats[(int)Stat.Energy]; }
    public BaseStat getMovespeed() { return (BaseStat)stats[(int)Stat.Movespeed]; }
    public BaseStat getArmor() { return (BaseStat)stats[(int)Stat.Armor]; }
    public BaseStat getHealthRegen() { return (BaseStat)stats[(int)Stat.HealthRegen]; }
    public BaseStat getEnergyRegen() { return (BaseStat)stats[(int)Stat.EnergyRegen]; }
    public float getMaxHealth() { return stats[(int)Stat.Health].getValue(); }
    public float getCurHealth() { return ((Vital)stats[(int)Stat.Health]).getCurValue(); }
    public float getMaxEnergy() { return stats[(int)Stat.Energy].getValue(); }
    public float getCurEnergy() { return ((Vital)stats[(int)Stat.Energy]).getCurValue(); }

    public float getDamage(int damageType, bool melee)
    {
        if (melee)
        {
            return getMeleeDamage(damageType);
        }
        else
        {
            return getRangedDamage(damageType);
        }
    }

    public float getMeleeDamage(int damageType)
    {
        switch (damageType)
        {
            case (DamageType.COMBAT):
                return getStatV(Stat.IncreasedDamage) * getStatV(Stat.MeleeDamage);
            case (DamageType.TREE):
                return getStatV(Stat.IncreasedDamage) * getStatV(Stat.TreeDamage);
            case (DamageType.STONE):
                return getStatV(Stat.IncreasedDamage) * getStatV(Stat.StoneDamage);
            default:
                return 0;
        }
    }

    public float getRangedDamage(int damageType)
    {
        if (damageType == (int)DamageType.COMBAT)
            return getStatV(Stat.IncreasedDamage) * getStatV(Stat.RangedDamage);

        return 0;
    }

    public BaseStat[] getAllStats(){ return stats; }
}
