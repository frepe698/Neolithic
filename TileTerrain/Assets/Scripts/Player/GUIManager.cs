using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour{

	EventSystem eventSystem;

	private Transform canvas;
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
	private bool craftingActive;

	private GameObject craftingObject;
	private List<Button> itemCraftingButtons;
	private List<Button> materialCraftingButtons;

	private RectTransform healthbarTransform;
	private RectTransform energybarTransform;
	private RectTransform hungerbarTransform;
	private RectTransform coldbarTransform;

	private bool mouseOverGUI = false;

	void Awake()
	{
		canvas = GameObject.Find("Canvas").transform;
		eventSystem = GameObject.Find ("EventSystem").GetComponent<EventSystem>();

		healthbarTransform = canvas.FindChild("healthbar").FindChild("healthbar_mask").FindChild("healthbar_drink").GetComponent<RectTransform>();
		energybarTransform = canvas.FindChild("energybar").FindChild("energybar_mask").FindChild("energybar_drink").GetComponent<RectTransform>();

		Transform rightPanel = canvas.FindChild("RightPanel");
		hungerbarTransform = rightPanel.FindChild("hungerbar").FindChild("hungerbar_mask").GetComponent<RectTransform>();
		coldbarTransform = rightPanel.FindChild("coldbar").FindChild("coldbar_mask").GetComponent<RectTransform>();

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

		craftingObject = canvas.FindChild("Crafting").gameObject;
		itemCraftingButtons = new List<Button>();
		materialCraftingButtons = new List<Button>();
		updateCrafting();

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
		Hero hero = GameMaster.getPlayerHero();
		healthbarTransform.anchoredPosition = new Vector2(0, (hero.getHealth() / hero.getMaxHealth()) * 100);
		energybarTransform.anchoredPosition = new Vector2(0, (hero.getEnergy() / hero.getMaxEnergy()) * 100);
		hungerbarTransform.sizeDelta = new Vector2(100, (hero.getHunger()/hero.getMaxHunger())*100);
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

		while(craftedItemButtons.Count < manItems.Count)
		{
			int index = craftedItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
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
		}

		while(materialItemButtons.Count < resItems.Count)
		{
			int index = materialItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
			bo.transform.SetParent(materialItemParent);
			
			RectTransform rect = bo.GetComponent<RectTransform>();
			rect.localPosition = new Vector3(0, -20 - index*25, 0);
			rect.transform.localScale = Vector3.one;
			
			Text text = bo.transform.FindChild("Text").GetComponent<Text>();
			Debug.Log (resItems[index].getAmount());
			text.text = resItems[index].getInventoryDisplay();
			text.color = itemTextColor;
			
			Button button = bo.GetComponent<Button>();
			button.onClick.AddListener(() => inventoryItemButtonClick(index));
			
			materialItemButtons.Add(button);
		}

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
		}
		
		while(consumableItemButtons.Count < conItems.Count)
		{
			int index = consumableItemButtons.Count;
			GameObject bo = (GameObject)Instantiate(prefab);
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
		GameObject prefab = (GameObject)Resources.Load ("GUI/Button");

		int materialItemOffset = -50;
		while(itemCraftingButtons.Count < itemRecipes.Length)
		{
			int index = itemCraftingButtons.Count;
			string gameName = itemRecipes[index].gameName;
			string name = itemRecipes[index].name;
			GameObject button = (GameObject)Instantiate(prefab);
			button.transform.SetParent(craftingObject.transform);
			button.transform.localPosition = new Vector3(0, 100 - index*40, 0);
			button.transform.localScale = new Vector3(1, 1, 1);
			Button b = button.GetComponent<Button>();
			b.onClick.AddListener(() =>  craftItemButtonClick(name) );
			button.transform.FindChild("Text").GetComponent<Text>().text = gameName;
			itemCraftingButtons.Add(b);
			materialItemOffset -= index*40;
		}

		MaterialRecipeData[] materialRecipes = DataHolder.Instance.getMaterialRecipeData();
		while(materialCraftingButtons.Count < materialRecipes.Length)
		{
			int index = materialCraftingButtons.Count;
			string gameName = materialRecipes[index].gameName;
			string name = materialRecipes[index].name;
			GameObject button = (GameObject)Instantiate(prefab);
			button.transform.SetParent(craftingObject.transform);
			button.transform.localPosition = new Vector3(0, 100 + materialItemOffset - index*40, 0);
			button.transform.localScale = new Vector3(1,1,1);
			Button b = button.GetComponent<Button>();
			b.onClick.AddListener( () => craftMaterialButtonClick(name) );
			button.transform.FindChild ("Text").GetComponent<Text>().text = gameName;
			materialCraftingButtons.Add(b);
			
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
