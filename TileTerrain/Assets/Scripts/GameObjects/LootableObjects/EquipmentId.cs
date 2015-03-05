using UnityEngine;
using System.Collections;

public class EquipmentId : LootId {

	int durability;

	void setDurability(int durability)
	{
		this.durability = durability;
	}

	int getDurability()
	{
		return durability;
	}

	public override Item getItem(string item)
	{
		return new EquipmentItem(item);
	}
}
