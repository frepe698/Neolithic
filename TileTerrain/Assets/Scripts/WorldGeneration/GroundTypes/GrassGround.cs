﻿using UnityEngine;
using System.Collections;

public class GrassGround : GroundType {

	private static readonly string[] resObjects = new string[]
	{
		"greenTree",
        "greenBush",
        "stone",
	};
	
	private static readonly string[] startingLootTypes = new string[]
	{
		"stone",
		"stick",
		"stick",
	};
	
	private static readonly string[] eyecandyTypes = new string[]
	{
		"grass01",
	};

	private static readonly string[] engTypes = new string[]
	{
		"eng01",
		"eng02",
	};
	
	public override ResourceObject getRandomResource(Vector3 position)
	{
		return new ResourceObject(position,Random.value*360, resObjects[Random.Range(0, resObjects.Length)]);
	}

	public override bool spawnResource ()
	{
		return (Random.value > 0.985f);
	}
	
	public override int spawnLootAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.96)
		{
			lootCount++;
			if(rand > 0.98)
			{
				lootCount++;
			}
		}
		return lootCount;
	}
	
	public override int spawnEyecandyAmount ()
	{
		float rand = Random.value;
		int lootCount = 0;
		if(rand > 0.5)
		{
			lootCount++;
			if(rand > 0.9)
			{
				lootCount++;
//				if(rand > 0.8)
//				{
//					lootCount++;
//					if(rand > 0.9) lootCount++;
//				}
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
	
	public override EyecandyObject getRandomEyecandy(Vector3 position)
	{
		string candyName;

		if(outsideForest(new Vector2i(position.x, position.z)))
		{
			int lootType = Mathf.RoundToInt(Mathf.PerlinNoise(position.x/20 + 100, position.z/20 + 100));
			if(lootType == 0)
			{
				//candyName = eyecandyTypes[Random.Range(0, eyecandyTypes.Length)];
                return null;
			}
			else
			{
				candyName = "fernCluster02";
			}
		}
		else
		{
			int lootType = Mathf.FloorToInt(Mathf.PerlinNoise(position.x/5, position.z/5) * 4);
			if(lootType > 1)
			{
				return null;
			}
			if(lootType == 0)
			{
				candyName = eyecandyTypes[Random.Range(0, eyecandyTypes.Length)];
			}
			else
			{
				candyName = engTypes[Random.Range(0, engTypes.Length)];
			}
		}
		
		return new EyecandyObject(position,
		                          Quaternion.Euler(0, Random.value*360, 0),
		                          candyName);
	}

	private bool outsideForest(Vector2i tile)
	{
		TileMap map = World.getMap();
		for(int x = tile.x - 2; x < tile.x + 3; x++)
		{
			for(int y = tile.y - 2; y < tile.y + 3; y++)
			{
				if(map.isValidTile(x, y) && map.getTile(x, y).ground == (short)GroundType.Type.PineForest)
				{
					return true;
				}
			}
		}
		return false;
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_GRASS;
	}
	
}
