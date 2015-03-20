using UnityEngine;
using System.Collections;

public abstract class Buff {
    public abstract void apply();
    public abstract void remove();
    public abstract void update();
    public abstract bool isFinished();
}

public enum BuffType
{
    StatBuff,
    Stun,
}

public class StatBuff : Buff
{
    public static readonly int STAT_PARAM = 0;
    public static readonly int DURATION_PARAM = 1;
    public static readonly int AMOUNT_PARAM = 2;
    public static readonly int PERCENT_PARAM = 3;

    private Stat stat;
    private float duration;
    private float amount;
    private bool percent;
    private Unit unit;

    private bool finished = false;
    public StatBuff(Unit unit, params object[] parameters)
    {
        this.unit = unit;
        this.stat = (Stat)parameters[STAT_PARAM];
        this.duration = (float)parameters[DURATION_PARAM];
        this.amount = (float)parameters[AMOUNT_PARAM];
        this.percent = (bool)parameters[PERCENT_PARAM];
    }

    public override void apply()
    {
        if(percent)
        {
            unit.addMultiplierToStat(stat, amount);
        }
        else
        {
            unit.addToStat(stat, amount);
        }
        unit.addBuff(this);
    }

    public override void remove()
    {
        if (percent)
        {
            unit.addMultiplierToStat(stat, -amount);
        }
        else
        {
            unit.addToStat(stat, -amount);
        }
        unit.removeBuff(this);
    }

    public override void update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) finished = true;
    }

    public override bool isFinished()
    {
        return finished;
    }
}

public static class BuffGetter
{
    public static Buff getStatBuff(Unit unit, params object[] parameters)
    {
	    return new StatBuff(unit, parameters);
    }

    public static Buff getStun(Unit unit, params object[] parameters)
    {
        return null;
    }
}
