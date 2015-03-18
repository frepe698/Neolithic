﻿using UnityEngine;
using System.Collections;

public class SingleTargetEffect  : AbilityEffect {

    private readonly static float radius = 2;
    private SingleTargetEffectData data;

    public SingleTargetEffect(string name, Unit unit, Vector2 targetPosition) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = (SingleTargetEffectData)DataHolder.Instance.getEffectData(name);
    }

    public override void action()
    {
        //Load some kind of visual effect
        Vector2i targetTile = new Vector2i(targetPosition);

        Unit closestTarget = null;
        float closestDistance = radius + 1;

        for (int x = targetTile.x - (int)radius - 1; x < targetTile.x + (int)radius + 1; x++)
        {
            for (int y = targetTile.y - (int)radius - 1; y < targetTile.y + (int)radius + 1; y++)
            {
                Vector2i tile = new Vector2i(x, y);
                foreach (Unit target in World.tileMap.getTile(tile).getUnits())
                {
                    if (target.getID() == unit.getID()
                        || target.getTeam() == unit.getTeam()) continue;

                    float distance = Vector2.Distance(target.get2DPos(), unit.get2DPos());
                    if (distance > radius) continue;

                    if (distance < closestDistance) closestTarget = target;
                }
            }
        }
        //No target hit
        if (closestTarget == null) return;

        //Target is officially hit here, calculate damage
        int selfDamage = 0;
        int damage = 0;
        foreach (HitDamage hit in data.hitDamage)
        {
            int tempDamage;
            if (hit.yourStat)
            {
                tempDamage = (int)(unit.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            else
            {
                tempDamage = (int)(closestTarget.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            if (hit.damageSelf) selfDamage += tempDamage;
            else damage += tempDamage;
        }
        GameMaster.getGameController().requestHit(damage, unit.getID(), closestTarget.getID());
    }
}
