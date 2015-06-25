using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class HarvestableData : ResourceData {

    public readonly string harvestDrop;
    public readonly int minHarvest;
    public readonly int maxHarvest;
    public readonly float minRespawnTime;
    public readonly float maxRespawnTime;

    private Texture2D fullTexture;
    private Texture2D emptyTexture;

    public HarvestableData()
    { 
    }

    public HarvestableData(HarvestableEdit data)
            : base(data)
    {
        harvestDrop = data.harvestDrop;
        minHarvest = Mathf.Max(Mathf.Min(data.minHarvest, data.maxHarvest), 0);
        maxHarvest = Mathf.Max(data.maxHarvest, data.minHarvest);
        minRespawnTime = Mathf.Max(Mathf.Min(data.minRespawnTime, data.minRespawnTime), 0);
        maxRespawnTime = Mathf.Max(data.maxRespawnTime, data.minRespawnTime);
    }

    public void initTextures()
    {
        fullTexture = Resources.Load<Texture2D>("Harvestable/Textures/" + modelName + "_full");
        emptyTexture = Resources.Load<Texture2D>("Harvestable/Textures/" + modelName + "_empty");
        if (fullTexture == null)
        {
            Debug.LogError("couldnt load full texture for " + name);
            fullTexture = emptyTexture;
        }
        if (emptyTexture == null)
        {
            Debug.LogError("couldnt load empty texture for " + name);
            emptyTexture = fullTexture;
        }
    }

    public Texture2D getTexture(bool full)
    {
        return full ? fullTexture : emptyTexture;
    }

    public override ResourceObject getResourceObject(Vector3 position, float rotation)
    {
        return new HarvestableObject(position, rotation, this);
    }

}
