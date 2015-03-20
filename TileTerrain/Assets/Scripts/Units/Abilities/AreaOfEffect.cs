using UnityEngine;
using System.Collections;

public class AreaOfEffect : AbilityEffect {

    private AreaOfEffectData data;

    public AreaOfEffect(string name, Unit unit, Vector3 targetPosition, AreaOfEffectData data) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = data;
    }
    public override void action()
    {
        //Load some kind of visual effect
        float radius = data.radius;
        Vector2i targetTile = new Vector2i(targetPosition);
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.z);
        for(int x = targetTile.x - (int)radius - 1; x < targetTile.x + (int)radius + 1; x++)
        {
            for (int y = targetTile.y - (int)radius - 1; y < targetTile.y + (int)radius + 1; y++)
            {
                Vector2i tile = new Vector2i(x, y);
                foreach(Unit target in World.tileMap.getTile(tile).getUnits())
                {
                    if (target.getID() == unit.getID() 
                        || target.getTeam() == unit.getTeam()
                        || Vector2.Distance(target.get2DPos(), targetPosition2D) > radius) continue;

                    //Target is officially hit here, calculate damage
                    int selfDamage = 0;
                    int damage = 0;
                    foreach(HitDamage hit in data.hitDamage)
                    {
                        int tempDamage;
                        if(hit.yourStat)
                        {
                            tempDamage = (int)(unit.getUnitStats().getStatV(hit.stat)*hit.percent);
                        }
                        else
                        {
                            tempDamage = (int)(target.getUnitStats().getStatV(hit.stat)*hit.percent);
                        }
                        if (hit.damageSelf) selfDamage += tempDamage;
                        else damage += tempDamage;
                    }
                    GameMaster.getGameController().requestHit(damage, unit.getID(), target.getID());
                    applyBuffs(data.hitBuffs, target);
                }
            }
        }
    }
}
