using UnityEngine;
using System.Collections;

public class LootableItem : LootableObject {

	int durability;

	public LootableItem(Vector3 position, Quaternion rotation, string lootType) : base(position, rotation, lootType)
	{

	}

	public override Item getItem()
	{
		return new CraftedItem(getName());
	}
}
