using UnityEngine;
using System.Collections;

public class LootableItem : LootableObject {

	int durability;

	public LootableItem(Vector3 position, Quaternion rotation, string name, string poolName) : base(position, rotation, name, poolName)
	{

	}

	public override Item getItem()
	{
		return new CraftedItem(getName());
	}
}
