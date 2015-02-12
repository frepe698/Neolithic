using UnityEngine;
using System.Collections;

public class HarvestableObject {

	private GameObject poolObject;
	private Vector3 position;
	private Quaternion rotation;
	private string poolName;
	private int id;
	
	public HarvestableObject(Vector3 position, Quaternion rotation, string lootType)
	{
		this.position = position;
		this.rotation = rotation;
		this.poolName = lootType;
	}
	
	public bool Activate(int id)
	{
		this.id = id;
		if(ObjectActive())
		{
			poolObject.GetComponent<LootId>().setId(id);
			return true; //Object already active, just set id
		}
		poolObject = ObjectPoolingManager.Instance.GetObject(poolName);
		if(poolObject == null) return false; //Object pool was full and no more objects are available
		
		poolObject.transform.position = position;
		poolObject.transform.rotation = rotation;
		poolObject.SetActive(true);
		poolObject.GetComponent<LootId>().setId(id);
		return true; //Everything worked as expected
	}
	
	public void Inactivate()
	{
		if(!ObjectActive()) return; //Already inactive
		
		poolObject.SetActive(false);
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
		return poolName;
	}
	
	public void setName(string name)
	{
		poolName = name;
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
	
	public override bool Equals(object o)
	{
		LootableObject other = o as LootableObject;
		if(other == null) return false;
		return other.get2DPos() == this.get2DPos();
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	public string getLootType()
	{
		return poolName;
	}
}
