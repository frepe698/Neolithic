using UnityEngine;
using System.Collections;

public class Eyecandy
{

    private RenderData renderData;
    private Vector3 position;
    private Quaternion rotation;
    private string poolName;

    public Eyecandy(Vector3 position, Quaternion rotation, float yScale, string poolName)
    {
        this.position = position;
        this.rotation = rotation;
        this.poolName = poolName;
        renderData = RenderDataPool.Instance.GetEyecandyData(poolName);
        if (renderData == null) Debug.Log("data is null");
    }

    public Eyecandy(Vector3 position, Quaternion rotation, string poolName)
    {
        this.position = position;
        this.rotation = rotation;
        this.poolName = poolName;
        renderData = RenderDataPool.Instance.GetEyecandyData(poolName);
        if (renderData == null) Debug.Log("data is null");
    }


    public void render()
    {
        Graphics.DrawMesh(renderData.mesh, position, rotation, renderData.material, 0, Camera.main, 0, renderData.property, false, true);
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public Quaternion getRotation()
    {
        return rotation;
    }

    public void setPosition(Vector3 pos)
    {
        position = pos;
    }

    public void setRotation(Quaternion rot)
    {
        rotation = rot;
    }

    public string getName()
    {
        return poolName;
    }

    public void setName(string name)
    {
        poolName = name;
    }

    public Vector2 get2DPos()
    {
        return new Vector2(position.x, position.z);
    }

    public override bool Equals(object o)
    {
        Eyecandy other = o as Eyecandy;
        if (other == null) return false;
        return other.get2DPos() == this.get2DPos();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
