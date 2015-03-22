using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathUnitSpawner : UnitSpawner
{
    private List<WorldPoint> points;
    public PathUnitSpawner(string unitName, int maxUnits, Vector2i pos, int id, List<WorldPoint> points) : base(unitName, maxUnits, pos, id)
    {
        this.points = points;
    }

    public override void respawnUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];
            if (!unit.isAlive())
            {
                units.Remove(unit);
                i--;
            }
        }
        int unitsToSpawn = maxUnits - units.Count;
        if (unitsToSpawn <= 0) return;
        int unitsSpawned = 0;

        int unitID = GameMaster.getNextUnitID();
        for (int x = -maxUnits; x < maxUnits; x++)
        {
            for (int y = -maxUnits; y < maxUnits; y++)
            {
                Vector2i spawnPos = position + new Vector2i(x, y);
                if (World.tileMap.isValidTile(spawnPos) && World.tileMap.getTile(spawnPos).isWalkable(unitID))
                {
                    AIUnit unit = new PathAIUnit(unitName, new Vector3(spawnPos.x + 0.5f, 0, spawnPos.y + 0.5f), Vector3.zero, unitID, points);
                    GameMaster.addAwakeUnit(unit);
                    units.Add(unit);
                    unitsSpawned++;
                    unitID = GameMaster.getNextUnitID();
                    if (unitsSpawned >= unitsToSpawn) return;
                }
            }
        }
        if (maxUnits - units.Count > 0) Debug.Log("Not every unit was spawned");
    }
}
