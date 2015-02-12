using UnityEngine;
using System.Collections;

public class ItemId : LootId {

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
		return new CraftedItem(item);
	}
}
