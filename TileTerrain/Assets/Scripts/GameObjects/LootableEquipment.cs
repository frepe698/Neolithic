using UnityEngine;
using System.Collections;

public class LootableEquipment : LootableObject {

	int durability;

	public LootableEquipment(Vector3 position, Quaternion rotation, string name, string poolName) : base(position, rotation, name, poolName)
	{

	}

    public LootableEquipment(Vector2 position2D, Quaternion rotation, string name, string poolName)
        : base(position2D, rotation, name, poolName)
    {

    }

    public LootableEquipment(Vector2 position2D, float yrotation, string name, string poolName)
        : base(position2D, yrotation, name, poolName)
    {

    }

	public override Item getItem()
	{
		return new EquipmentItem(getName());
	}
}
