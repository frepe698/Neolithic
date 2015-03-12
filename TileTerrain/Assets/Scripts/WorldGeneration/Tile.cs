using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile {

	public static readonly short stUnset = 0;
	public static readonly short stFixed = 1;
	public static readonly short stSetable = 2;

	private short h;
	private short s;

    private bool cliff = false;

	private TileObject tileObject;
	private List<LootableObject> lootableObjects = new List<LootableObject>();
	private List<Eyecandy> eyecandyObjects = new List<Eyecandy>();
	private List<Unit> units = new List<Unit>();
	private bool active = false;

	private static GroundType[] groundTypes = new GroundType[]{
		new GrassGround(),
		new ForestGround(),
		new DeadForestGround(),
		new WaterGround(),
		new ShoreGround(),
		new BeachGround(),
		new RoadGround(),
		new MountainGround(),
		new SnowMountainGround(),
	};

	private short groundType;

	public Tile()
	{
		this.h = 0;
		this.s = stUnset;
		groundType = -1;
	}

	public Tile(short height, short state)
	{
		this.h = height;
		this.s = state;
		groundType = -1;
	}

	public short height
	{
		get{return h;}
		set
		{
			if(s!=stFixed){
				s = stSetable;
			}
			h = value;
		}
	}

	public short ground
	{
		get{return groundType;}
		set
		{
			if(value >= 0 && value < groundTypes.Length){
				groundType = value;
			}
		}
	}

	public short state
	{
		get{return s;}
		set{s = value;}
	}

	public bool canTree()
	{
		return s == stSetable && height > 0;
	}

    public void setCliff(bool cliff)
    {
        this.cliff = cliff;
    }

    public bool isCliff()
    {
        return cliff;
    }

	public bool isWalkable(int id)
	{
        if (cliff) return false;
		if((units.Count == 0 && tileObject == null) || (units.Count == 1 && containsUnit(id))) return true;

		if(tileObject != null || units.Count > 0) return false;
		return true;

	}

    public bool hasResourceObject()
    {
        return tileObject != null && tileObject is ResourceObject;
    }

	public bool hasTileObject()
	{
		return tileObject != null;
	}

	public void renderEyecandy()
	{
		foreach(Eyecandy eo in eyecandyObjects)
		{
			eo.render();
		}
	}
	

	public void setTileObject(TileObject obj)
	{
		tileObject = obj;
	}

	public ResourceObject getResourceObject()
	{
		return tileObject as ResourceObject;
	}

    public TileObject getTileObject()
    {
        return tileObject;
    }

	public void activateTile()
	{
		World.addActiveTile(this);
		active = true;
		if(tileObject != null) tileObject.Activate();
		foreach(LootableObject loot in lootableObjects)
		{
			loot.Activate();
		}
		foreach(Unit unit in units)
		{
			unit.activate();
		}
	}

	public void resetLootIds()
	{
		int id = 0;
		foreach(LootableObject loot in lootableObjects)
		{
			loot.setID(id++);
		}
	}

	public void inactivateTile()
	{
		World.removeActiveTile(this);
		//if(!active) return;
		active = false;
		if(tileObject != null) tileObject.Inactivate();
		foreach(LootableObject loot in lootableObjects)
		{
			loot.Inactivate();
		}
		foreach(Unit unit in units)
		{
			unit.inactivate();
		}
	}

	public bool isActive()
	{
		return active;
	}

	public int getColor()
	{
		return groundTypes[groundType].getTexture();

	}

	public GroundType getGroundType()
	{
		return groundTypes[groundType];
	}

	public string removeTileObject()
	{
		if(tileObject == null) return "";
		string name = tileObject.getName();
		tileObject.Inactivate();
		tileObject = null;
		return name;
	}

	public Item removeAndReturnLootObject(int id)
	{
		if(id >= 0)
		{
			int lootIndex = getLootableObjectIndex(id);
			if(lootIndex < 0) return null;
			LootableObject loot = lootableObjects[lootIndex];
			Item item = loot.getItem();
			loot.Inactivate();
			lootableObjects.RemoveAt(lootIndex);
			return item;
		}

		return null;
	}

	public int getLootableObjectIndex(int id)
	{
		for(int i = 0; i < lootableObjects.Count; i++)
		{
			if(lootableObjects[i].getID() == id) return i;
		}
		return -1;
	}

	public LootableObject getLootableObject(int id)
	{
		foreach(LootableObject loot in lootableObjects)
		{
			if(loot.getID() == id) return loot;
		}
		return null;
	}

	public void addLoot(LootableObject loot)
	{
		int id = 0;
		if(lootableObjects.Count > 0)
		{
			id = lootableObjects[lootableObjects.Count-1].getID() + 1;
		}
		loot.setID(id);
		lootableObjects.Add(loot);
	}

	public LootableObject getLootObject(Vector3 position)
	{
		foreach(LootableObject loot in lootableObjects)
		{
			if(loot.getPosition() == position) return loot;
		}
		return null;
	}

	public void addEyecandy(Eyecandy eyecandy)
	{
		eyecandyObjects.Add (eyecandy);
	}
	

	public void addUnit(Unit unit)
	{
		if(containsUnit(unit.getID())) return;
		units.Add(unit);
		if(active)
		{
			unit.activate();
		}
		else
		{
			unit.inactivate();
		}
	}


	public bool containsUnit(int id)
	{
		foreach(Unit unit in units)
		{
			if(unit.getID() == id) return true;
		}
		return false;
	}

	public bool containsUnits()
	{
		return units.Count > 0;
	}

	public void removeUnit(Unit unit)
	{
		units.Remove(unit);
	}

	public List<Unit> getUnits()
	{
		return units;
	}

	public bool blocksProjectile()
	{
		return (hasTileObject() && tileObject.blocksProjectile());
	}
}
