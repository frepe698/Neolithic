using UnityEngine;
using System.Collections;

public abstract class Buff {
    public static readonly int STAT_PARAM = 0;
    public static readonly int DURATION_PARAM = 1;
    public static readonly int AMOUNT_PARAM = 2;
    public static readonly int PERCENT_PARAM = 3;

    public abstract void apply(Unit unit);
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
    private Stat stat;
    private float duration;
    private float amount;
    private bool percent;
    private Unit unit;

    private bool finished = false;
    public StatBuff(object[] parameters)
    {
        
        this.stat = (Stat)parameters[STAT_PARAM];
        this.duration = (float)parameters[DURATION_PARAM];
        this.amount = (float)parameters[AMOUNT_PARAM];
        this.percent = (bool)parameters[PERCENT_PARAM];
    }

    public override void apply(Unit unit)
    {
        this.unit = unit;
        unit.addBuff(this);
    }


    public void applyStats(Unit unit)
    {
        if (percent)
        {
            unit.addMultiplierToStat(stat, amount);
        }
        else
        {
            unit.addToStat(stat, amount);
        }
    }
    public override void remove()
    {
        /*
        if (percent)
        {
            unit.addMultiplierToStat(stat, -amount);
        }
        else
        {
            unit.addToStat(stat, -amount);
        }*/
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

public class Stun : Buff
{
    private float duration;
    private Unit unit;

    private bool finished = false;
    public Stun(object[] parameters)
    {
        this.duration = (float)parameters[DURATION_PARAM];
    }

    public override void apply(Unit unit)
    {
        this.unit = unit;
        unit.giveCommand(new StunnedCommand(unit, duration));
        unit.addBuff(this);
    }

    public override void remove()
    {
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

public class BuffGetter
{

    
    public static Buff getStatBuff(object[] parameters)
    {
        return new StatBuff(parameters);
    }

    public static Buff getStun(params object[] parameters)
    {
        return new Stun(parameters);
    }
}
