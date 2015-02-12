using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

public class CraftedItemData : ItemData {

	public readonly int durability;
	

	
	public override LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableItem(position, rotation, name);
	}
}
