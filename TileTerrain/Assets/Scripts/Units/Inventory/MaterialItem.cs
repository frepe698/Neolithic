using UnityEngine;
using System.Collections;

public class MaterialItem : Item {

	private int amount;

	public MaterialItem(string name) :base(name)
	{
		
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
        MaterialData data = DataHolder.Instance.getMaterialData(getName());
        if (data == null)
        {
            Debug.LogError("Could not find material data for " + getName());
            return null;
        }
		return data.gameName;
	}

	public override string getInventoryDisplay()
	{
		return getGameName() + "["+amount+"]";
	}
}
