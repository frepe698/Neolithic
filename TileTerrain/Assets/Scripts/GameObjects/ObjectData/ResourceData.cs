using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class ResourceData : ObjectData {
	public readonly int health;
	public readonly int damageType;

	[XmlArray("safeDrops")]
	public readonly string[] safeDrops;

	[XmlArray("randomDrops")]
	public readonly string[] randomDrops;

    [XmlArray("rareDrops")]
    public readonly string[] rareDrops;

	public readonly int minDrops = 0;
	public readonly int maxDrops = 2;

	public readonly int variances = 1;

    [XmlElement(IsNullable = false)]
	public readonly string hitParticle;

	public readonly bool blocksProjectile;

    public ResourceData()
    { 
    }

    public ResourceData(ResourceEdit edit)
        : base(edit)
    {
        health = edit.health;
        damageType = DamageType.damageTypeToInt(edit.damageType);

        safeDrops = edit.safeDrops.Trim().Split('\n');
        randomDrops = edit.randomDrops.Trim().Split('\n');
        rareDrops = edit.rareDrops.Trim().Split('\n');
        minDrops = edit.minDrops;
        maxDrops = edit.maxDrops;

        variances = edit.variances;
        if(edit.hitParticle != null && !edit.hitParticle.Trim().Equals(""))hitParticle = edit.hitParticle;
        blocksProjectile = edit.blocksProjectile;
    }

	public void debug()
	{
		string dropsString = ", drops: ";
		for(int i = 0; i < safeDrops.Length; i++)
		{
			dropsString += safeDrops[i] + ", ";
		}
		Debug.Log (gameName + " hp: " + health + dropsString);
	}

	public string getRandomVariance()
	{
		return Random.Range(1, variances+1).ToString().PadLeft(2, '0');
	}

    public virtual ResourceObject getResourceObject(Vector3 position, float rotation)
    {
        return new ResourceObject(position, rotation, this);
    }

}
