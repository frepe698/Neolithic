using UnityEngine;
using System.Collections;

public class Projectile {

	private GameObject poolObject;
	private string projectileName;
    private AbilityEffectData effectData;
	private int id;
	
	private Vector3 position;
	private Vector3 velocity;
	private Vector2 rotation;
	float speed;

	private float range;

	private bool removed = false;

	private int unitID;
    private Unit unit;

	/*public Projectile(Vector3 start, Vector3 goal, float range, float speed, string name, int damage, int unitID)
	{
		this.position = start;
		Vector3 direction = (goal - start).normalized;
		rotation = new Vector2(direction.y*90
		                       , Mathf.Rad2Deg*Mathf.Atan2(direction.x, direction.z) + 180);
		velocity = direction*speed;
		this.speed = speed;
		this.range = range;
		this.projectileName = name;
		this.unitID = unitID;
        this.unit = GameMaster.getUnit(unitID);
	}*/

    public Projectile(Vector3 start, Vector3 goal, float range, float speed, string projectileName, string dataName, int unitID)
    {
        this.position = start;
        Vector3 direction = (goal - start).normalized;
        rotation = new Vector2(direction.y * 90
                               , Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z) + 180);
        velocity = direction * speed;
        this.speed = speed;
        this.range = range;
        this.projectileName = projectileName;
        this.effectData = DataHolder.Instance.getEffectData(dataName);
        this.unitID = unitID;
        this.unit = GameMaster.getUnit(unitID);

    }

	public void update()
	{
		if(range <= 0)
		{
			removed = true;
			return;
		}

		position += velocity*Time.deltaTime;
		range -= speed*Time.deltaTime;

		Vector2i curtile = new Vector2i(position);
		if(!World.getMap().isValidTile(curtile) || World.getMap().getTile(curtile).blocksProjectile())
		{
			removed = true;
			return;
		}

		Line line = new Line(get2DPos(), get2DPos() + new Vector2(velocity.x*Time.deltaTime, velocity.z*Time.deltaTime)*2);
		for(int x = curtile.x - 1; x < curtile.x + 2; x++)
		{
			for(int y = curtile.y - 1; y < curtile.y + 2; y++)
			{
				if(!World.getMap().isValidTile(x, y)) continue;
				Tile tile = World.getMap().getTile(x,y);
				foreach(Unit target in tile.getUnits())
				{
					if(target.getID() == unitID) continue;
					if(line.distanceFromPoint(target.get2DPos()) < target.getSize())
					{
                        hitTarget(target);
						
						removed = true;
					}
				}
			}
		}

		if(ObjectActive())
		{
			poolObject.transform.position = position;
		}
	}

	public bool Activate()
	{
		if(ObjectActive()) return true; //Object already active, do nothing
		
		poolObject = ObjectPoolingManager.Instance.GetObject(projectileName);
		if(poolObject == null) return false; //Object pool was full and no more objects are available
		
		poolObject.transform.position = position;
		poolObject.transform.eulerAngles = new Vector3(rotation.x, rotation.y, 0);
		return true; //Everything worked as expected
	}


	public void Inactivate()
	{
		if(!ObjectActive()) return; //Already inactive
		ObjectPoolingManager.Instance.ReturnObject(projectileName, poolObject);
		poolObject = null;
	}
	
	
	public bool ObjectActive()
	{
		return (poolObject != null && poolObject.activeSelf == true);
	}

	public bool isRemoved()
	{
		return removed;
	}
				
	public Vector2 get2DPos()
	{
		return new Vector2(position.x, position.z);
	}

	public Vector2 getCollisionPos()
	{
		return get2DPos();
	}

    private void hitTarget(Unit target)
    {
        int damage = 0;
        foreach (HitDamage hit in effectData.hitDamage)
        {
            int tempDamage = 0;
            if (hit.yourStat)
            {
                tempDamage = (int)(unit.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            damage += tempDamage;
        }
        GameMaster.getGameController().requestProjectileHit(damage, unitID, target.getID());
        applyBuffs(effectData.hitBuffs, target);
    }

    protected void applyBuffs(HitBuff[] buffs, Unit target)
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            HitBuff hbuff = buffs[i];
            object[] parameters = new object[] { hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent };
            string name = "get" + hbuff.type.ToString();
            GameMaster.getGameController().requestAddBuff(target.getID(), name, parameters);
        }
    }

}
