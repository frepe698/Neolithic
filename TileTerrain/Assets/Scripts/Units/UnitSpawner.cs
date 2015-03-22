using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawner {

    protected readonly string unitName;
    protected int maxUnits;
    protected List<Unit> units = new List<Unit>();
    protected Vector2i position;
    protected int id;
    public UnitSpawner(string unitName, int maxUnits, Vector2i pos, int id)
    {
        this.unitName = unitName;
        this.maxUnits = maxUnits;
        this.position = pos;
        //respawnUnits();

    }

    public virtual void respawnUnits()
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
        for(int x = -maxUnits; x < maxUnits; x++)
        {
            for (int y = -maxUnits; y < maxUnits; y++)
            {
                Vector2i spawnPos = position + new Vector2i(x,y);
                if(World.tileMap.isValidTile(spawnPos) && World.tileMap.getTile(spawnPos).isWalkable(unitID))
                {
                    AIUnit unit = new AIUnit(unitName, new Vector3(spawnPos.x + 0.5f, 0, spawnPos.y + 0.5f), Vector3.zero, unitID);
                    GameMaster.addUnit(unit);
                    units.Add(unit);
                    unitsSpawned++;
                    unitID = GameMaster.getNextUnitID();
                    if (unitsSpawned >= unitsToSpawn) return;
                }
            }
        }
        if (maxUnits - units.Count > 0) Debug.Log("Not every unit was spawned");
    }

    public void respawnIfInactive()
    {
        if (World.tileMap.getTile(position).isActive()) return;
        respawnUnits();
    }

    public void removeUnits()
    {
        foreach(Unit unit in units)
        {
            GameMaster.removeUnit(unit);
        }
        units = new List<Unit>();
    }

    public void removeInactiveUnits()
    {
        List<Unit> newUnits = new List<Unit>();
        foreach(Unit unit in units)
        {
            if(!unit.isActive())
            {
                GameMaster.removeUnit(unit);
            }
            else
            {
                newUnits.Add(unit);
            }
        }
        units = newUnits;
    }

    public int getID()
    {
        return id;
    }

}
