using UnityEngine;
using System.Collections;
using Edit;

public class SingleTargetEffectData : AbilityEffectData {

    public SingleTargetEffectData() : base() { }

    public SingleTargetEffectData(SingleTargetEffectEdit edit) : base(edit)
    {
        
    }

    public override AbilityEffect getAbilityEffect(Unit unit, Vector3 targetPosition)
    {
        return new SingleTargetEffect(name, unit, targetPosition);
    }
}
