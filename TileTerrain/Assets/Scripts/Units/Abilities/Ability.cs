using UnityEngine;
using System.Collections;

public class Ability {

    private int energyCost;
    private int healthCost;

    private float curCooldown;
    private float totalTime;

    public readonly AbilityData data;

    private Unit unit;

    public Ability(string name, Unit unit)
    {
        //Fetch data
        //save data
        //cooldown = data.cooldown
        data = DataHolder.Instance.getAbilityData(name);
        curCooldown = data.cooldown;
        totalTime = data.totalTime;
        energyCost = data.energyCost;
        healthCost = data.healthCost;
        this.unit = unit;
    }

    

    public virtual void start()
    { 
        
    }

    public void update()
    {
        curCooldown -= Time.deltaTime;
    }

    public float getCoolDown()
    {
        return curCooldown;
    }

    public bool isCool()
    {
        return curCooldown <= 0;
    }
}
