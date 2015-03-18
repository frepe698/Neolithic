using UnityEngine;
using System.Collections;
using Edit;

public class Ability {

    private int energyCost;
    private int healthCost;

    private readonly float cooldown;
    private float curCooldown;
    
    private float totalTime;

    private AbilityEffectAndTime[] timedEffects; 

    public readonly AbilityData data;

    private Unit unit;

    public Ability(string name, Unit unit)
    {
        //Fetch data
        //save data
        //cooldown = data.cooldown
        data = DataHolder.Instance.getAbilityData(name);
        cooldown = data.cooldown;
        totalTime = data.totalTime;
        energyCost = data.energyCost;
        healthCost = data.healthCost;
        timedEffects = data.effects;
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
        return cooldown;
    }

    public bool isCool()
    {
        return curCooldown <= 0;
    }

    public float getTotalTime()
    {
        return totalTime;
    }

    public int getEnergyCost()
    {
        return energyCost;
    }

    public int getHealthCost()
    {
        return healthCost;
    }

    public AbilityEffectAndTime getEffect(int effect)
    {
        //AbilityEffect abilityEffect = DataHolder.Instance.getEffectData(timedEffects[effect].name).getAbilityEffect(unit, targetPosition);
        return timedEffects[effect];
    }


    public AbilityEffectAndTime getTimedEffect(int effect, float time)
    {
        if (timedEffects[effect].time <= time) return timedEffects[effect];
        return null;
    }
}
