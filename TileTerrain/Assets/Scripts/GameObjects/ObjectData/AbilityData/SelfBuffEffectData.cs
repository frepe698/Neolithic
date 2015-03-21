using UnityEngine;
using System.Collections;
using Edit;

public class SelfBuffEffectData : AbilityEffectData{

    public SelfBuffEffectData() : base() { }

    public SelfBuffEffectData(SelfBuffEffectEdit edit) : base(edit)
    {

    }

    public override AbilityEffect getAbilityEffect(Unit unit, Vector3 targetPosition)
    {
        return new SelfBuffEffect(name, unit, this);
    }
}
