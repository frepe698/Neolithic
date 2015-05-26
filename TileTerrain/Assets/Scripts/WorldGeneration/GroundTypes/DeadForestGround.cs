﻿using UnityEngine;
using System.Collections;

public class DeadForestGround : GroundType {

	private static readonly string[] resObjects = new string[]
	{
		"deadTree",
		"deadBush",
		"deadBush",
		"stone",
	};

	private static readonly string[] startingLootTypes = new string[]
	{
		"stone",
		"stick",
		"stick",
		"stick",
	};
	
	public override ResourceObject getRandomResource(Vector3 position)
	{
		return new ResourceObject(position,Random.value*360, resObjects[Random.Range(0, resObjects.Length)]);
	}

	public override bool spawnResource ()
	{
		return (Random.value > 0.95f);
	}
	
	public override int spawnLootAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.98)
		{
			lootCount++;
			if(rand > 0.995)
			{
				lootCount++;
				if(rand > 0.998)
				{
					lootCount++;
				}
			}
		}
		return lootCount;
	}
	
	public override int spawnEyecandyAmount ()
	{
		return 0;
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
		return null;
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_DRYMUD;
	}
	
}
