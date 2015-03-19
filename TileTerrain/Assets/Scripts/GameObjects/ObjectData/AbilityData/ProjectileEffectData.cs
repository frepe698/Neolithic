using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using Edit;

public class ProjectileEffectData : AbilityEffectData {

    [XmlElement(IsNullable = false)]
    public readonly string projectileName;

    public readonly bool weaponProjectile;
    public readonly float angle;

    public ProjectileEffectData() : base() { }

    public ProjectileEffectData(ProjectileEffectEdit edit) : base(edit)
    {
        weaponProjectile = edit.weaponProjectile;

        //only set projectile name if weapon projectile is false
        if(!weaponProjectile) projectileName = edit.projectileName;
        
        angle = edit.angle;
    }

    public override AbilityEffect getAbilityEffect(Unit unit, Vector3 targetPosition)
    {
        return new ProjectileEffect(name, unit, targetPosition);
    }

	
}
