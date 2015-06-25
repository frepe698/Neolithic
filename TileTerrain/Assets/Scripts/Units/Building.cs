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
        if (!awake) return;
        if (command != null)
        {
            if (command.isCompleted()) command = null;
            else command.update();
        }
    }

    public override int getTeam()
    {
        return team;
    }

    public float getWarmth()
    {
        return warmth;
    }

    public override void setPosition(Vector3 position)
    {
        this.position = position;
        if (isActive())
        {
            gameObject.transform.position = position;
        }
    }

    public override void onEnterNewTile()
    {
        for (int x = tile.x - Building.WARMTH_TILE_RANGE; x < tile.x + Building.WARMTH_TILE_RANGE + 1; x++)
        {
            for (int y = tile.y - Building.WARMTH_TILE_RANGE; y < tile.y + Building.WARMTH_TILE_RANGE + 1; y++)
            {
                if (!World.getMap().isValidTile(x, y)) continue;
                Tile checkTile = World.getMap().getTile(x, y);
                if (checkTile.containsActors() /*&& Pathfinder.unhinderedTilePath(World.getMap(), get2DPos(), new Vector2(x, y), id)*/)
                {
                    foreach (Actor actor in checkTile.getActors())
                    {
                        Hero hero = actor as Hero;
                        if (hero == null) continue;
                        float distance = Vector2i.getDistance(new Vector2i(x, y), tile);
                        if (distance <= Building.WARMTH_TILE_RANGE)
                            hero.calculateBuildingWarmth();
                    }
                }
            }
        }
    }

    protected override void setTag()
    {
        if (gameObject != null)
        {
            gameObject.tag = alive ? "Building" : "DeadUnit";
        }
    }
}
