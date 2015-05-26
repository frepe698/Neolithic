using UnityEngine;
using System.Collections;
using Edit;

public class MovementEffectData : AbilityEffectData {

    public readonly float range;
    public readonly float travelTime;

    public MovementEffectData() : base() { }

    public MovementEffectData(MovementEffectEdit edit)
        : base(edit)
    {
        range = edit.range;
        travelTime = edit.travelTime;
    }

    public override AbilityEffect getAbilityEffect(Actor actor, Vector3 targetPosition)
    {
        return new MovementEffect(name, actor, targetPosition, this);
    }
}
