using UnityEngine;
using System.Collections;

public class LootableConsumable : LootableObject {

	public LootableConsumable(Vector3 position, Quaternion rotation, string name, string poolName)
        : base(position, rotation, name, poolName)
	{
		
	}

    public LootableConsumable(Vector2 position2D, Quaternion rotation, string name, string poolName)
         : base(position2D, rotation, name, poolName)
    {

    }

    public LootableConsumable(Vector2 position2D, float yrotation, string name, string poolName)
        : base(position2D, yrotation, name, poolName)
    {

    }
	
	public override Item getItem()
	{
		return new ConsumableItem(getName());
	}
}
