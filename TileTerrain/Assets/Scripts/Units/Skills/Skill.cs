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

    public readonly string name;
    public readonly string gameName;
    public const int MAXLEVEL = 20;
    private readonly int[] requiredExp;
    private readonly StatChange[] statChanges;
    private int experience;
    private int level;
    private int unlockedLevel;

    private SkillManager manager;

    public Skill(string name, SkillManager manager)
    {
        this.name = name;
        this.manager = manager;
        SkillData data = DataHolder.Instance.getSkillData(name);
        if (data == null) return;
        this.requiredExp = data.requiredExp;
        this.gameName = data.gameName;
    }

    //Returns true if leveled up, false if not
    public bool grantExperience(int experience)
    {
        if (level >= MAXLEVEL) return false;
        this.experience += experience;
        Debug.Log(gameName + " experience is now " + this.experience);
        if(this.experience >= requiredExp[level])
        {
            level++;
            manager.increaseLevel();
            Debug.Log("Skill leveled up! " + gameName + " is now level " + level);
            return true;
        }
        return false;

    }

    public int getLevel()
    {
        return this.level;
    }

    public void unlockNext()
    {
        if (unlockedLevel >= MAXLEVEL) return;
        unlockedLevel++;
        //Apply changes to unitstats or smth
    }

    public StatChange getStatChange(int level)
    {
        return statChanges[level];
    }

    public StatChange[] getStatChanges()
    {
        return statChanges;
    }

    public int getUnlockedLevel()
    {
        return unlockedLevel;
    }
	
}
