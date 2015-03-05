using UnityEngine;
using System.Collections;

public abstract class QuickUseItem {

    public int itemIndex;
    public string itemName;

    public QuickUseItem()
    {
        this.itemIndex = -1;
        itemName = "";
    }
    public QuickUseItem(int itemIndex)
    {
        this.itemIndex = itemIndex;
    }

    public virtual void onRemoveItem(ItemType type, int index)
    {
    }

    //returns if it added the item
    public virtual bool onAddItem(ItemType type, int index, Inventory inventory)
    {
        return false;
    }


    public abstract Item getItem(Inventory inventory);
}

public class QuickUseEquipment : QuickUseItem
{
    public QuickUseEquipment()
        : base()
    { 
    }
    public QuickUseEquipment(int itemIndex, Inventory inventory)
        : base(itemIndex)
    {
        itemName = inventory.getEquipmentItems()[itemIndex].getName();
    }

    public override void onRemoveItem(ItemType type, int index)
    {
        if (type != ItemType.Equipment) return;
        if (itemIndex == index)
        {
            itemIndex = -1;
        }
        if (index < itemIndex)
        {
            itemIndex--;
        }
    }

    public override bool onAddItem(ItemType type, int index, Inventory inventory)
    {
        if (type != ItemType.Equipment || itemIndex > -1) return false;
        //if (inventory.getEquipmentItems()[index].getName().Equals(itemName)) itemIndex = index;
        itemIndex = index;
        return true;
    }

    public override Item getItem(Inventory inventory)
    {
        if (itemIndex < 0 || itemIndex >= inventory.getEquipmentItems().Count) return null;
        return inventory.getEquipmentItems()[itemIndex];
    }
}

public class QuickUseMaterial : QuickUseItem
{
    public QuickUseMaterial()
        : base()
    { 
    }
    public QuickUseMaterial(int itemIndex, Inventory inventory)
        : base(itemIndex)
    {
        itemName = inventory.getMaterialItems()[itemIndex].getName();
    }

    public override void onRemoveItem(ItemType type, int index)
    {
        if (type != ItemType.Material) return;
        if (itemIndex == index)
        {
            itemIndex = -1;
        }
        if (index < itemIndex)
        {
            itemIndex--;
        }
    }

    public override bool onAddItem(ItemType type, int index, Inventory inventory)
    {
        if (type != ItemType.Material || itemIndex > -1) return false;
        if (inventory.getMaterialItems()[index].getName().Equals(itemName))
        {
            itemIndex = index;
            return true;
        }
        return false;
    }

    public override Item getItem(Inventory inventory)
    {
        if (itemIndex < 0 || itemIndex >= inventory.getMaterialItems().Count) return null;
        return inventory.getMaterialItems()[itemIndex];
    }
}

public class QuickUseConsumable : QuickUseItem
{
    public QuickUseConsumable()
        : base()
    { 
    }
    public QuickUseConsumable(int itemIndex, Inventory inventory)
        : base(itemIndex)
    {
        itemName = inventory.getConsumableItems()[itemIndex].getName();
    }

    public override void onRemoveItem(ItemType type, int index)
    {
        if (type != ItemType.Consumable) return;
        if (itemIndex == index)
        {
            itemIndex = -1;
        }
        if (index < itemIndex)
        {
            itemIndex--;
        }
    }

    public override bool onAddItem(ItemType type, int index, Inventory inventory)
    {
        if (type != ItemType.Consumable || itemIndex > -1) return false;
        if (inventory.getConsumableItems()[index].getName().Equals(itemName))
        {
            itemIndex = index;
            return true;
        }
        return false;
    }

    public override Item getItem(Inventory inventory)
    {
        if(itemIndex < 0 || itemIndex >= inventory.getConsumableItems().Count) return null;
        return inventory.getConsumableItems()[itemIndex];
    }
}

