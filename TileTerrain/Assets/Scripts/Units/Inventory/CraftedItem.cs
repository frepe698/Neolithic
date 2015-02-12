using UnityEngine;
using System.Collections;

public class CraftedItem : Item {

	public CraftedItem(string name) :base(name)
	{

	}

	public override string getGameName()
	{
		return DataHolder.Instance.getCraftedItemData(getName()).gameName;
	}

	public override string getInventoryDisplay()
	{
		return getGameName();
	}
}
