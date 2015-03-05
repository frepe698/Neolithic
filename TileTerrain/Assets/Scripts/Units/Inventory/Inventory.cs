using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void ChangedEventHandler(object sender, ItemArgs e);


public enum ItemType
{
    Equipment = 0,
    Material = 1,
    Consumable = 2,
}

public class ItemArgs : EventArgs
{
    public ItemType itemType {get; set;}
    public int itemIndex {get; set;}
}

public class Inventory {

	private bool hasBeenChanged = false;

	private List<EquipmentItem> equipmentItems;
	private List<ConsumableItem> consumableItems;
	private List<MaterialItem> materialItems;

    private EquipmentItem equipedWeapon;
    private EquipmentItem[] equipedArmor = new EquipmentItem[3];

    public event ChangedEventHandler addedItemListener;
    public event ChangedEventHandler removedItemListener;
    public event EventHandler changeItemListener;

	public Inventory()
	{
		equipmentItems = new List<EquipmentItem>();
		consumableItems = new List<ConsumableItem>();
		materialItems = new List<MaterialItem>();
	}

    private void onAddedItem(ItemArgs e)
    {
        if (addedItemListener != null)
            addedItemListener(this, e);
    }

    private void onRemovedItem(ItemArgs e)
    {
        if (removedItemListener != null)
            removedItemListener(this, e);
    }

    private void onChangeItem(EventArgs e)
    {
        if (changeItemListener != null)
            changeItemListener(this, e);
    }

	public void addItem(Item item)
	{
		if(item is EquipmentItem)
		{
			equipmentItems.Add((EquipmentItem)item);
            ItemArgs args = new ItemArgs();
            args.itemType = ItemType.Equipment;
            args.itemIndex = equipmentItems.Count - 1;
            onAddedItem(args);
		}
		else if(item is MaterialItem)
		{
            ItemArgs args = new ItemArgs();
            args.itemType = ItemType.Material;

			MaterialItem listResItem = findMaterialItem((MaterialItem)item);
			if(listResItem != null)
			{
				listResItem.add();                
                args.itemIndex = materialItems.IndexOf(listResItem);
			}
			else
			{
				MaterialItem newResItem = (MaterialItem)item;
				materialItems.Add(newResItem);
                args.itemIndex = materialItems.IndexOf(newResItem);
				newResItem.add();
			}
            onAddedItem(args);
		}
		else
		{
            ItemArgs args = new ItemArgs();
            args.itemType = ItemType.Consumable;

			ConsumableItem listConItem = findConsumableItem((ConsumableItem)item);
			if(listConItem != null)
			{
				listConItem.add();
                args.itemIndex = consumableItems.IndexOf(listConItem);
			}
			else
			{
				ConsumableItem newConItem = (ConsumableItem)item;
				consumableItems.Add(newConItem);
                args.itemIndex = consumableItems.IndexOf(newConItem);
				newConItem.add();
			}
            onAddedItem(args);
		}
		hasBeenChanged = true;
	}

	public bool craftItem(RecipeData recipe)
	{
		foreach(Ingredient i in recipe.ingredients)
		{
			MaterialItem mi = findMaterialItem(i.name);
            if (mi != null && mi.getAmount() >= i.amount)
            {
                removeMaterialItem(materialItems.IndexOf(mi), i.amount);
                continue;
            }

            ConsumableItem ci = findConsumableItem(i.name);
            if (ci != null && ci.getAmount() >= i.amount)
            {
                removeConsumableItem(consumableItems.IndexOf(ci), i.amount);
                continue;
            }
            return false;
		}
		addItem(recipe.getCraftedItem());
        return true;
	}

    public bool setEquipedItem(int index, out EquipmentData data)
    {
        if (index < 0 || equipmentItems.Count <= index)
        {
            data = null;
            return false;
        }
        EquipmentItem item = equipmentItems[index];
        data = (EquipmentData)item.getData();

        if (data is WeaponData)
        {
            if(equipedWeapon != null) equipedWeapon.setEquiped(false);

            if (equipedWeapon == item)
            {
                data = DataHolder.Instance.getWeaponData("unarmed");
                equipedWeapon = null;
                hasBeenChanged = true;
                onChangeItem(EventArgs.Empty);
                return true;
            }
            equipedWeapon = item;
            equipedWeapon.setEquiped(true);
        }
        else if (data is ArmorData)
        {
            int type = ((ArmorData)data).armorType;
            if(equipedArmor[type] != null) equipedArmor[type].setEquiped(false);

            if (equipedArmor[type] == item)
            {
                equipedArmor[type] = null;
                hasBeenChanged = true;
                onChangeItem(EventArgs.Empty);
                return false;
            }
            equipedArmor[type] = item;
            equipedArmor[type].setEquiped(true);
        }

        hasBeenChanged = true;
        onChangeItem(EventArgs.Empty);
        return true;
    }

    private void removeEquipmentItem(int index)
    {
        equipmentItems.RemoveAt(index);
        ItemArgs args = new ItemArgs();
        args.itemType = ItemType.Equipment;
        args.itemIndex = index;
        onRemovedItem(args);
    }

    private void removeMaterialItem(int index, int amount = 1)
    {
        if (materialItems[index].remove(amount))
        {
            materialItems.RemoveAt(index);
            ItemArgs args = new ItemArgs();
            args.itemType = ItemType.Material;
            args.itemIndex = index;
            onRemovedItem(args);
        }
        onChangeItem(EventArgs.Empty);
    }

    private void removeConsumableItem(int index, int amount = 1)
    {
        if (consumableItems[index].remove(amount))
        {
            consumableItems.RemoveAt(index);
            ItemArgs args = new ItemArgs();
            args.itemType = ItemType.Consumable;
            args.itemIndex = index;
            onRemovedItem(args);
        }
        onChangeItem(EventArgs.Empty);
    }

	public void consumeItem(int index)
	{
        removeConsumableItem(index);
		hasBeenChanged = true;
	}

	public MaterialItem findMaterialItem(MaterialItem item)
	{
		foreach(MaterialItem ri in materialItems)
		{
			if(ri.getName().Equals(item.getName()))
			{
				return ri;
			}
		}
		return null;
	}

	public MaterialItem findMaterialItem(string name)
	{
		foreach(MaterialItem ri in materialItems)
		{
			if(ri.getName().Equals(name))
			{
				return ri;
			}
		}
		return null;
	}

	public ConsumableItem findConsumableItem(ConsumableItem item)
	{
		foreach(ConsumableItem ri in consumableItems)
		{
			if(ri.getName().Equals(item.getName()))
			{
				return ri;
			}
		}
		return null;
	}
	
	public ConsumableItem findConsumableItem(string name)
	{
		foreach(ConsumableItem ri in consumableItems)
		{
			if(ri.getName().Equals(name))
			{
				return ri;
			}
		}
		return null;
	}

    public Item getItem(int index)
    {
        int itemIndex = index;
        if (itemIndex < equipmentItems.Count)
        {
            return equipmentItems[itemIndex];
        }
        itemIndex -= equipmentItems.Count;

        if (itemIndex < materialItems.Count)
        {
            return materialItems[itemIndex];
        }
        itemIndex -= materialItems.Count;

        if (itemIndex < consumableItems.Count)
        {
            return consumableItems[itemIndex];
        }
        itemIndex -= consumableItems.Count;

        return null;
    }

	public void drawGUI()
	{
		GUI.Box(new Rect(100, 100, 400, 400), "Inventory");
		float yDrawPos = 110;
		float lineHeight = 20;
		GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), "Man Made Items");
		yDrawPos += lineHeight;
		for(int i = 0; i < equipmentItems.Count; i++)
		{
			GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), equipmentItems[i].getName());
			yDrawPos += lineHeight;
		}

		for(int i = 0; i < materialItems.Count; i++)
		{
			GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), materialItems[i].getName() + " x"+materialItems[i].getAmount());
			yDrawPos += lineHeight;
		}
	}


	public List<EquipmentItem> getEquipmentItems()
	{
		return equipmentItems;
	}

	public List<MaterialItem> getMaterialItems()
	{
		return materialItems;
	}

	public List<ConsumableItem> getConsumableItems()
	{
		return consumableItems;
	}

	public bool shouldUpdate()
	{
		bool result = hasBeenChanged;
		hasBeenChanged = false;
		return result;
	}

	public bool hasIngredients(Ingredient[] ingredients)
	{
		foreach(Ingredient i in ingredients)
		{
			MaterialItem mi = findMaterialItem(i.name);
            if (mi == null || mi.getAmount() < i.amount)
            {
                ConsumableItem ci = findConsumableItem(i.name);
                if (ci == null || ci.getAmount() < i.amount)
                {
                    return false;
                }
            }
		}

		return true;
	}

}
