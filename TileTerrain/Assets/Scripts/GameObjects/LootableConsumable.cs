using UnityEngine;
using System.Collections;

public class LootableConsumable : LootableObject {

	public LootableConsumable(Vector3 position, Quaternion rotation, string name, string poolName) : base(position, rotation, name, poolName)
	{
		
	}
	
	public override Item getItem()
	{
		return new ConsumableItem(getName());
	}
}
