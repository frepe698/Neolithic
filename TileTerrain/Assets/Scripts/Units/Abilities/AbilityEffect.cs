using UnityEngine;
using System.Collections;

public class AbilityEffect {

    protected Unit unit;
    protected Vector2 targetPosition;
    protected float time;

    public AbilityEffect(string name, Unit unit, Vector2 targetPosition)
    {
        //Fetch data with name
        this.unit = unit;
        this.targetPosition = targetPosition;
    }

    public virtual void action()
    {
        //DO the shiet
    }
}

public enum AbilityEffects
{
    SingleTarget,
    Area,

}