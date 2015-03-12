using UnityEngine;
using System.Collections;

public class WarpObject : TileObject {

    private Vector2 destination;
    private float destRotation;

    public WarpObject(Vector3 position, float rotation, string name, Vector2 destination, float destRotation)
        : base(position, rotation, name)
    {
        this.destination = destination;
        this.destRotation = destRotation;
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

    public float getDestRotation()
    {
        return destRotation;
    }
}
