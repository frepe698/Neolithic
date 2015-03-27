using UnityEngine;
using System.Collections;

public class EquipmentItem : Item {

    private EquipmentData data;
    private bool equiped = false;

	public EquipmentItem(string name) :base(name)
	{
        data = DataHolder.Instance.getEquipmentData(name);
	}

	public override string getGameName()
	{
        if(data == null)
        {
            Debug.LogWarning(getName() + " do not have any data");
            return null;
        }
		return data.gameName;
	}

	public override string getInventoryDisplay()
	{
        if (equiped) return ">>" + getGameName();
		return getGameName();
	}

    public override ItemData getData()
    {
        return data;
    }

    public void setEquiped(bool equiped)
    {
        this.equiped = equiped;
    }

    public bool isEquiped()
    {
        return equiped;
    }

    public override bool canBeDropped()
    {
        return !isEquiped();
    }

}
