using UnityEngine;
using System.Collections;

public class ProjectileEffect : AbilityEffect {

    private ProjectileEffectData data;

    public ProjectileEffect(string name, Unit unit, Vector2 targetPosition) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = (ProjectileEffectData)DataHolder.Instance.getEffectData(name);
    }

    public override void action()
    {
        float angle = data.angle;
        string name = data.modelName;
        base.action();
    }
}
