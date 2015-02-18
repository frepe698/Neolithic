using UnityEngine;
using System.Collections;

public class CraftedItem : Item {

	public CraftedItem(string name) :base(name)
	{

	}

	public override string getGameName()
	{
        CraftedItemData data = DataHolder.Instance.getCraftedItemData(getName());
        if(data == null)
        {
            Debug.LogWarning(getName() + " do not have any data");
            return null;
        }
		return data.gameName;
	}

	public override string getInventoryDisplay()
	{
		return getGameName();
	}
}
