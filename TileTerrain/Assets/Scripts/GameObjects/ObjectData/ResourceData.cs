using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class ResourceData : ObjectData {
	public readonly int health;
	public readonly int damageType;

	[XmlArray("safeDrops")]
	public readonly string[] safeDrops;

	[XmlArray("randomDrops")]
	public readonly string[] randomDrops;

	public readonly int minDrops = 0;
	public readonly int maxDrops = 2;

	public readonly int variances = 1;

	public readonly string hitParticle;

	public readonly bool blocksProjectile;

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
}
