using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GroundType {

	public static readonly int TEX_GRASS = 0;
	public static readonly int TEX_SNOW = 1;
	public static readonly int TEX_SAND = 2;
	public static readonly int TEX_ROAD = 3;
	public static readonly int TEX_ROCK = 4;
	public static readonly int TEX_PINEFOREST = 5;
	public static readonly int TEX_LEAFFOREST = 6;
	public static readonly int TEX_DRYMUD = 7;

//	public static readonly short GRASS = 1;
//	public static readonly short PINEFOREST = 2;
//	public static readonly short DEADFOREST = 3;
//	public static readonly short WATER = 4;
//	public static readonly short SHORE = 5;
//	public static readonly short BEACH = 6;
//	public static readonly short ROAD = 0;

	public enum Type
	{
		Grass,
		PineForest,
		DeadForest,
		Water,
		Shore,
		Beach,
		Road,
		Mountain,
		SnowMountain,
	}


	public abstract ResourceObject getRandomResource(Vector3 position);
	public abstract bool spawnResource();

	public abstract ItemData getRandomLoot(Vector3 position);
	public abstract int spawnLootAmount();
	public abstract EyecandyObject getRandomEyecandy(Vector3 position);
	public abstract int spawnEyecandyAmount();

	public abstract int getTexture();

}
