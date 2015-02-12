using UnityEngine;
using System.Collections;

public class LootableConsumable : LootableObject {

	public LootableConsumable(Vector3 position, Quaternion rotation, string lootType) : base(position, rotation, lootType)
	{
		
	}
	
	public override Item getItem()
	{
		return new ConsumableItem(getName());
	}
}
