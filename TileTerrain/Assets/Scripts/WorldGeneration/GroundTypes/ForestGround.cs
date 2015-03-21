using UnityEngine;
using System.Collections;

public class ForestGround : GroundType {

	private static readonly string[] resObjects = new string[]
	{
		"gran",
		"gran",
		"stone",
		"bigTree",
        "greenBush"
	};

	private static readonly string[] startingLootTypes = new string[]
	{
		"stone",
		"stick",
		"deathcap",
		"cep",
		"puffball",
		"blueberrybush",
	};

	private static readonly string[] eyecandyTypes = new string[]
	{
		"fern01",
		"fern02",
        "moss01",
        "moss01",
	};

	public override ResourceObject getRandomResource(Vector3 position)
	{
		return new ResourceObject(position,Random.value*360, resObjects[Random.Range(0, resObjects.Length)]);
	}

	public override bool spawnResource ()
	{
		return (Random.value > 0.9f);
	}

	public override int spawnLootAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.9)
		{
			lootCount++;
			if(rand > 0.96)
			{
				lootCount++;
				if(rand > 0.99) lootCount++;
			}
		}
		return lootCount;
	}

	public override int spawnEyecandyAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.8)
		{
			lootCount++;
			if(rand > 0.96)
			{
				lootCount++;
				if(rand > 0.99) lootCount++;
			}
		}
		return lootCount;
	}
	
	public override ItemData getRandomLoot(Vector3 position)
	{
		int lootType = Random.Range(0, startingLootTypes.Length);
		return DataHolder.Instance.getItemData(startingLootTypes[lootType]);
		//		return new LootableObject(position,
		//		                          Quaternion.Euler(0, Random.value*360, 0),
		//		                          startingLootTypes[lootType]);
	}

	public override Eyecandy getRandomEyecandy(Vector3 position)
	{
		int lootType = Random.Range(0, eyecandyTypes.Length);
		
		return new Eyecandy(position,
		                          Quaternion.Euler(0, Random.value*360, 0),
		                          eyecandyTypes[lootType]);
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_PINEFOREST;
	}

}
