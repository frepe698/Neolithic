using UnityEngine;
using System.Collections;

public class MovementEffect : AbilityEffect {

    MovementEffectData data;

    public MovementEffect(string name, Unit unit, Vector3 targetPosition, MovementEffectData data)
        : base(name, unit, targetPosition)
    {
        this.data = data;
    }

    public override void action()
    {
        unit.setPosition(targetPosition);
    }

}
