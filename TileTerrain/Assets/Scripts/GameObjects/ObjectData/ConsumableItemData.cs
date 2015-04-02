using UnityEngine;
using System.Collections;
using Edit;
using System.Xml.Serialization;

public class ConsumableItemData : ItemData {

    [XmlArray("hitBuffs"), XmlArrayItem("HitBuff")]
    public readonly HitBuff[] hitBuffs;

	public readonly int hungerChange;

    public ConsumableItemData()
    { 
    }

    public ConsumableItemData(ConsumableEdit edit)
        : base(edit)
    {
        hungerChange = edit.hungerChange;

        hitBuffs = new HitBuff[edit.hitBuffs.Count];
        for (int i = 0; i < edit.hitBuffs.Count; i++)
        {
            hitBuffs[i] = new HitBuff(edit.hitBuffs[i]);
        }
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

    public override Item getItem()
    {
        return new ConsumableItem(name);
    }
}
