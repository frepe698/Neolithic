using UnityEngine;
using System.Collections;

public class Building : Actor {

    public const int WARMTH_TILE_RANGE = 4;

    private int team;
    protected float warmth;

    public Building(string name, Vector2i position, float yRotation, int id, int team)
        : base(name, new Vector3(position.x + 0.5f, 0, position.y + 0.5f), new Vector3(0, yRotation, 0), id)
    {
        this.team = team;
    }

    public Building(BuildingData data, Vector2i position, float yRotation, int id, int team)
        : base(data.name, new Vector3(position.x + 0.5f, 0, position.y + 0.5f), new Vector3(0, yRotation, 0), id)
    {
        this.team = team;
        this.warmth = data.warmth;
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

    public float getWarmth()
    {
        return warmth;
    }
}
