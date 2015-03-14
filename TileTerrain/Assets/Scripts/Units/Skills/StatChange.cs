using UnityEngine;
using System.Collections;
using Edit;
using System;

[Serializable]
public class StatChange  {

    public readonly Stat stat;
    public readonly float value;
    public readonly bool multiplier;

    public StatChange() { }

    public StatChange(PassiveStat data)
    {
        this.stat = data.stat;
        this.value = data.amount;
        this.multiplier = data.multiplier;
    }


    public void applyChange(UnitStats unitstats, int unitLevel)
    {
        if (multiplier)
        {
            unitstats.addMultiplierToStat(stat, value * unitLevel);
        }
        else
        {
            unitstats.addToStat(stat, value * unitLevel);
        }
    }
}
