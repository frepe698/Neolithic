using UnityEngine;
using System.Collections;

public class WorldUnit {

	private UnitController unit;
	private Vector3 position;
	private string poolName;
	private ParticleSystem particles;
	private int health;
	
	
	public WorldUnit(Vector3 position, float rotation, string name, int unitID)
	{
		this.position = position;
		this.poolName = name;
		this.health = 10;
	}
	
	public UnitController Activate()
	{
		GameObject poolObject = ObjectPoolingManager.Instance.GetObject(poolName);
		if(poolObject == null) return null; //Object pool was full and no more objects are available
		
		poolObject.transform.position = position;
		poolObject.SetActive(true);

		UnitController unit = poolObject.GetComponent<UnitController>();

		return unit; //Everything worked as expected
	}

	public Vector3 getPosition()
	{
		return position;
	}

	public void setPosition(Vector3 pos)
	{
		position = pos;
	}

	public string getName()
	{
		return poolName;
	}
	
	public void setName(string name)
	{
		poolName = name;
	}
	
	public virtual float getGatherRadius()
	{
		return 1.0f;
	}
	
	public Vector2 get2DPos()
	{
		return new Vector2(position.x, position.z);
	}
	
	public override bool Equals(object o)
	{
		WorldUnit other = o as WorldUnit;
		if(other == null) return false;
		return other.getName() == this.getName() && other.get2DPos() == this.get2DPos();
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	public int getHealth()
	{
		return health;
	}
}
