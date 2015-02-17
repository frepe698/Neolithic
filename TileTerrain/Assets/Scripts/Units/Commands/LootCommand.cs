using UnityEngine;
using System.Collections;

public class LootCommand : Command {

	private LootableObject lootableObject;
	private bool looting;
	private bool hasLooted;
	private float lootTime;
	private float animationTime;
	
	public LootCommand(Unit unit, LootableObject lootableObject) : base(unit)
	{
		this.lootableObject = lootableObject;
		this.destination = lootableObject.get2DPos();
	}
	
	public override void start ()
	{
		unit.setPath(destination);
	}
	
	public override void update()
	{
		if(looting)
		{
			if(!hasLooted)
			{
				lootTime -= Time.deltaTime;
				if(lootTime <= 0) 
				{
					unit.loot(lootableObject);
					hasLooted = true;
				}
			}
			else
			{
				animationTime -= Time.deltaTime;
				if(animationTime <= 0)
				{
					setCompleted();
				}
			}
			
		}
		else if( Vector2.Distance(unit.get2DPos(), destination) < lootableObject.getGatherRadius() )
		{
			Debug.Log ("in range to loot");
			unit.setMoving(false);
			if(World.tileMap.getTile(new Vector2i(lootableObject.get2DPos())).getLootableObject(lootableObject.getID()) != null)
			{
				looting = true;
				lootTime = unit.getLootTime();
				hasLooted = false;
				animationTime = lootTime * 0.3f;
				calculateRotation();
				unit.setAnimationRestart(unit.getLootAnim());
			}
			else
			{
				setCompleted();
			}
			
			
			
		}
	}
	
	private void calculateRotation()
	{
		Vector2 dir = (unit.get2DPos()-destination).normalized;
		unit.setRotation (new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0));
		
	}
	
	public LootableObject getLootableObject()
	{
		return lootableObject;
	}
	
	public override bool Equals(object o)
	{
		LootCommand other = o as LootCommand;
		if(other == null || other.getLootableObject() == null || getLootableObject() == null) return false;
		return other.getLootableObject().Equals(this.getLootableObject());
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}	
	
}
