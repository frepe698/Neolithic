using UnityEngine;
using System.Collections;

public class LootableItem : LootableObject {

	int durability;

	public LootableItem(Vector3 position, Quaternion rotation, string name, string poolName) : base(position, rotation, name, poolName)
	{

	}

    public LootableItem(Vector2 position2D, Quaternion rotation, string name, string poolName)
        : base(position2D, rotation, name, poolName)
    {

    }

    public LootableItem(Vector2 position2D, float yrotation, string name, string poolName)
        : base(position2D, yrotation, name, poolName)
    {

    }

	public override Item getItem()
	{
		return new CraftedItem(getName());
	}
}
