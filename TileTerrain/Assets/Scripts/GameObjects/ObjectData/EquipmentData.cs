using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public abstract class EquipmentData : ItemData {

	public readonly int durability;

    public EquipmentData()
    { 
    }

    public EquipmentData(CraftedEdit edit) : base(edit)
    {
        durability = edit.durability;
    }

	public override LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableEquipment(position, rotation, name, modelName);
	}

    public override LootableObject getLootableObject(Vector2 position2D, Quaternion rotation)
    {
        return new LootableEquipment(position2D, rotation, name, modelName);
    }

    public override LootableObject getLootableObject(Vector2 position2D, float yrotation)
    {
        return new LootableEquipment(position2D, yrotation, name, modelName);
    }

    public override Item getItem()
    {
        return new EquipmentItem(name);
    }
}
