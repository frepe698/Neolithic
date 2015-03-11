using UnityEngine;
using System.Collections;

public class TileObject {

	protected GameObject poolObject;
	protected Vector3 position;
	protected float rotation;
	protected Vector2 scale; // x = thickness, y = height
	protected string poolName;
	protected bool blocks;

	public TileObject(Vector3 position, float rotation, string name)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = new Vector2(1, 1);
        this.poolName = name;
        this.blocks = false;
	}

    public TileObject(Vector3 position, float rotation)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = new Vector2(1, 1);
        this.blocks = false;
    }

	public bool Activate()
	{
        if (poolName.Equals("caveEntrance")) Debug.Log("Activate cave ");
		if(ObjectActive()) return true; //Object already active, do nothing

		poolObject = ObjectPoolingManager.Instance.GetObject(poolName);
		if(poolObject == null) return false; //Object pool was full and no more objects are available

		poolObject.transform.position = position;
		poolObject.transform.eulerAngles = new Vector3(0, rotation, 0);
		poolObject.transform.localScale = new Vector3(scale.x, scale.y, scale.x);
		return true; //Everything worked as expected
	}

	public void Inactivate()
	{
		if(!ObjectActive()) return; //Already inactive
		ObjectPoolingManager.Instance.ReturnObject(poolName, poolObject);
		poolObject = null;
	}


	public bool ObjectActive()
	{
		return (poolObject != null && poolObject.activeSelf == true);
	}

	public GameObject getObject()
	{
		return poolObject;
	}

	public Vector3 getPosition()
	{
		return position;
	}

	public float getRotation()
	{
		return rotation;
	}

	public void setPosition(Vector3 pos)
	{
		position = pos;
	}

	public void setRotation(float rot)
	{
		rotation = rot;
	}
	
	public virtual string getName()
	{
		return poolName;
	}

	public void setName(string name)
	{
		poolName = name;
	}

	public virtual float getActionRadius()
	{
		return 1.0f;
	}

	public Vector2 get2DPos()
	{
		return new Vector2(position.x, position.z);
	}

	public override bool Equals(object o)
	{
		TileObject other = o as TileObject;
		if(other == null) return false;
		return other.get2DPos() == this.get2DPos();
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public bool blocksProjectile()
	{
		return blocks;
	}
}
