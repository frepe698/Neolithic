using UnityEngine;
using System.Collections;

public class AreaOfEffect : AbilityEffect {

    private AreaOfEffectData data;

    public AreaOfEffect(string name, Unit unit, Vector3 targetPosition, AreaOfEffectData data) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = data;
    }
    public override void action(AbilityCommand ability)
    {
        ParticleSystem particles = ParticlePoolingManager.Instance.GetObject(data.modelName);
        if (particles != null)
        {
            particles.transform.position = targetPosition - Vector3.up;
            
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        //Load some kind of visual effect
        float radius = data.radius;
        Vector2i targetTile = new Vector2i(targetPosition);
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);
        for(int x = targetTile.x - (int)radius - 1; x < targetTile.x + (int)radius + 1; x++)
        {
            for (int y = targetTile.y - (int)radius - 1; y < targetTile.y + (int)radius + 1; y++)
            {
                if (!World.tileMap.isValidTile(x, y)) continue;
                Vector2i tile = new Vector2i(x, y);
                foreach(Unit target in World.tileMap.getTile(tile).getUnits())
                {
                    if (target.getID() == unit.getID() 
                        || target.getTeam() == unit.getTeam()
                        || Vector2.Distance(target.get2DPos(), targetPosition2D) > radius) continue;

                    //Target is officially hit here, calculate damage
                    applyDamage(data.hitDamage, unit, target, data.expSkill);
                    
                    //Apply buffs if there are buffs
                    if(data.hitBuffs.Length > 0)
                        GameMaster.getGameController().requestApplyEffect(unit.getID(), target.getID(), data.name);
                }
            }
        }
    }
}
