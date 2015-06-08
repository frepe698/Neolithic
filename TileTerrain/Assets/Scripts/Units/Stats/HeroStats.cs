using UnityEngine;
using System.Collections;
using System;

public class HeroStats : UnitStats {

    public event EventHandler onStatsUpdatedListener;

    private Hero hero;

    public HeroStats(Hero unit, int level, UnitData data)
        : base(unit, level, data)
    {
        this.hero = unit;
    }

    public override void updateStats()
    {
        for (int s = 0; s < stats.Length; s++)
        {
            //set to base + per level
            stats[s].reset(level);
        }
        hero.setWarmth(0);

        //get stats from unit skills
        Skill[] skills = hero.getSkillManager().getSkills();
        foreach (Skill skill in skills)
        {
            StatChange[] statChanges = skill.getStatsPerLevel();
            if (statChanges == null) continue;
            foreach (StatChange stat in statChanges)
            {
                stat.applyChange(this, skill.getLevel());
            }
        }

        //get stats from equiped armor
        ArmorData[] armor = hero.getEquipedArmor();
        foreach (ArmorData a in armor)
        {
            if (a == null) continue;
            addToStat(Stat.Armor, a.armor);
            addToStat(Stat.Movespeed, -a.speedPenalty);
            hero.addWarmth(a.warmth);
        }

        //get damage from equiped weapon
        if (actor.isMelee())
        {
            addToStat(Stat.MeleeDamage, actor.getBaseDamage(DamageType.COMBAT));
            addToStat(Stat.TreeDamage, actor.getBaseDamage(DamageType.TREE));
            addToStat(Stat.StoneDamage, actor.getBaseDamage(DamageType.STONE));
        }
        else
        {
            addToStat(Stat.RangedDamage, actor.getBaseDamage(DamageType.COMBAT));
        }
        addToStat(Stat.Attackspeed, actor.getBaseAttackSpeed());

        foreach (Buff buff in actor.getBuffs())
        {
            StatBuff statBuff = buff as StatBuff;
            if (statBuff == null)
            {

                continue;
            }

            statBuff.applyStats(this);
        }

        //Multiply stats
        for (int s = 0; s < stats.Length; s++)
        {
            stats[s].multiply();
        }

        //Calculate damage conversions 
        float treeToAttack = getStatV(Stat.TreeToAttack);
        if (treeToAttack > 0)
            addToStat(Stat.MeleeDamage, treeToAttack * getStatV(Stat.TreeDamage));
        float stoneToAttack = getStatV(Stat.StoneToAttack);
        if (stoneToAttack > 0)
            addToStat(Stat.MeleeDamage, stoneToAttack * getStatV(Stat.StoneDamage));


        onStatsUpdated();
    }

    private void onStatsUpdated()
    {
        if (onStatsUpdatedListener != null)
            onStatsUpdatedListener(this, EventArgs.Empty);
    }
}
