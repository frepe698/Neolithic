using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

	private bool hasBeenChanged = false;

	private List<CraftedItem> craftedItems;
	private List<ConsumableItem> consumableItems;
	private List<MaterialItem> materialItems;

	GameObject gui;

	public Inventory()
	{
		craftedItems = new List<CraftedItem>();
		consumableItems = new List<ConsumableItem>();
		materialItems = new List<MaterialItem>();
		initGUI();
	}

	private void initGUI()
	{
		gui = new GameObject("Inventory");
		gui.transform.SetParent(GameObject.Find("Canvas").transform);
	}

	public void addItem(Item item)
	{
		if(item is CraftedItem)
		{
			craftedItems.Add((CraftedItem)item);
		}
		else if(item is MaterialItem)
		{
			MaterialItem listResItem = findMaterialItem((MaterialItem)item);
			if(listResItem != null)
			{
				listResItem.add();
				Debug.Log ("added amount " + listResItem.getAmount());
			}
			else
			{
				MaterialItem newResItem = (MaterialItem)item;
				materialItems.Add(newResItem);
				newResItem.add();
			}
		}
		else
		{
			ConsumableItem listConItem = findConsumableItem((ConsumableItem)item);
			if(listConItem != null)
			{
				listConItem.add();
				Debug.Log ("added amount " + listConItem.getAmount());
			}
			else
			{
				ConsumableItem newConItem = (ConsumableItem)item;
				consumableItems.Add(newConItem);
				newConItem.add();
			}
		}
		hasBeenChanged = true;
	}

	public void craftItem(ItemRecipeData recipe)
	{
		foreach(Ingredient i in recipe.ingredients)
		{
			MaterialItem mi =findMaterialItem(i.name); 
			if(mi.remove(i.amount))
			{
				materialItems.Remove(mi);
			}
		}
		addItem(new CraftedItem(recipe.product));
	}

	public void craftMaterial(MaterialRecipeData recipe)
	{
		foreach(Ingredient i in recipe.ingredients)
		{
			MaterialItem mi =findMaterialItem(i.name); 
			if(mi.remove(i.amount))
			{
				materialItems.Remove(mi);
			}
		}
		addItem(new MaterialItem(recipe.product));
	}

	public void consumeItem(int index)
	{
		if(consumableItems[index].remove())
		{
			consumableItems.RemoveAt(index);
		}
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

	public void drawGUI()
	{
		GUI.Box(new Rect(100, 100, 400, 400), "Inventory");
		float yDrawPos = 110;
		float lineHeight = 20;
		GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), "Man Made Items");
		yDrawPos += lineHeight;
		for(int i = 0; i < craftedItems.Count; i++)
		{
			GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), craftedItems[i].getName());
			yDrawPos += lineHeight;
		}

		for(int i = 0; i < materialItems.Count; i++)
		{
			GUI.Label(new Rect(110, yDrawPos, 300, lineHeight), materialItems[i].getName() + " x"+materialItems[i].getAmount());
			yDrawPos += lineHeight;
		}
	}

	public List<CraftedItem> getCraftedItems()
	{
		return craftedItems;
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
			if(mi == null || mi.getAmount() < i.amount) return false;
		}

		return true;
	}

}
