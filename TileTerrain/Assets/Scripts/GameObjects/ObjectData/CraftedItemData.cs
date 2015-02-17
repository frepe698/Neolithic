using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public abstract class CraftedItemData : ItemData {

	public readonly int durability;

    public CraftedItemData()
    { 
    }

    public CraftedItemData(CraftedEdit edit) : base(edit)
    {
        durability = edit.durability;
    }

	public override LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableItem(position, rotation, name);
	}
}
