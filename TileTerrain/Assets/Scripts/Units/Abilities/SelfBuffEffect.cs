using UnityEngine;
using System.Collections;

public class SelfBuffEffect : AbilityEffect{

    private SelfBuffEffectData data;
    public SelfBuffEffect(string name, Actor actor, SelfBuffEffectData data)
        : base(name, actor, new Vector3(0, 0, 0))
    {
        this.data = data;
    }

    public override void action(AbilityCommand ability)
    {
        AbilityEffect.modelAndSound(data, actor, Vector3.zero, true);

        applyDamage(data.hitDamage, actor, actor, data.expSkill);
        GameMaster.getGameController().requestApplyEffect(actor.getID(), actor.getID(), data.name);
    }
}
