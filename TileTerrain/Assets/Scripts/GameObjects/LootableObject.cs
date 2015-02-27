using UnityEngine;
using System.Collections;

public class LootableObject {

	private GameObject poolObject;
	private Vector3 position;
	private Quaternion rotation;
    private string name;
	private string poolName;
	private int id;

	public LootableObject(Vector3 position, Quaternion rotation, string name, string poolName)
	{
		this.position = position;
		this.rotation = rotation;
        this.name = name;
		this.poolName = poolName;
	}

    public LootableObject(Vector2 position2D, Quaternion rotation, string name, string poolName)
    {
        this.position = new Vector3(position2D.x, World.getHeight(position2D), position2D.y);
        this.rotation = rotation;
        this.name = name;
        this.poolName = poolName;
    }

    public LootableObject(Vector2 position2D, float yrotation, string name, string poolName)
    {
        Vector3 groundNormal;
        float groundHeight = World.getHeight(position2D, out groundNormal);
        this.position = new Vector3(position2D.x, groundHeight, position2D.y);
        this.rotation.eulerAngles = new Vector3(Mathf.Sin(yrotation * Mathf.Deg2Rad)*Mathf.Rad2Deg * groundNormal.z, yrotation, Mathf.Cos(yrotation * Mathf.Deg2Rad)*Mathf.Rad2Deg * groundNormal.x);
        
        this.name = name;
        this.poolName = poolName;
    }
	
	public bool Activate()
	{
		if(ObjectActive())
		{
			poolObject.GetComponent<LootId>().setId(id);
			return true; //Object already active, just set id
		}
		poolObject = ObjectPoolingManager.Instance.GetObject(poolName);
		if(poolObject == null) return false; //Object pool was full and no more objects are available
		
		poolObject.transform.position = position;
		poolObject.transform.rotation = rotation;
		poolObject.GetComponent<LootId>().setId(id);
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
		return name;
	}
	
	public void setName(string name)
	{
		this.name = name;
	}

	public void setID(int id)
	{
		this.id = id;
	}

	public int getID()
	{
		return id;
	}
	
	public virtual float getGatherRadius()
	{
		return 0.1f;
	}
	
	public Vector2 get2DPos()
	{
		return new Vector2(position.x, position.z);
	}

	public Vector2i getTile()
	{
		return new Vector2i(position.x, position.z);
	}
	
	public override bool Equals(object o)
	{
		LootableObject other = o as LootableObject;
		if(other == null || other.getTile() != getTile()) return false;
		return other.getID() == this.getID();
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public string getLootType()
	{
		return name;
	}

	public virtual Item getItem()
	{
		return new MaterialItem(poolName);
	}


}
