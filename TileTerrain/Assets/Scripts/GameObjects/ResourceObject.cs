using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceObject 
{
	private GameObject poolObject;
	private Vector3 position;
	private float rotation;
	private Vector2 scale; // x = thickness, y = height
	private float lightness;
	private string poolName;
	private int health;
	private bool blocks;

	private ResourceData data;

	public ResourceObject(Vector3 position, float rotation, string name)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = new Vector2(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
		//this.lightness = Random.Range(0.6f, 1.0f);
		data = DataHolder.Instance.getResourceData(name);
		this.poolName = name+data.getRandomVariance();
		this.health = data.health;
		this.blocks = data.blocksProjectile;
	}

	public bool Activate()
	{
		if(ObjectActive()) return true; //Object already active, do nothing

		poolObject = ObjectPoolingManager.Instance.GetObject(poolName);
		if(poolObject == null) return false; //Object pool was full and no more objects are available

		poolObject.transform.position = position;
		poolObject.transform.eulerAngles = new Vector3(0, rotation, 0);
		poolObject.transform.localScale = new Vector3(scale.x, scale.y, scale.x);
		//poolObject.GetComponentInChildren<Renderer>().material.color = new Color(lightness, lightness, lightness);
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
	
	public string getName()
	{
		return data.name;
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

	public int getHealth()
	{
		return health;
	}

	public int getDamageType()
	{
		return data.damageType;
	}

	public bool blocksProjectile()
	{
		return blocks;
	}
	
}
