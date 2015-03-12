using UnityEngine;
using System.Collections;

public class WaterGround : GroundType {
	
	private static readonly string[] eyecandyTypes = new string[]
	{
		"waterlily01",
	};
	
	public override ResourceObject getRandomResource(Vector3 position)
	{
        return new ResourceObject(position, Random.value * 360, "stone");
	}
	
	public override bool spawnResource ()
	{
		return Random.value > 0.95f;
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
	
	public override Eyecandy getRandomEyecandy(Vector3 position)
	{

        if (Mathf.PerlinNoise(position.x / 2, position.z / 2) * 4 < 1)
        {
            return new Eyecandy(new Vector3(position.x, 0.5f, position.z),
                                  Quaternion.Euler(0, Random.value * 360, 0),
                                  eyecandyTypes[0]);
        }
        return null;
		
	}
	
	public override int getTexture()
	{
		return GroundType.TEX_SAND;
	}
	
}
