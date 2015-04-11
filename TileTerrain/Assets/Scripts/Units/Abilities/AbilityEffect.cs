using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public abstract class AbilityEffect {

    protected Unit unit;
    protected Vector3 targetPosition;
    protected float time;

    public AbilityEffect(string name, Unit unit, Vector3 targetPosition)
    {
        //Fetch data with name
        this.unit = unit;
        this.targetPosition = targetPosition;
    }

    public abstract void action(AbilityCommand ability);
    #if false
    protected void applyBuffs(HitBuff[] buffs, Unit target)
    {


        for (int i = 0; i < buffs.Length; i++)
        {
            HitBuff hbuff = buffs[i];
            object[] parameters = new object[] { hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent };
            string name = "get" + hbuff.type.ToString();
            GameMaster.getGameController().requestAddBuff(target.getID(), name, parameters);
        }
        

    }
#endif

    public static void applyDamage(HitDamage[] hitDamage, Unit unit, Unit target, int expSkill)
    {
        int selfDamage = 0;
        int damage = 0;
        foreach (HitDamage hit in hitDamage)
        {
            int tempDamage;
            if (hit.yourStat)
            {
                tempDamage = (int)(unit.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            else
            {
                tempDamage = (int)(target.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            if (hit.damageSelf) selfDamage += tempDamage;
            else damage += tempDamage;
        }
        if (damage != 0)
        {
            //Debug.Log("Take Damage " + damage);
            GameMaster.getGameController().requestHit(damage, unit.getID(), target.getID(), expSkill);
        }

        //Should you get xp for hitting yourself?
        if (selfDamage != 0) GameMaster.getGameController().requestHit(selfDamage, unit.getID(), unit.getID());
    }

    public static void applyBuffs(string dataName, Unit unit, Unit target)
    {
        AbilityEffectData data = DataHolder.Instance.getEffectData(dataName);
        if (data != null)
        {
            HitBuff[] buffs = data.hitBuffs;
            for (int i = 0; i < buffs.Length; i++)
            {
                HitBuff hbuff = buffs[i];
                object[] parameters = new object[] { hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent };
                string name = "get" + hbuff.type.ToString();

                MethodInfo info = typeof(BuffGetter).GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                Buff buff = (Buff)info.Invoke(null, new object[] { parameters });
                buff.apply(target);
            }
        }
    }

    public static void modelAndSound(AbilityEffectData data, Unit unit, Vector3 targetPosition, bool followUnit = false)
    {

        //AbilityEffectData data = DataHolder.Instance.getEffectData(dataName);

        if(data.weaponSound)
        {
            unit.playSound(unit.getAttackSound(0));
        }
        else if(data.modelIsSound)
        {
            unit.playSound(data.modelName);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("Effects/" + data.modelName);
            if (prefab != null)
            {
                if(followUnit) 
                    unit.addEffectObject(prefab, targetPosition);
                else
                    GameObject.Instantiate(prefab, targetPosition - Vector3.up, Quaternion.identity);
            }
        }
        
    }

}

public enum AbilityEffects
{
    SingleTarget,
    Area,

}