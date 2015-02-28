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

    public override string getTooltipStatsString()
    {
        return "Hunger change: " + hungerChange;
    }
}
