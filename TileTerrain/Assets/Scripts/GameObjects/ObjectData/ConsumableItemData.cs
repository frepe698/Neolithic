using UnityEngine;
using System.Collections;

public class ConsumableItemData : ItemData {

	public readonly int hungerChange;

	public override LootableObject getLootableObject(Vector3 position, Quaternion rotation)
	{
		return new LootableConsumable(position, rotation, name, modelName);
	}

    public override string getTooltipStatsString()
    {
        return "Hunger change: " + hungerChange;
    }
}
