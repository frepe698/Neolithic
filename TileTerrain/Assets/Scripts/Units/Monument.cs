using UnityEngine;
using System.Collections;

public class Monument : Building {

    public Monument(string name, Vector2i position, float yRotation, int id, int team)
        : base(name, position, yRotation, id, team)
    {
        MonumentData data = DataHolder.Instance.getMonumentData(name);
        if (data == null)
        {
            Debug.LogError("The Monument " + name + " has no data");
            return;
        }
        modelName = data.modelName;
        unitstats = new UnitStats(this, 0, data);
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        unitstats.updateStats();
    }

    public Monument(MonumentData data, Vector2i position, float yRotation, int id, int team)
        : base(data.name, position, yRotation, id, team)
    {
        modelName = data.modelName;
        unitstats = new UnitStats(this, 0, data);
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        unitstats.updateStats();
    }
}
