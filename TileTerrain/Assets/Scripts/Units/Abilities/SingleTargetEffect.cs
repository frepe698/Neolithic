using UnityEngine;
using System.Collections;

public class SingleTargetEffect  : AbilityEffect {

    private SingleTargetEffectData data;

    public SingleTargetEffect(string name, Unit unit, Vector2 targetPosition) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = (SingleTargetEffectData)DataHolder.Instance.getEffectData(name);
    }

    public override void action()
    {
        //Find closest target within aoe
        //Apply damage to target
        //Apply on hit effects on target
    }
}
