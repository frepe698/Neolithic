using UnityEngine;
using System.Collections;

public class HarvestableObject : ResourceObject {

    private HarvestableData harvestData;

    private int harvestAmount;

	public HarvestableObject(Vector3 position, float rotation, string name)
        : base(position, rotation)
	{
		this.position = position;
		this.rotation = rotation;
        harvestData = DataHolder.Instance.getHarvestableData(name);
        data = harvestData;
        randomizeHarvestAmount();
        this.poolName = harvestData.modelName;
        this.health = data.health;
        this.blocks = data.blocksProjectile;
	}

    public HarvestableObject(Vector3 position, float rotation, HarvestableData data)
        : base(position, rotation)
    {
        this.position = position;
        this.rotation = rotation;
        harvestData = data;
        this.data = harvestData;
        randomizeHarvestAmount();
        this.poolName = harvestData.modelName;
        this.health = data.health;
        this.blocks = data.blocksProjectile;
        
    }

    public override bool Activate()
    {
        if (base.Activate())
        {
            updateTexture(canBeHarvested());
            return true;
        }
        return false;
    }

    public override bool Equals(object o)
    {
        HarvestableObject other = o as HarvestableObject;
        if (other == null) return false;
        return other.get2DPos() == this.get2DPos();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
	
	public string getHarvestDrop()
	{
		return harvestData.harvestDrop;
	}

    public override string harvest()
    {
        if (harvestAmount > 0)
        {
            harvestAmount--;
            if (harvestAmount == 0)
            {
                updateTexture(false);
            }
            return harvestData.harvestDrop;
        }
        return null;
    }

    public void regrow()
    {
        randomizeHarvestAmount();
        updateTexture(true);
    }

    private void randomizeHarvestAmount()
    {
        harvestAmount = Random.Range(harvestData.minHarvest, harvestData.maxHarvest);
    }

    public float getRegrowTime()
    {
        return Random.Range(harvestData.minRespawnTime, harvestData.maxRespawnTime);
    }

    public override bool canBeHarvested()
    {
        return harvestAmount > 0;
    }

    private void updateTexture(bool full)
    {
        if (ObjectActive())
        {
            Texture2D texture = harvestData.getTexture(full);
            if (texture == null)
                Debug.LogError("Texture is null");
            getObject().GetComponent<Renderer>().material.mainTexture = harvestData.getTexture(full);
        }
    }
}
