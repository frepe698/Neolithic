using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceObject : TileObject
{
	protected int health;

	protected ResourceData data;

	public ResourceObject(Vector3 position, float rotation, string name)
        : base(position, rotation)
	{
		this.scale = new Vector2(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
		data = DataHolder.Instance.getResourceData(name);
		this.poolName = name+data.getRandomVariance();
		this.health = data.health;
		this.blocks = data.blocksProjectile;
	}

    public ResourceObject(Vector3 position, float rotation)
        : base(position, rotation)
    {
 
    }

    public ResourceObject(Vector3 position, float rotation, ResourceData data)
        : base(position, rotation)
    {
        this.scale = new Vector2(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
        this.data = data;
        this.poolName = data.modelName + data.getRandomVariance();
        this.health = data.health;
        this.blocks = data.blocksProjectile;
    }
	

	public override bool Equals(object o)
	{
		ResourceObject other = o as ResourceObject;
		if(other == null) return false;
		return other.get2DPos() == this.get2DPos();
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public virtual string getDrops()
	{
		return "0;0";
	}

	public bool dealDamage(int damage)
	{
		if(ObjectActive())
		{
			ParticleSystem particles = ParticlePoolingManager.Instance.GetObject(data.hitParticle);
			if(particles != null)
			{
				particles.transform.position = new Vector3(position.x, position.y + 1, position.z);
                particles.Play();
			}
			Animator anim = poolObject.transform.GetComponentInChildren<Animator>();
			if(anim != null)
			{
				anim.SetTrigger("hit");
			}
		}

		health -= damage;
		return health <= 0;
	}

    public override string getName()
    {
        return data.name;
    }

	public int getHealth()
	{
		return health;
	}

	public int getDamageType()
	{
		return data.damageType;
	}

    public virtual string harvest()
    {
        return null;
    }

    public virtual bool canBeHarvested()
    {
        return false;
    }
	
}
