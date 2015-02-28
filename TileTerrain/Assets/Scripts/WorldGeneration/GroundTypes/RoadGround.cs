using UnityEngine;
using System.Collections;

public class RoadGround : GroundType {

	private static readonly string[] startingLootTypes = new string[]
	{
		"stick",
	    "stone",
	};

	public override ResourceObject getRandomResource(Vector3 position)
	{
		return null;
	}
	
	public override ItemData getRandomLoot(Vector3 position)
	{
		int lootType = Random.Range(0, startingLootTypes.Length);
		return DataHolder.Instance.getItemData(startingLootTypes[lootType]);
//		return new LootableObject(position,
//		                          Quaternion.Euler(0, Random.value*360, 0),
//		                          startingLootTypes[lootType]);
	}
	
	public override EyecandyObject getRandomEyecandy(Vector3 position)
	{
		return null;
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_ROAD;
	}

	public override bool spawnResource ()
	{
		return false;
	}
	
	public override int spawnLootAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.99)
		{
			lootCount++;
			if(rand > 0.99)
			{
				lootCount++;
			}
		}
		return lootCount;
	}
	
	public override int spawnEyecandyAmount ()
	{
		return 0;
	}
	
}
