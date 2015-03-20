using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class AbilityEffect {

    protected Unit unit;
    protected Vector3 targetPosition;
    protected float time;

    public AbilityEffect(string name, Unit unit, Vector3 targetPosition)
    {
        //Fetch data with name
        this.unit = unit;
        this.targetPosition = targetPosition;
    }

    public virtual void action()
    {
        //DO the shiet
    }

    protected void applyBuffs(HitBuff[] buffs, Unit target)
    {
 
        for(int i = 0; i < buffs.Length; i++)
        {
            HitBuff hbuff = buffs[i];
            object[] parameters = new object[]{hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent};
            string name = "get" + hbuff.type.ToString();
            GameMaster.getGameController().requestAddBuff(target.getID(), name, parameters);
        }
    }

}

public enum AbilityEffects
{
    SingleTarget,
    Area,

}