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
            object[] parameters = new object[]{target, hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent};
            Debug.Log(hbuff.type.ToString());
            MethodInfo info = typeof(BuffGetter).GetMethod("get" + hbuff.type.ToString(), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            Buff buff = (Buff)info.Invoke(this, new object[]{ parameters });
            buff.apply();
        }
    }

    
}

public enum AbilityEffects
{
    SingleTarget,
    Area,

}