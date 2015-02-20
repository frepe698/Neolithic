﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class AIUnitData : UnitData {

    public readonly int damage;
    public readonly float attackSpeed = 1;

    [XmlElement(IsNullable = false)]
    public readonly string attackSound;
    public readonly bool hostile;
    public readonly int lineofsight;

    [XmlArray("safeDrops")]
    public readonly string[] safeDrops;

    [XmlArray("randomDrops")]
    public readonly string[] randomDrops;

    public readonly int minDrops;
    public readonly int maxDrops;

    public AIUnitData()
    { 
    }

    public AIUnitData(AIUnitEdit data)
        :  base(data)
    {
        damage = data.damage;
        attackSpeed = data.attackSpeed;
        if (data.attackSound != null && !data.attackSound.Trim().Equals("")) attackSound = data.attackSound;
        hostile = data.hostile;
        lineofsight = data.lineofsight;

        safeDrops = data.safeDrops.Trim().Split('\n');
        randomDrops = data.randomDrops.Trim().Split('\n');

        minDrops = data.minDrops;
        maxDrops = data.maxDrops;
    }

    public override string[] getSafeDrops()
    {
        return safeDrops;
    }
    public override string[] getRandomDrops()
    {
        return randomDrops;
    }
    public override int getMinDrops()
    {
        return minDrops;
    }
    public override int getMaxDrops()
    {
        return maxDrops;
    }
}