using UnityEngine;
using System.Collections;
using Edit;

public class Ability {

    private float curCooldown;
    
    public readonly AbilityData data;

    private Unit unit;

    public Ability(string name, Unit unit)
    {
        //Fetch data
        //save data
        //cooldown = data.cooldown
        data = DataHolder.Instance.getAbilityData(name);
        this.unit = unit;
    }

    

    public virtual void start()
    {

    }

    public void update()
    {
        if(curCooldown > 0) curCooldown -= Time.deltaTime;
    }

    public float getCurCoolDown()
    {
        return curCooldown;
    }

    public float getCoolDown()
    {
        return data.cooldown;
    }

    public bool isCool()
    {
        return curCooldown <= 0;
    }

    public void setCooldown()
    {
        curCooldown = data.cooldown;
    }

    public float getTotalTime()
    {
        return data.totalTime;
    }

    public int getEnergyCost()
    {
        return data.energyCost;
    }

    public int getHealthCost()
    {
        return data.healthCost;
    }

    public AbilityEffectAndTime getEffect(int effect)
    {
        //AbilityEffect abilityEffect = DataHolder.Instance.getEffectData(timedEffects[effect].name).getAbilityEffect(unit, targetPosition);
        return data.effects[effect];
    }


    public AbilityEffectAndTime getTimedEffect(int effect, float time)
    {
        if (data.effects[effect].time <= time) return data.effects[effect];
        return null;
    }

    public AbilityAnimation getTimedAnimation(int nextAnimation, float time)
    {
        if (data.animations[nextAnimation].time <= time) return data.animations[nextAnimation];
        return null;
    }
}
