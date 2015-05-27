using UnityEngine;
using System.Collections;

public class Building : Actor {

    private int team;

    public Building(string name, Vector2i position, float yRotation, int id, int team)
        : base(name, new Vector3(position.x + 0.5f, 0, position.y + 0.5f), new Vector3(0, yRotation, 0), id)
    {
        this.team = team;
    }

    public Building(BuildingData data, Vector2i position, float yRotation, int id, int team)
        : base(data.name, new Vector3(position.x + 0.5f, 0, position.y + 0.5f), new Vector3(0, yRotation, 0), id)
    {
        this.team = team;
        modelName = data.modelName;
        unitstats = new UnitStats(this, 0, data);
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        unitstats.updateStats();
    }

    public override void update()
    {

    }

    public override int getTeam()
    {
        return team;
    }
}
