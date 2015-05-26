using UnityEngine;
using System.Collections;

public class SingleTargetEffect  : AbilityEffect {

    private readonly static float radius = 1;
    private SingleTargetEffectData data;

    public SingleTargetEffect(string name, Actor actor, Vector3 targetPosition, SingleTargetEffectData data)
        : base(name, actor, targetPosition)
    {
        //Fetch data with name
        this.data = data;
    }

    public override void action(AbilityCommand ability)
    {
        //Load some kind of visual effect
        AbilityEffect.modelAndSound(data, actor, actor.getPosition());


        Vector2i targetTile = new Vector2i(targetPosition);

        Actor closestTarget = null;
        float closestDistance = radius + 1;
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);

        for (int x = targetTile.x - ((int)radius + 1); x < targetTile.x + (int)radius + 1; x++)
        {
            for (int y = targetTile.y - ((int)radius + 1); y < targetTile.y + (int)radius + 1; y++)
            {
                if (!World.tileMap.isValidTile(x, y)) continue;
                Vector2i tile = new Vector2i(x, y);
                foreach (Actor target in World.tileMap.getTile(tile).getActors())
                {
                    if (target.getID() == actor.getID()
                        || target.getTeam() == actor.getTeam()) continue;

                    float distance = Vector2.Distance(target.get2DPos(), targetPosition2D);
                    if (distance > radius) continue;

                    if (distance < closestDistance) closestTarget = target;
                }
            }
        }
        //No target hit
        if (closestTarget == null) return;

        //Target is officially hit here, calculate damage
        applyDamage(data.hitDamage, actor, closestTarget, data.expSkill);

        //Apply buffs if there are buffs
        if (data.hitBuffs.Length > 0)
            GameMaster.getGameController().requestApplyEffect(actor.getID(), closestTarget.getID(), data.name);
    }
}
