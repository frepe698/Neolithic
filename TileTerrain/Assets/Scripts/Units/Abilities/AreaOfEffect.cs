using UnityEngine;
using System.Collections;

public class AreaOfEffect : AbilityEffect {

    private AreaOfEffectData data;

    public AreaOfEffect(string name, Unit unit, Vector2 targetPosition) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = (AreaOfEffectData)DataHolder.Instance.getEffectData(name);
    }
    public override void action()
    {
        base.action();
    }
}
