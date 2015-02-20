﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour{

	EventSystem eventSystem;

	private Transform canvas;

    //Inventory
	private GameObject inventoryObject;
	private Inventory inventory;

   
	private Transform craftedItemParent;
	private Transform consumableItemParent;
	private Transform materialItemParent;
	private ScrollRect inventoryScrollRect;
	private RectTransform inventoryMask;

	private List<Button> craftedItemButtons;
	private List<Button> consumableItemButtons;
	private List<Button> materialItemButtons;
	private Button[] inventoryTabs;
    private const string INVENTORY_BUTTON_NAME = "InventoryButton";

	private int lastClickedItem = 0;
	private float clickTime = 0;
	private readonly static float DOUBLECLICK_TIME = 0.5f;

	private int selectedItemTab = 0;
	private readonly static int TAB_CRAFTED = 0;
	private readonly static int TAB_MATERIAL = 1;
	private readonly static int TAB_CONSUMABLE = 2;

	private int selectedItem = -1;
	private Color itemTextColor = new Color(0.5f,0.5f,0.5f);
	private Color selectedItemTextColor = new Color(1,1,1);

	private bool inventoryActive;
	

    //Crafting window
	private GameObject craftingObject;

    private Transform itemRecipeParent;
    private Transform materialRecipeParent;
    private Transform consumableRecipeParent;
    private ScrollRect craftingScrollRect;
    private RectTransform craftingMask;

	private List<Button> itemCraftingButtons;
    private List<Button> materialCraftingButtons;
    private List<Button> consumableCraftingButtons;
    private Button[] craftingTabs;
    private const string CRAFTING_BUTTON_NAME = "CraftingButton";

    private int selectedCraftingTab = 0;
    private bool craftingActive;

    //Tooltips
    private GameObject recipeTooltip;
    private Text recipeTooltipName;
    private Text recipeTooltipIngredients;
    private Text recipeTooltipDescription;
    private int recipeTooltipIndex = -1;

    private GameObject itemTooltip;
    private Text itemTooltipName;
    private Text itemTooltipStats;
    private Text itemTooltipDescription;
    private string itemTooltipIndexName = "";
   // public Vector3 tooltipOffset = Vector3.zero;
    
    //Player stats
	private RectTransform healthbarTransform;
	private RectTransform energybarTransform;
	private RectTransform hungerbarTransform;
	private RectTransform coldbarTransform;

	private bool mouseOverGUI = false;

	void Awake()
	{
		canvas = GameObject.Find("GameCanvas").transform;
		eventSystem = GameObject.Find ("EventSystem").GetComponent<EventSystem>();

		healthbarTransform = canvas.FindChild("healthbar").FindChild("healthbar_mask").FindChild("healthbar_drink").GetComponent<RectTransform>();
		energybarTransform = canvas.FindChild("energybar").FindChild("energybar_mask").FindChild("energybar_drink").GetComponent<RectTransform>();

		Transform rightPanel = canvas.FindChild("RightPanel");
		hungerbarTransform = rightPanel.FindChild("hungerbar").FindChild("hungerbar_mask").GetComponent<RectTransform>();
		coldbarTransform = rightPanel.FindChild("coldbar").FindChild("coldbar_mask").GetComponent<RectTransform>();

        //Inventory init
		inventoryObject = canvas.FindChild("Inventory").gameObject;
		Transform scrollMask = inventoryObject.transform.FindChild("ScrollMask");
		craftedItemParent = scrollMask.FindChild("CraftedItems");
		consumableItemParent = scrollMask.FindChild("ConsumableItems");
		materialItemParent = scrollMask.FindChild("MaterialItems");
		inventoryScrollRect = scrollMask.GetComponent<ScrollRect>();
		inventoryMask = scrollMask.GetComponent<RectTransform>();

		inventoryTabs = new Button[3];
		for(int i = 0; i < 3; i++)
		{
			int index = i;
			inventoryTabs[i] = inventoryObject.transform.FindChild("Tab"+i).GetComponent<Button>();
			inventoryTabs[i].onClick.AddListener(() => inventoryTabButtonClick(index));
		}

        //Crafting init
		craftingObject = canvas.FindChild("Crafting").gameObject;
        Transform cscrollMask = craftingObject.transform.FindChild("ScrollMask");
        itemRecipeParent = cscrollMask.FindChild("ItemRecipes");
        materialRecipeParent = cscrollMask.FindChild("MaterialRecipes");
        consumableRecipeParent = cscrollMask.FindChild("ConsumableRecipes");
        craftingScrollRect = cscrollMask.GetComponent<ScrollRect>();
        craftingMask = cscrollMask.GetComponent<RectTransform>();

        craftingTabs = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            craftingTabs[i] = craftingObject.transform.FindChild("Tab" + i).GetComponent<Button>();
            craftingTabs[i].onClick.AddListener(() => craftingTabButtonClick(index));
        }
		itemCraftingButtons = new List<Button>();
		materialCraftingButtons = new List<Button>();
        consumableCraftingButtons = new List<Button>();
		updateCrafting();

        //Tooltips init
        recipeTooltip = canvas.FindChild("RecipeTooltip").gameObject;
        recipeTooltipName = recipeTooltip.transform.FindChild("GameName").GetComponent<Text>();
        recipeTooltipIngredients = recipeTooltip.transform.FindChild("Ingredients").GetComponent<Text>();
        recipeTooltipDescription = recipeTooltip.transform.FindChild("Description").GetComponent<Text>();
        recipeTooltip.SetActive(false);

        itemTooltip = canvas.FindChild("ItemTooltip").gameObject;
        itemTooltipName = itemTooltip.transform.FindChild("GameName").GetComponent<Text>();
        itemTooltipStats = itemTooltip.transform.FindChild("Stats").GetComponent<Text>();
        itemTooltipDescription = itemTooltip.transform.FindChild("Description").GetComponent<Text>();
        itemTooltip.SetActive(false);

		activateInventory(false);
		activateCrafting(false);
	}

	public void update()
	{
		if(inventory != null && inventory.shouldUpdate())
		{
			updateInventory();
		}
		mouseOverGUI = eventSystem.IsPointerOverGameObject();
        if (mouseOverGUI)
        {
            updateMouseCollision();
        }
        else
        {
            inactivateRecipeTooltip();
            inactivateItemTooltip();
        }
		Hero hero = GameMaster.getPlayerHero();
		healthbarTransform.anchoredPosition = new Vector2(0, (hero.getHealth() / hero.getMaxHealth()) * 100);
		energybarTransform.anchoredPosition = new Vector2(0, (hero.getEnergy() / hero.getMaxEnergy()) * 100);
		hungerbarTransform.sizeDelta = new Vector2(100, (hero.getHunger()/hero.getMaxHunger())*100);
	}

    public void updateMouseCollision()
    {
        PointerEventData pe = new PointerEventData(eventSystem);
        pe.position = Input.mousePosition;

        List<RaycastResult> hits = new List<RaycastResult>();
        eventSystem.RaycastAll(pe, hits);

        bool hit = false;
        GameObject hgo = null;
        foreach (RaycastResult h in hits)
        {
            GameObject g = h.gameObject;

            hit = (g.GetComponent<Button>());

            if(hit)
            {
                hgo = g;
                break;
            }
        }
        if (hit)
        {
            if (hgo.name.StartsWith(INVENTORY_BUTTON_NAME))
            {
                int index;
                if (int.TryParse(hgo.name.Remove(0, INVENTORY_BUTTON_NAME.Length), out index))
                {
                    string itemName = null;
                    if (index < craftedItemButtons.Count)
                    {
                        itemName = inventory.getCraftedItems()[index].getName();
                    }
                    else if (index < materialItemButtons.Count + craftedItemButtons.Count)
                    {
                        itemName = inventory.getMaterialItems()[index - craftedItemButtons.Count].getName();
                    }
                    else if (index < consumableItemButtons.Count + materialItemButtons.Count + craftedItemButtons.Count)
                    {
                        itemName = inventory.getConsumableItems()[index - materialItemButtons.Count - craftedItemButtons.Count].getName();
                    }

                    if (itemName != null && !itemName.Equals(itemTooltipIndexName))
                    {
                        activateItemTooltip(itemName);
                    }
                }
            }
            else if (hgo.name.StartsWith(CRAFTING_BUTTON_NAME))
            {
                int index;
                if (int.TryParse(hgo.name.Remove(0, CRAFTING_BUTTON_NAME.Length), out index))
                {
                    if (index != recipeTooltipIndex)
                    {
                        activateRecipeTooltip(index);
                    }
                }
            }
        }
        else
        {
            inactivateRecipeTooltip();
            inactivateItemTooltip();
        }

    }

    private void inactivateRecipeTooltip()
    {
        recipeTooltipIndex = -1;
        recipeTooltip.SetActive(false);
    }

    private void activateRecipeTooltip(int index)
    {
        recipeTooltipIndex = index;
        recipeTooltip.SetActive(true);
        if (index < itemCraftingButtons.Count)
        {
#if false
            RectTransform cwrect = craftingObject.GetComponent<RectTransform>();
            RectTransform ttrect = recipeTooltip.GetComponent<RectTransform>();
            Vector3 pos = itemCraftingButtons[index].GetComponent<RectTransform>().localPosition + tooltipOffset;
            pos.x = Mathf.Clamp(pos.x, 0, cwrect.sizeDelta.x - ttrect.sizeDelta.x);

            float halfy = (cwrect.sizeDelta.y) / 2;
            pos.y = Mathf.Clamp(pos.y, -(halfy - ttrect.sizeDelta.y), halfy);
            ttrect.localPosition = pos;

            recipeTooltip.transform.SetAsLastSibling();
#endif
            ItemRecipeData data = DataHolder.Instance.getItemRecipeData()[index];
            recipeTooltipName.text = data.gameName;
            string ing = "";
            foreach (Ingredient i in data.ingredients)
            {
                ing += i.name + " x" + i.amount + "\n"; 
            }
            recipeTooltipIngredients.text = ing;
            recipeTooltipDescription.text = data.tooltip;
        }
        else
        {
            MaterialRecipeData data = DataHolder.Instance.getMaterialRecipeData()[index - itemCraftingButtons.Count];
            recipeTooltipName.text = data.gameName;
            string ing = "";
            foreach (Ingredient i in data.ingredients)
            {
                ing += i.name + " x" + i.amount + "\n";
            }
            recipeTooltipIngredients.text = ing;
            recipeTooltipDescription.text = data.tooltip;
        }
    }

    private void inactivateItemTooltip()
    {
        itemTooltipIndexName = "";
        itemTooltip.SetActive(false);
    }

    private void activateItemTooltip(string name)
    {
        itemTooltipIndexName = name;
        itemTooltip.SetActive(true);
        Debug.Log(name);
        ItemData data = DataHolder.Instance.getItemData(name);
        itemTooltipName.text = data.gameName;

        itemTooltipStats.text = data.getTooltipStatsString();
        //itemTooltipDescription.text = data.tooltip;

    }

	public void setInventory(Inventory inventory)
	{
		this.inventory = inventory;
		craftedItemButtons = new List<Button>();
		materialItemButtons = new List<Button>();
		consumableItemButtons = new List<Button>();
		updateInventory();
	}

	private void updateInventory()
	{
		//TODO: fix the scroller when changing tabs
		List<CraftedItem> manItems = inventory.getCraftedItems();
		List<MaterialItem> resItems = inventory.getMaterialItems();
		List<ConsumableItem> conItems = inventory.getConsumableItems();

		GameObject prefab = (GameObject)Resources.Load ("GUI/InventoryItemButton");

        int addedIndex = 0;
      
		while(craftedItemButtons.Count < manItems.Count)
		{
			int index = craftedItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
            bo.name = INVENTORY_BUTTON_NAME + index;
			bo.transform.SetParent(craftedItemParent);

			RectTransform rect = bo.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(0, -20 - index*25, 0);
			rect.transform.localScale = Vector3.one;

			Text text = bo.transform.FindChild("Text").GetComponent<Text>();
			text.text = manItems[index].getGameName();
			text.color = itemTextColor;

			Button button = bo.GetComponent<Button>();
			button.onClick.AddListener(() => inventoryItemButtonClick(index));

			craftedItemButtons.Add(button);
		}

        addedIndex += craftedItemButtons.Count;

		for(int i = 0; i < materialItemButtons.Count; i++)
		{
			if(i >= resItems.Count)
			{
				Destroy(materialItemButtons[i].gameObject);
				materialItemButtons.RemoveAt(i);
				i--;
				continue;
			}

			materialItemButtons[i].transform.FindChild("Text").GetComponent<Text>().text = resItems[i].getInventoryDisplay();
            materialItemButtons[i].name = INVENTORY_BUTTON_NAME + (i + addedIndex);
		}

		while(materialItemButtons.Count < resItems.Count)
		{
			int index = materialItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
            bo.name = INVENTORY_BUTTON_NAME + (index + addedIndex);
			bo.transform.SetParent(materialItemParent);
			
			RectTransform rect = bo.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(0, -20 - index*25, 0);
			rect.transform.localScale = Vector3.one;
			
			Text text = bo.transform.FindChild("Text").GetComponent<Text>();
			text.text = resItems[index].getInventoryDisplay();
			text.color = itemTextColor;
			
			Button button = bo.GetComponent<Button>();
			button.onClick.AddListener(() => inventoryItemButtonClick(index));
			
			materialItemButtons.Add(button);
		}

        addedIndex += materialItemButtons.Count;

		for(int i = 0; i < consumableItemButtons.Count; i++)
		{
			if(i >= conItems.Count)
			{
				Destroy(consumableItemButtons[i].gameObject);
				consumableItemButtons.RemoveAt(i);
				i--;
				continue;
			}
			
			consumableItemButtons[i].transform.FindChild("Text").GetComponent<Text>().text = conItems[i].getInventoryDisplay();
            Debug.Log((i + craftedItemButtons.Count + materialCraftingButtons.Count));
            consumableItemButtons[i].name = INVENTORY_BUTTON_NAME + (i + addedIndex);
		}
		
		while(consumableItemButtons.Count < conItems.Count)
		{
			int index = consumableItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
            bo.name = INVENTORY_BUTTON_NAME + (index + addedIndex);
			bo.transform.SetParent(consumableItemParent);
			
			RectTransform rect = bo.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(0, -20 - index*25, 0);
			rect.transform.localScale = Vector3.one;
			
			Text text = bo.transform.FindChild("Text").GetComponent<Text>();
			Debug.Log (conItems[index].getAmount());
			text.text = conItems[index].getInventoryDisplay();
			text.color = itemTextColor;
			
			Button button = bo.GetComponent<Button>();
			button.onClick.AddListener(() => inventoryItemButtonClick(index));
			
			consumableItemButtons.Add(button);
		}

		float cheight = craftedItemButtons.Count*25+12;
		float mheight = materialItemButtons.Count*25+12;
		float fheight = consumableItemButtons.Count*25+12;

		craftedItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, cheight);
		materialItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, mheight);
		consumableItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, fheight);

		onSelectItemTab();
	}

	public void onSelectItemTab()
	{
		craftedItemParent.gameObject.SetActive(false);
		materialItemParent.gameObject.SetActive(false);
		consumableItemParent.gameObject.SetActive(false);
		inventoryTabs[TAB_CRAFTED].interactable = true;
		inventoryTabs[TAB_MATERIAL].interactable = true;
		inventoryTabs[TAB_CONSUMABLE].interactable = true;

		if(selectedItemTab == TAB_CRAFTED)
		{
			inventoryScrollRect.vertical = inventoryMask.sizeDelta.y < craftedItemButtons.Count*25+12;
			craftedItemParent.gameObject.SetActive(true);
			inventoryTabs[TAB_CRAFTED].interactable = false;
		}
		else if(selectedItemTab == TAB_MATERIAL)
		{
			inventoryScrollRect.vertical = inventoryMask.sizeDelta.y < materialItemButtons.Count*25+12;
			materialItemParent.gameObject.SetActive(true);
			inventoryTabs[TAB_MATERIAL].interactable = false;
		}
		else if(selectedItemTab == TAB_CONSUMABLE)
		{
			inventoryScrollRect.vertical = inventoryMask.sizeDelta.y < consumableItemButtons.Count*25+12;
			consumableItemParent.gameObject.SetActive(true);
			inventoryTabs[TAB_CONSUMABLE].interactable = false;
		}
	}

	public void inventoryTabButtonClick(int index)
	{
		Debug.Log ("clicked tab " + index);
		deselectItem();
		selectedItemTab = index;
		onSelectItemTab();
	}

	public void deselectItem()
	{
		if(selectedItemTab == TAB_CRAFTED)
		{
			if(selectedItem >= 0 && selectedItem < craftedItemButtons.Count)
			{
				craftedItemButtons[selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
			}
		}
		else
		{
			if(selectedItem >= 0 && selectedItem < materialItemButtons.Count)
			{
				materialItemButtons[selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
			}
		}
		selectedItem = -1;
	}

	public void inventoryItemButtonClick(int index)
	{
		bool doubleclick = false;
		if(Time.time <= clickTime + DOUBLECLICK_TIME && lastClickedItem == index)
		{
			doubleclick = true;
			clickTime = 0;
		}
		else
		{
			clickTime = Time.time;
		}
		if(selectedItemTab == TAB_CRAFTED)
		{
			if(selectedItem >= 0 && selectedItem < craftedItemButtons.Count)
			{
				craftedItemButtons[selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
			}
			craftedItemButtons[index].transform.FindChild("Text").GetComponent<Text>().color = selectedItemTextColor;
			if(doubleclick) GameMaster.getGameController().requestItemChange(GameMaster.getPlayerUnitID(), index);
		}
		else if(selectedItemTab == TAB_MATERIAL)
		{
			if(selectedItem >= 0 && selectedItem < materialItemButtons.Count)
			{
				materialItemButtons[selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
			}
			materialItemButtons[index].transform.FindChild("Text").GetComponent<Text>().color = selectedItemTextColor;
		}
		else if(selectedItemTab == TAB_CONSUMABLE)
		{
			if(selectedItem >= 0 && selectedItem < consumableItemButtons.Count)
			{
				consumableItemButtons[selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
			}
			consumableItemButtons[index].transform.FindChild("Text").GetComponent<Text>().color = selectedItemTextColor;
			if(doubleclick) GameMaster.getGameController().requestItemConsume(GameMaster.getPlayerUnitID(), index);
		}

		lastClickedItem = index;
		selectedItem = index;
	}

	public void toggleInventory()
	{
		activateInventory(!inventoryActive);
	}

	public void toggleCrafting()
	{
		activateCrafting(!craftingActive);
	}

	private void updateCrafting()
	{
		ItemRecipeData[] itemRecipes = DataHolder.Instance.getItemRecipeData();
		GameObject prefab = (GameObject)Resources.Load ("GUI/CraftingButton");

        int lineHeight = 32;

		while(itemCraftingButtons.Count < itemRecipes.Length)
		{
			int index = itemCraftingButtons.Count;
			string gameName = itemRecipes[index].gameName;
			string name = itemRecipes[index].name;
			GameObject button = (GameObject)Instantiate(prefab);
            button.name = CRAFTING_BUTTON_NAME + itemCraftingButtons.Count;
			button.transform.SetParent(itemRecipeParent);
            button.transform.localPosition = new Vector3(0, -20 - index * lineHeight, 0);
			button.transform.localScale = new Vector3(1, 1, 1);
			Button b = button.GetComponent<Button>();
			b.onClick.AddListener(() =>  craftItemButtonClick(name) );
			button.transform.FindChild("Text").GetComponent<Text>().text = gameName;
			itemCraftingButtons.Add(b);
		}

		MaterialRecipeData[] materialRecipes = DataHolder.Instance.getMaterialRecipeData();
		while(materialCraftingButtons.Count < materialRecipes.Length)
		{
			int index = materialCraftingButtons.Count;
			string gameName = materialRecipes[index].gameName;
			string name = materialRecipes[index].name;
			GameObject button = (GameObject)Instantiate(prefab);
            button.name = CRAFTING_BUTTON_NAME + itemCraftingButtons.Count;
			button.transform.SetParent(materialRecipeParent);
            button.transform.localPosition = new Vector3(0, -20 - index * lineHeight, 0);
			button.transform.localScale = new Vector3(1,1,1);
			Button b = button.GetComponent<Button>();
			b.onClick.AddListener( () => craftMaterialButtonClick(name) );
			button.transform.FindChild ("Text").GetComponent<Text>().text = gameName;
			materialCraftingButtons.Add(b);
			
		}

        float cheight = itemCraftingButtons.Count * lineHeight + 12;
        float mheight = materialCraftingButtons.Count * lineHeight + 12;
        float fheight = consumableCraftingButtons.Count * lineHeight + 12;

        itemRecipeParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, cheight);
        materialRecipeParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, mheight);
        consumableRecipeParent.GetComponent<RectTransform>().sizeDelta = new Vector2(190, fheight);

	}

    public void craftingTabButtonClick(int index)
    {
        Debug.Log("clicked tab " + index);
        selectedCraftingTab = index;
        onSelectCraftingTab();
    }

    public void onSelectCraftingTab()
    {
        itemRecipeParent.gameObject.SetActive(false);
        materialRecipeParent.gameObject.SetActive(false);
        consumableRecipeParent.gameObject.SetActive(false);
        craftingTabs[TAB_CRAFTED].interactable = true;
        craftingTabs[TAB_MATERIAL].interactable = true;
        craftingTabs[TAB_CONSUMABLE].interactable = true;

        if (selectedCraftingTab == TAB_CRAFTED)
        {
            craftingScrollRect.vertical = craftingMask.sizeDelta.y < itemCraftingButtons.Count * 32 + 12;
            itemRecipeParent.gameObject.SetActive(true);
            craftingTabs[TAB_CRAFTED].interactable = false;
        }
        else if (selectedCraftingTab == TAB_MATERIAL)
        {
            craftingScrollRect.vertical = craftingMask.sizeDelta.y < materialCraftingButtons.Count * 32 + 12;
            materialRecipeParent.gameObject.SetActive(true);
            craftingTabs[TAB_MATERIAL].interactable = false;
        }
        else if (selectedCraftingTab == TAB_CONSUMABLE)
        {
            craftingScrollRect.vertical = craftingMask.sizeDelta.y < consumableCraftingButtons.Count * 32 + 12;
            consumableRecipeParent.gameObject.SetActive(true);
            craftingTabs[TAB_CONSUMABLE].interactable = false;
        }
    }

	public void craftItemButtonClick(string name)
	{
		if(inventory.hasIngredients(DataHolder.Instance.getItemRecipeData(name).ingredients))
		{
			Debug.Log ("can craft");
			GameMaster.getGameController().requestItemCraft(GameMaster.getPlayerUnitID(), name);
		}
		else
		{
			Debug.Log ("cant afford this");
		}

	}

	public void craftMaterialButtonClick(string name)
	{
		Debug.Log("Clicked" + name);
		if(inventory.hasIngredients(DataHolder.Instance.getMaterialRecipeData(name).ingredients))
		{
			Debug.Log ("Can craft");
			GameMaster.getGameController ().requestMaterialCraft (GameMaster.getPlayerUnitID(), name);
		}
		else
		{
			Debug.Log ("Cant afford this");
		}
	}

	public void activateInventory(bool active)
	{
		inventoryActive = active;
		inventoryObject.SetActive(inventoryActive);
	}

	public void activateCrafting(bool active)
	{
		craftingActive = active;
		craftingObject.SetActive(craftingActive);
	}

	public bool isMouseOverGUI()
	{
		return mouseOverGUI;
	}
}