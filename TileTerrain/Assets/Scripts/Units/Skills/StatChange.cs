using UnityEngine;
using System.Collections;

public class StatChange  {

    public readonly int stat;
    public readonly float value;

    public void applyChange(UnitStats unitstats)
    {
        unitstats.addToStat(stat, value);
    }
}
