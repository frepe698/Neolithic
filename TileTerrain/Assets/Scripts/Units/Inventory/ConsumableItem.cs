using UnityEngine;
using System.Collections;

public class ConsumableItem : Item {

	private int amount;

	public ConsumableItem(string name) : base(name)
	{

	}

	public int getHungerChange()
	{
		return DataHolder.Instance.getConsumableItemData(getName()).hungerChange;
	}

	public int getAmount()
	{
		return amount;
	}
	
	public void add(int count = 1)
	{
		amount += count;
	}
	
	/**
	 * 	if this returns true, remove from list!
	 **/
	public bool remove(int count = 1)
	{
		amount-=count;
		if(amount <= 0) return true;
		return false;
	}

	public override string getGameName()
	{
		return DataHolder.Instance.getConsumableItemData(getName()).gameName;
	}

	public override string getInventoryDisplay()
	{
		return getGameName() + "["+amount+"]";
	}
}
