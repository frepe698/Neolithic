using UnityEngine;
using System.Collections;

public abstract class DurationEffect : AbilityEffect {

    protected float duration;

    public DurationEffect(string name, Unit unit, Vector3 targetPosition, float duration)
        : base(name, unit, targetPosition)
    {
        this.duration = duration;
    }

    public override void action(AbilityCommand ability)
    {
 	    ability.addDurationEffect(this);
    }

    public void update(float speedIncrease)
    {
        updateEffect(speedIncrease);
        duration -= Time.deltaTime * speedIncrease;
    }

    protected abstract void updateEffect(float speedIncrease); 

    public bool done()
    {
        return duration <= 0;
    }
}
