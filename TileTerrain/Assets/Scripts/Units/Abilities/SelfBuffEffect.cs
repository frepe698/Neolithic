using UnityEngine;
using System.Collections;

public class SelfBuffEffect : AbilityEffect{

    private SelfBuffEffectData data;
    public SelfBuffEffect(string name, Unit unit, SelfBuffEffectData data) : base(name, unit, new Vector3(0,0,0))
    {
        this.data = data;
    }

    public override void action()
    {
        applyDamage(data.hitDamage, unit, unit, data.expSkill);
        GameMaster.getGameController().requestApplyEffect(unit.getID(), unit.getID(), data.name);
    }
}
