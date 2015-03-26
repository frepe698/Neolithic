using UnityEngine;
using System.Collections;

public class Skill {

    public static readonly int Melee = 0;
    public static readonly int Mining = 1;
    public static readonly int Building = 2;
    public static readonly int Crafting = 3;
    public static readonly int DarkMagic = 4;
    public static readonly int LightMagic = 5;
    public static readonly int Alchemy = 6;
    public static readonly int Harvesting = 7;
    public static readonly int Hunting = 8;
    public static readonly int Ranged = 9;
    public static readonly int WoodChopping = 10;

    public const int MAXLEVEL = 20;
    public readonly SkillData data;
    private int experience;
    private int level;
    private int unlockedLevel;

    private SkillManager manager;

    public Skill(string name, SkillManager manager)
    {
        this.manager = manager;
        data = DataHolder.Instance.getSkillData(name);
        if (data == null) Debug.LogWarning("could not load data for the skill: " + name);
    }

    //Returns true if leveled up, false if not
    public bool grantExperience(int experience)
    {
        if (level > MAXLEVEL) return false;
        bool leveledup = false;
        this.experience += experience;
        Debug.Log(data.gameName + " experience is now " + this.experience);
        while (level+1 < MAXLEVEL && this.experience >= data.requiredExp[level])
        {
            level++;
            manager.increaseLevel();
            Debug.Log("Skill leveled up! " + data.gameName + " is now level " + level);
            leveledup = true;

        }
        return leveledup;

    }

    public int getLevel()
    {
        return this.level;
    }

    public int getDisplayLevel()
    {
        return this.level + 1;
    }

    public void unlockNext()
    {
        if (unlockedLevel >= data.abilities.Length) return;
        manager.learnAbility(data.abilities[unlockedLevel].name);
        unlockedLevel++;
        manager.removeAbilityPoint();
        //Apply changes to unitstats or smth
    }

    public void unlock(int level)
    {
        if (level == unlockedLevel && this.level >= data.abilities[level].reqLevel)
        {
            unlockNext();
        }
    }

    public StatChange getStatChange(int index)
    {
        return data.statsPerLevel[index];
    }

    public StatChange[] getStatsPerLevel()
    {
        if (data.statsPerLevel == null) Debug.LogWarning("error in " + data.name);
        return data.statsPerLevel;
    }

    public int getUnlockedLevel()
    {
        return unlockedLevel;
    }

    public bool canUnlock(int level)
    {
        return level == unlockedLevel && manager.hasAbilitypoint() && this.level >= data.abilities[level].reqLevel;
    }
	
}

public enum Skills
{
    Melee,
    Mining,
    Building,
    Crafting,
    DarkMagic,
    LightMagic,
    Alchemy,
    Harvesting,
    Hunting,
    Ranged,
    WoodChopping,
}
