using UnityEngine;
using System.Collections;
using Edit;

public class MonumentData : BuildingData {

    public MonumentData()
    { 
    }

    public MonumentData(MonumentEdit edit)
        : base(edit)
    { 
    }

    public virtual Building getBuilding(Vector2i position, float yRotation, int id, int team)
    {
        return new Monument(this, position, yRotation, id, team);
    }
}
