using UnityEngine;
using System.Collections;

public class WaterGround : GroundType {
	
	private static readonly string[] eyecandyTypes = new string[]
	{
		"waterlily01",
	};
	
	public override ResourceObject getRandomResource(Vector3 position)
	{
		return null;
	}
	
	public override bool spawnResource ()
	{
		return false;
	}
	
	public override int spawnLootAmount ()
	{
		return 0;
	}
	
	public override int spawnEyecandyAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.7)
		{
			lootCount++;
			if(rand > 0.95)
			{
				lootCount++;
			}
		}
		return lootCount;
	}
	
	public override ItemData getRandomLoot(Vector3 position)
	{
		return null;
	}
	
	public override EyecandyObject getRandomEyecandy(Vector3 position)
	{
		int lootType = Random.Range(0, eyecandyTypes.Length);
		
		return new EyecandyObject(new Vector3(position.x, 0.5f, position.z),
		                          Quaternion.Euler(0, Random.value*360, 0),
		                          eyecandyTypes[lootType]);
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_SAND;
	}
	
}
