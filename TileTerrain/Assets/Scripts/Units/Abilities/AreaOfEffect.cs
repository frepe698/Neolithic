using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaOfEffect : AbilityEffect {

    private AreaOfEffectData data;

    public AreaOfEffect(string name, Actor actor, Vector3 targetPosition, AreaOfEffectData data)
        : base(name, actor, targetPosition)
    {
        //Fetch data with name
        this.data = data;
    }
    public override void action(AbilityCommand ability)
    {
        //Load some kind of visual effect
        /*GameObject prefab = Resources.Load<GameObject>("Effects/"+data.modelName);
        if (prefab != null)
        {
            GameObject.Instantiate(prefab, targetPosition - Vector3.up, Quaternion.identity);
        }*/
        AbilityEffect.modelAndSound(data, actor, targetPosition);
        float radius = data.radius;
        Vector2i targetTile = new Vector2i(targetPosition);
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);
        for(int x = targetTile.x - (int)radius - 1; x < targetTile.x + (int)radius + 1; x++)
        {
            for (int y = targetTile.y - (int)radius - 1; y < targetTile.y + (int)radius + 1; y++)
            {
                if (!World.tileMap.isValidTile(x, y)) continue;
                Vector2i tile = new Vector2i(x, y);
                List<Actor> actors = World.tileMap.getTile(tile).getActors();
                for (int i = actors.Count - 1; i >= 0; i--)
                {
                    Actor target = actors[i];
                    if (target.getID() == actor.getID()
                        || target.getTeam() == actor.getTeam()
                        || Vector2.Distance(target.get2DPos(), targetPosition2D) > radius) continue;

                    //Target is officially hit here, calculate damage
                    applyDamage(data.hitDamage, actor, target, data.expSkill);

                    //Apply buffs if there are buffs
                    if (data.hitBuffs.Length > 0)
                        GameMaster.getGameController().requestApplyEffect(actor.getID(), target.getID(), data.name);
                }
            }
        }
    }
}
