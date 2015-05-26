using UnityEngine;
using System.Collections;

public abstract class DurationEffect : AbilityEffect {

    protected float duration;

    public DurationEffect(string name, Actor actor, Vector3 targetPosition, float duration)
        : base(name, actor, targetPosition)
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
