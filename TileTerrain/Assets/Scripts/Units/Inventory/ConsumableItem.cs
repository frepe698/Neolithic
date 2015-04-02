using UnityEngine;
using System.Collections;
using System.Reflection;

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

    public HitBuff[] getBuffs()
    {
        return data.hitBuffs;
    }

    public void applyBuffs(Unit unit)
    {
        if (data != null)
        {
            HitBuff[] buffs = data.hitBuffs;
            for (int i = 0; i < buffs.Length; i++)
            {
                HitBuff hbuff = buffs[i];
                object[] parameters = new object[] { hbuff.stat, hbuff.duration, hbuff.amount, hbuff.percent };
                string name = "get" + hbuff.type.ToString();

                MethodInfo info = typeof(BuffGetter).GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                Buff buff = (Buff)info.Invoke(null, new object[] { parameters });
                buff.apply(unit);
            }
        }
    }
}
