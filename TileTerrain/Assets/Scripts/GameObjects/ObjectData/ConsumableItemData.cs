using UnityEngine;
using System.Collections;
using Edit;

public class ConsumableItemData : ItemData {

	public readonly int hungerChange;

    public ConsumableItemData()
    { 
    }

    public ConsumableItemData(ConsumableEdit edit)
        : base(edit)
    {
        hungerChange = edit.hungerChange;
    }

	public override LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableConsumable(position, rotation, name, modelName);
	}

    public override LootableObject getLootableObject(Vector2 position2D, Quaternion rotation)
    {
        return new LootableConsumable(position2D, rotation, name, modelName);
    }

    public override LootableObject getLootableObject(Vector2 position2D, float yrotation)
    {
        return new LootableConsumable(position2D, yrotation, name, modelName);
    }

    public override string getTooltipStatsString()
    {
        return "Hunger change: " + hungerChange;
    }
}
