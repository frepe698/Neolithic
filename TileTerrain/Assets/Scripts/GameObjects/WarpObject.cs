using UnityEngine;
using System.Collections;

public class WarpObject : TileObject {

    private Vector2 destination;

    public WarpObject(Vector3 position, float rotation, string name, Vector2 destination)
        : base(position, rotation, name)
    {
        this.destination = destination;
    }

    public Vector2 getDestination()
    {
        return destination;
    }

    public override bool Equals(object o)
    {
        WarpObject other = o as WarpObject;
        if (other == null) return false;
        return other.get2DPos() == this.get2DPos();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
