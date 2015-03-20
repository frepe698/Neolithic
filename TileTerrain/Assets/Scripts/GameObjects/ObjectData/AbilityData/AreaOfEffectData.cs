using UnityEngine;
using System.Collections;
using Edit;

public class AreaOfEffectData : AbilityEffectData {

    public readonly float radius;

     public AreaOfEffectData() : base() { }

     public AreaOfEffectData(AreaOfEffectEdit edit)
         : base(edit)
    {
        this.radius = edit.radius;
    }

    public override AbilityEffect getAbilityEffect(Unit unit, Vector3 targetPosition)
    {
        return new AreaOfEffect(name, unit, targetPosition, this);
    }
}
