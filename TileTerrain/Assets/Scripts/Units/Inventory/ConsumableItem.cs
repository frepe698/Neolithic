using UnityEngine;
using System.Collections;

public class ConsumableItem : Item {

    private ConsumableItemData data;
	private int amount;

	public ConsumableItem(string name) : base(name)
	{
        data = DataHolder.Instance.getConsumableItemData(name);
	}

	public int getHungerChange()
	{
		return data.hungerChange;
	}

	public override int getAmount()
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
		return data.gameName;
	}

	public override string getInventoryDisplay()
	{
		return getGameName() + "["+amount+"]";
	}

    public override ItemData getData()
    {
        return data;
    }
}
