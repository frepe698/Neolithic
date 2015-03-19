using UnityEngine;
using System.Collections;
using Edit;

public class ProjectileEffectData : AbilityEffectData {

    public float angle;

    public ProjectileEffectData() : base() { }

    public ProjectileEffectData(ProjectileEffectEdit edit) : base(edit)
    {
        angle = edit.angle;
    }

    public override AbilityEffect getAbilityEffect(Unit unit, Vector2 targetPosition)
    {
        return new ProjectileEffect(name, unit, targetPosition);
    }

	
}
