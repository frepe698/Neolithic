using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GUIManager : MonoBehaviour{

	EventSystem eventSystem;

	private Transform canvas;

    private Hero playerHero;

    #region IN GAME MENU MEMBERS
    private GameObject ingameMenuObject;
    private bool ingameMenuActive = false;
    private const int MENU_BUTTON_RESUME = 0;
    private const int MENU_BUTTON_SETTINGS = 1;
    private const int MENU_BUTTON_MAINMENU = 2;
    private const int MENU_BUTTON_DESKTOP = 3;
    #endregion //END IN GAME MENU MEMBERS

    #region CHAT MEMBERS
    private GameObject chatObject;
    private GameObject chatInputObject;
    private InputField chatInputField;
    private Text chatOutputText;
    private float chatOutputHeight = 0;
    private Scrollbar chatScrollbar;
    private bool justOpenedChat = false;
    private bool chatting = false;
    private bool chatOutputOpen = false;
    private float chatCloseTime = 0;
    #endregion //END CHAT MEMBERS

    #region HERO STATS MEMBERS
    private GameObject heroStatsObject;
    private UnitStats unitStats;

    private Text[] statTexts;

    private bool heroStatsActive;
    #endregion //END HERO STATS MEMBERS

    #region ABILITY WINDOW MEMBERS
    private GameObject abilityWindowObject;
    private SkillManager skillManager;

    private Text headerText;
    private Text[] skillLevelText;
    private Button[,] abilityButtons;

    private bool abilityWindowActive;
    #endregion //END ABILITY WINDOW MEMBERS

    #region INVENTORY MEMBERS
    private GameObject inventoryObject;
	private Inventory inventory;
   
	private RectTransform[] itemParents;
	private ScrollRect inventoryScrollRect;
	private RectTransform inventoryMask;

    public const int ITEM_TYPE_COUNT = 3;
    private const float ITEM_LINE_HEIGHT = 14;
	private List<Button>[] itemButtons;
    private bool[] inventoryFoldOpen;
	private RectTransform[] inventoryFolds;
    private RectTransform[] inventoryFoldArrows;
    private RectTransform itemHolder;
    private const string INVENTORY_BUTTON_NAME = "InventoryButton";

	private int lastClickedItem = 0;
	private float clickTime = 0;
	private readonly static float DOUBLECLICK_TIME = 0.5f;

	private const int TAB_CRAFTED = 0;
	private const int TAB_MATERIAL = 1;
	private const int TAB_CONSUMABLE = 2;

	private int selectedItem = -1;
	private Color itemTextColor = new Color(0,0,0);
	private Color selectedItemTextColor = new Color(1,1,1);

	private bool inventoryActive;
    
    #endregion //END INVENTORY MEMBERS

    #region CRAFTING MEMBERS
    private GameObject craftingObject;

    private RectTransform[] recipeParents;
    private ScrollRect craftingScrollRect;
    private RectTransform craftingMask;

    public const int RECIPE_TYPE_COUNT = 3;
	private List<Button>[] craftingButtons;
    private bool[] recipeFoldOpen;
    private RectTransform[] recipeFolds;
    private RectTransform[] recipeFoldArrows;
    private RectTransform recipeHolder;
    private const string CRAFTING_BUTTON_NAME = "CraftingButton";

    private int selectedCraftingTab = 0;
    private bool craftingActive;
    #endregion //END CRAFTING MEMBERS

    #region TOOLTIP MEMBERS
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
    private bool itemTooltipIsRecipe = false;
    // public Vector3 tooltipOffset = Vector3.zero;
    #endregion //END TOOLTIP MEMBERS

    #region PLAYER STATS MEMBERS
    private RectTransform healthbarTransform;
	private RectTransform energybarTransform;
	private RectTransform hungerbarTransform;
	private RectTransform coldbarTransform;
    #endregion //END PLAYER STATS MEMBERS

    #region QUICK USE ITEMS MEMBERS
    public const int QUICK_USE_ITEM_COUNT = 5;
    private QuickUseItem[] quickUseItems;
    private Image[] quickUseItemImages;
    private Image[] quickUseItemEquipedImages;
    private Text[] quickUseItemAmount;
    #endregion //END QUICK USE ITEMS MEMBERS

    #region ABILITIES MEMBERS
    public const int ABILITY_COUNT = 4;
    private Image[] abilityIcons;
    private Transform[] abilityCooldown;
    #endregion

    #region FPS DISPLAY MEMBERS
    private Text fpsDisplay;
    private float nextUpdateTimer;
    private int frames;
    #endregion //END FPS DISPLAY MEMBERS

    private bool mouseOverGUI = false;

	void Awake()
    {
        canvas = GameObject.Find("GameCanvas").transform;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        healthbarTransform = canvas.FindChild("healthbar").FindChild("healthbar_mask").FindChild("healthbar_drink").GetComponent<RectTransform>();
        energybarTransform = canvas.FindChild("energybar").FindChild("energybar_mask").FindChild("energybar_drink").GetComponent<RectTransform>();

        Transform rightPanel = canvas.FindChild("RightPanel");
        hungerbarTransform = rightPanel.FindChild("hungerbar").FindChild("hungerbar_mask").GetComponent<RectTransform>();
        coldbarTransform = rightPanel.FindChild("coldbar").FindChild("coldbar_mask").GetComponent<RectTransform>();

        #region INGAME MENU INIT
        //Ingame Menu init
        ingameMenuObject = canvas.FindChild("IngameMenu").gameObject;
        for (int i = 0; i < 4; i++)
        {
            Button b = ingameMenuObject.transform.FindChild("Button" + i).GetComponent<Button>();
            int index = i;
            b.onClick.AddListener(() => ingameMenuButtonClick(index));
        }
        ingameMenuActive = false;
        ingameMenuObject.SetActive(ingameMenuActive);
        #endregion

        #region CHAT INIT
        //Chat
        chatObject = canvas.FindChild("Chat").gameObject;
        chatInputObject = chatObject.transform.FindChild("ChatInput").gameObject;
        chatInputField = chatInputObject.GetComponent<InputField>();
        chatOutputText = chatObject.transform.FindChild("ChatOutputScrollmask").FindChild("ChatOutput").GetComponent<Text>();
        chatScrollbar = chatObject.transform.FindChild("Scrollbar").GetComponent<Scrollbar>();
        #endregion

        #region HERO STATS INIT
        //Hero stats
        heroStatsObject = canvas.FindChild("HeroStats").gameObject;

        {
            //Ability window
            abilityWindowObject = canvas.FindChild("AbilityWindow").gameObject;
            headerText = abilityWindowObject.transform.FindChild("Header").GetComponent<Text>();
            GameObject skillPrefab = (GameObject)Resources.Load("GUI/Skill");
            SkillData[] skillData = DataHolder.Instance.getAllSkillData();
            skillLevelText = new Text[skillData.Length];
            abilityButtons = new Button[skillData.Length, 5];
            float width = skillPrefab.GetComponent<RectTransform>().sizeDelta.x;
            float startX = (-(abilityWindowObject.GetComponent<RectTransform>().sizeDelta.x) / 2) + width; 
            for (int i = 0; i < skillData.Length; i++)
            {
                GameObject go = Instantiate(skillPrefab);
                go.transform.SetParent(abilityWindowObject.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector3(startX + width * i, 100);



                Text text = go.GetComponent<Text>();
                text.text = skillData[i].gameName.Substring(0, 3).ToUpper();
                Color color = new Color(0, 0, 0, 1);
                if (i < 4) color = Color.Lerp(Color.red, Color.blue, (float)i/4.0f);
                else if (i < 8) color = Color.Lerp(Color.blue, Color.green, (float)(i-4)/4.0f);
                else color = Color.Lerp(Color.green, Color.red, (float)(i - 8) / 3.0f);
                
                text.color = color;


                skillLevelText[i] = go.transform.FindChild("LevelText").GetComponent<Text>();

                int skillIndex = i;
                for (int b = 0; b < 5; b++)
                {
                    Button button = go.transform.FindChild("ButtonAbility" + b).GetComponent<Button>();
                    int levelIndex = b;
                    button.onClick.AddListener(() => abilityButtonClick(skillIndex, levelIndex));
                    abilityButtons[i, b] = button;
                }
            }
        }
        #endregion

        #region INVENTORY INIT
        //Inventory init
        inventoryObject = canvas.FindChild("Inventory").gameObject;
        Transform scrollMask = inventoryObject.transform.FindChild("ScrollMask");
        inventoryScrollRect = scrollMask.GetComponent<ScrollRect>();
        inventoryMask = scrollMask.GetComponent<RectTransform>();

        inventoryFolds = new RectTransform[ITEM_TYPE_COUNT];
        inventoryFoldArrows = new RectTransform[ITEM_TYPE_COUNT];
        itemParents = new RectTransform[ITEM_TYPE_COUNT];
        inventoryFoldOpen = new bool[ITEM_TYPE_COUNT];
        itemButtons = new List<Button>[ITEM_TYPE_COUNT];

        itemHolder = scrollMask.FindChild("Items").GetComponent<RectTransform>();
        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            int index = i;
            itemParents[i] = itemHolder.FindChild("Items" + i).GetComponent<RectTransform>();
            inventoryFolds[i] = itemHolder.FindChild("ItemsFold" + i).GetComponent<RectTransform>();
            inventoryFolds[i].GetComponent<Button>().onClick.AddListener(() => inventoryFoldButtonClick(index));
            inventoryFoldArrows[i] = inventoryFolds[i].FindChild("Arrow").GetComponent<RectTransform>();
            itemButtons[i] = new List<Button>();
            inventoryFoldOpen[i] = true;
        }
        #endregion

        #region CRAFTING INIT
        //Crafting init
        craftingObject = canvas.FindChild("Crafting").gameObject;
        Transform cscrollMask = craftingObject.transform.FindChild("ScrollMask");
        craftingScrollRect = cscrollMask.GetComponent<ScrollRect>();
        craftingMask = cscrollMask.GetComponent<RectTransform>();


        recipeFolds = new RectTransform[RECIPE_TYPE_COUNT];
        recipeFoldArrows = new RectTransform[RECIPE_TYPE_COUNT];
        recipeParents = new RectTransform[RECIPE_TYPE_COUNT];
        recipeFoldOpen = new bool[RECIPE_TYPE_COUNT];
        craftingButtons = new List<Button>[RECIPE_TYPE_COUNT];

        recipeHolder = cscrollMask.FindChild("Recipes").GetComponent<RectTransform>();
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            recipeParents[i] = recipeHolder.FindChild("Recipes" + i).GetComponent<RectTransform>();
            recipeFolds[i] = recipeHolder.transform.FindChild("RecipeFold" + i).GetComponent<RectTransform>();
            recipeFolds[i].GetComponent<Button>().onClick.AddListener(() => recipeFoldButtonClick(index));
            recipeFoldArrows[i] = recipeFolds[i].FindChild("Arrow").GetComponent<RectTransform>();
            craftingButtons[i] = new List<Button>();
            recipeFoldOpen[i] = true;
        }
        updateCrafting();
        #endregion

        #region TOOLTIPS INIT
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
        #endregion

        #region QUICK USE ITEMS INIT
        //Left panel init
        Transform leftPanel = canvas.FindChild("LeftPanel");
        quickUseItems = new QuickUseItem[QUICK_USE_ITEM_COUNT];
        quickUseItemImages = new Image[QUICK_USE_ITEM_COUNT];
        quickUseItemEquipedImages = new Image[QUICK_USE_ITEM_COUNT];
        quickUseItemAmount = new Text[QUICK_USE_ITEM_COUNT];
        for (int i = 0; i < QUICK_USE_ITEM_COUNT; i++)
        {
            quickUseItemImages[i] = leftPanel.FindChild("Item" + i).GetComponent<Image>();
            quickUseItemEquipedImages[i] = quickUseItemImages[i].transform.FindChild("Equiped").GetComponent<Image>();
            quickUseItemAmount[i] = quickUseItemImages[i].transform.FindChild("Amount").GetComponent<Text>();
            quickUseItems[i] = new QuickUseEquipment();
        }
        #endregion

        #region ABILITY INIT
        abilityIcons = new Image[ABILITY_COUNT];
        abilityCooldown = new RectTransform[ABILITY_COUNT];
        for (int i = 0; i < ABILITY_COUNT; i++)
        {
            abilityIcons[i] = rightPanel.FindChild("Ability" + i).GetComponent<Image>();
            abilityCooldown[i] = abilityIcons[i].rectTransform.FindChild("Cooldown");
        }
        #endregion

        #region FPS DISPLAY INIT
        //Fps display
        fpsDisplay = canvas.FindChild("FPSDisplay").GetComponent<Text>();
        #endregion

        activateInventory(false);
        activateCrafting(false);
        activateHeroStats(false);
        activateAbilityWindow(false);
    }

    #region UPDATE

    public void update()
	{
        if (chatting)
        {
            if (justOpenedChat)
            {
                chatInputField.ActivateInputField();
                chatInputField.Select();
                justOpenedChat = false;
            }
            chatInputField.text = chatInputField.text.Replace("\n", "").Replace("\r", "");
            chatObject.SetActive(true);
            chatOutputOpen = true;
            chatInputObject.SetActive(true);
        }
        else
        {
            chatInputField.DeactivateInputField();
            chatInputObject.SetActive(false);

            if (Time.time >= chatCloseTime)
            {
                chatObject.SetActive(false);
                chatOutputOpen = false;
            }
        }

        if (chatOutputOpen)
        {
            float newHeight = chatOutputText.rectTransform.sizeDelta.y;
            if (newHeight > chatOutputHeight)
            {
                chatScrollbar.value = 0;
                chatOutputHeight = newHeight;
            }
        }

		if(inventory != null)
		{
            if (inventory.shouldUpdate()) updateInventory();
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
		healthbarTransform.anchoredPosition = new Vector2(0, (hero.getHealth() / (hero.getMaxHealth()+float.Epsilon)) * 100);
        energybarTransform.anchoredPosition = new Vector2(0, (hero.getEnergy() / (hero.getMaxEnergy() + float.Epsilon)) * 100);
		hungerbarTransform.sizeDelta = new Vector2(100, (hero.getHunger()/hero.getMaxHunger())*100);

        updateAbilityCooldownDisplays();

        //Fps display
        nextUpdateTimer += Time.deltaTime;
        frames++;
        if (nextUpdateTimer >= 1)
        {
            fpsDisplay.text = "fps: " + frames + "vsync = " + QualitySettings.vSyncCount + " - ping: " + NetworkMaster.getAveragePing();
            nextUpdateTimer--;
            frames = 0;
        }
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
                    if (index < itemButtons[0].Count)
                    {
                        itemName = inventory.getEquipmentItems()[index].getName();
                    }
                    else if (index < itemButtons[1].Count + itemButtons[0].Count)
                    {
                        itemName = inventory.getMaterialItems()[index - itemButtons[0].Count].getName();
                    }
                    else if (index < itemButtons[0].Count + itemButtons[1].Count + itemButtons[2].Count)
                    {
                        itemName = inventory.getConsumableItems()[index - itemButtons[1].Count - itemButtons[0].Count].getName();
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

    #endregion //UPDATE

    #region IN GAME MENU

    public void ingameMenuButtonClick(int index)
    {
        Debug.Log("clicked " + index);
        switch (index)
        {
            case(MENU_BUTTON_RESUME):
                toggleIngameMenu();
                break;
            case(MENU_BUTTON_SETTINGS):
                break;
            case(MENU_BUTTON_MAINMENU):
                NetworkMaster.disconnect();
                Application.LoadLevel(0);
                break;
            case(MENU_BUTTON_DESKTOP):
                NetworkMaster.disconnect();
                Application.Quit();
                break;
            default:
                break;
        }
    }

    public void toggleIngameMenu()
    {
        ingameMenuActive = !ingameMenuActive;
        ingameMenuObject.SetActive(ingameMenuActive);
    }

    #endregion // IN GAME MENU

    #region TOOLTIPS
    private void inactivateRecipeTooltip()
    {
        recipeTooltipIndex = -1;
        recipeTooltip.SetActive(false);
        if (itemTooltipIsRecipe)
        {
            inactivateItemTooltip();
        }
    }

    private void activateRecipeTooltip(int index)
    {
        RecipeData data = DataHolder.Instance.getRecipeData(index);
        if (data != null)
        {
            recipeTooltipIndex = index;
            recipeTooltip.SetActive(true);
            recipeTooltipName.text = data.gameName;
            string ing = "";
            foreach (Ingredient i in data.ingredients)
            {
                ing += i.name + " x" + i.amount + "\n";
            }
            recipeTooltipIngredients.text = ing;
            recipeTooltipDescription.text = data.description;

            ItemData item = DataHolder.Instance.getItemData(data.product);
            if (item != null)
            {
                activateItemTooltip(item.name);
                itemTooltipIsRecipe = true;
            }
        }
        
#if false
        if (index < itemCraftingButtons.Count)
        {

            RectTransform cwrect = craftingObject.GetComponent<RectTransform>();
            RectTransform ttrect = recipeTooltip.GetComponent<RectTransform>();
            Vector3 pos = itemCraftingButtons[index].GetComponent<RectTransform>().localPosition + tooltipOffset;
            pos.x = Mathf.Clamp(pos.x, 0, cwrect.sizeDelta.x - ttrect.sizeDelta.x);

            float halfy = (cwrect.sizeDelta.y) / 2;
            pos.y = Mathf.Clamp(pos.y, -(halfy - ttrect.sizeDelta.y), halfy);
            ttrect.localPosition = pos;

            recipeTooltip.transform.SetAsLastSibling();

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
#endif
    }

    private void inactivateItemTooltip()
    {
        itemTooltipIndexName = "";
        itemTooltip.SetActive(false);
    }

    private void activateItemTooltip(string name)
    {
        
        ItemData data = DataHolder.Instance.getItemData(name);
        if (data != null)
        {
            itemTooltipIndexName = name;
            itemTooltip.SetActive(true);
            itemTooltipName.text = data.gameName;

            itemTooltipStats.text = data.getTooltipStatsString();
            itemTooltipDescription.text = data.description;
            itemTooltipIsRecipe = false;
        }
    }

    #endregion //TOOLTIPS

    #region UNIT STATS

    public void setUnitStats(UnitStats unitStats)
    {
        this.unitStats = unitStats;
        this.unitStats.onStatsUpdatedListener += new System.EventHandler(onUnitStatsUpdated);
        updateUnitStats();
    }

    private void onUnitStatsUpdated(object sender, System.EventArgs args)
    {
        updateUnitStats();
    }

    private void updateUnitStats()
    {
        BaseStat[] stats = unitStats.getAllStats();
        if (statTexts == null)
        {
            GameObject prefab = (GameObject)Resources.Load("GUI/StatText");
            statTexts = new Text[stats.Length];

            for (int i = 0; i < stats.Length; i++)
            {
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(heroStatsObject.transform);
                
                go.transform.localScale = new Vector3(1, 1, 1);

                Text text = go.GetComponent<Text>();
                text.rectTransform.anchoredPosition = new Vector3(0, -30 - i * 20);

                statTexts[i] = go.GetComponent<Text>();

            }
        }

        for (int i = 0; i < stats.Length; i++)
        {
            statTexts[i].text = stats[i].getWindowString();
        }

    }

    #endregion //UNIT STATS

    #region ABILITY WINDOW

    public void setSkillManager(SkillManager skillManager)
    {
        this.skillManager = skillManager;
        skillManager.onSkillsUpdatedListener += new System.EventHandler(onSkillsUpdated);
        updateAbilityWindow();
    }

    private void onSkillsUpdated(object sender, System.EventArgs args)
    {
        updateAbilityWindow();
    }

    private void updateAbilityWindow()
    {
        if (unitStats == null) return;

        headerText.text = "Level " + unitStats.getDisplayLevel() + " <color=black>|</color> <color=white>Ability Points "+skillManager.getAbilityPoints()+"</color>";

        Skill[] skills = skillManager.getSkills();

        for (int i = 0; i < skills.Length; i++)
        {
            skillLevelText[i].text = skills[i].getDisplayLevel().ToString();

            //Update buttons
            for (int bi = 0; bi < 5; bi++)
            {
                abilityButtons[i, bi].interactable = bi == skills[i].getUnlockedLevel();
                abilityButtons[i, bi].image.color = bi <= skills[i].getUnlockedLevel() ? Color.yellow : Color.gray;
            }
        }
        
        
    }

    public void abilityButtonClick(int skill, int level)
    {
        Debug.Log("Level skill: " + skill + " level: " + level);
        skillManager.getSkill(skill).unlock(level);
    }

    #endregion

    #region QUICK USE ITEMS

    private void updateQuickUseItems()
    {
        for (int i = 0; i < QUICK_USE_ITEM_COUNT; i++)
        {
            if (quickUseItems[i] != null)
            {
                Item item = quickUseItems[i].getItem(inventory);
                if (item != null)
                {
                    quickUseItemImages[i].sprite = item.getIcon();
                    quickUseItemImages[i].enabled = true;

                    if (item is EquipmentItem && ((EquipmentItem)item).isEquiped())
                    {
                        quickUseItemEquipedImages[i].enabled = true;
                    }
                    else
                    {
                        quickUseItemEquipedImages[i].enabled = false;
                    }

                    if (item.getAmount() > 1)
                    {
                        quickUseItemAmount[i].enabled = true;
                        quickUseItemAmount[i].text = "x"+item.getAmount().ToString();
                    }
                    else
                    {
                        quickUseItemAmount[i].enabled = false;
                    }

                    continue;
                }
            }

            quickUseItemImages[i].enabled = false;
            quickUseItemEquipedImages[i].enabled = false;
            quickUseItemImages[i].color = Color.white;
            quickUseItemAmount[i].enabled = false;
        }
    }

    private void onAddedItem(object sender, ItemArgs addedItem)
    {
        for (int i = 0; i < QUICK_USE_ITEM_COUNT; i++)
        {
            if (quickUseItems[i].onAddItem(addedItem.itemType, addedItem.itemIndex, inventory))
            {
                break;
            }
        }
        updateQuickUseItems();
    }

    private void onRemovedItem(object sender, ItemArgs removedItem)
    {
        for (int i = 0; i < QUICK_USE_ITEM_COUNT; i++)
        {
            quickUseItems[i].onRemoveItem(removedItem.itemType, removedItem.itemIndex);
        }
        updateQuickUseItems();
    }

    private void onQuickUseItemUpdate(object sender, System.EventArgs args)
    {
        updateQuickUseItems();
    }

    public void setQuickUseItem(int quickUseIndex)
    {
        if (quickUseIndex < 0 || selectedItem < 0) return;
        int itemIndex = selectedItem;
       
        if (itemIndex < inventory.getEquipmentItems().Count)
        {
            quickUseItems[quickUseIndex] = new QuickUseEquipment(itemIndex, inventory);
            updateQuickUseItems();
            return;
        }
        itemIndex -= inventory.getEquipmentItems().Count;

        if (itemIndex < inventory.getMaterialItems().Count)
        {
            quickUseItems[quickUseIndex] = new QuickUseMaterial(itemIndex, inventory);
            updateQuickUseItems();
            return;
        }
        itemIndex -= inventory.getMaterialItems().Count;
        
        if (itemIndex < inventory.getConsumableItems().Count)
        {
            quickUseItems[quickUseIndex] = new QuickUseConsumable(itemIndex, inventory);
            updateQuickUseItems();
            return;
        }
        itemIndex -= inventory.getConsumableItems().Count;

        updateQuickUseItems();
    }

    public Item getQuickUseItem(int quickUseIndex)
    {
        return quickUseItems[quickUseIndex].getItem(inventory);
    }

    public void quickUseItem(int index)
    {
        if (quickUseItems[index] != null)
        {
            Item item = quickUseItems[index].getItem(inventory);
            if (item is EquipmentItem)
            {
                GameMaster.getGameController().requestItemChange(GameMaster.getPlayerUnitID(), quickUseItems[index].itemIndex);
            }
            else if (item is ConsumableItem)
            {
                GameMaster.getGameController().requestItemConsume(GameMaster.getPlayerUnitID(), quickUseItems[index].itemIndex);
            }
        }
    }

    #endregion //QUICK USE ITEMS

    #region ABILITY

    private void updateAbilityIcons()
    {
        for (int i = 0; i < ABILITY_COUNT; i++)
        {
            abilityIcons[i].enabled = playerHero.hasAbility(i);
        }
    }

    private void updateAbilityCooldownDisplays()
    {
        for (int i = 0; i < ABILITY_COUNT; i++)
        {
            if (playerHero.hasAbility(i))
            {
                Ability a = playerHero.getAbility(i);
                float curCooldown = a.getCurCoolDown();
                if (curCooldown > 0)
                    abilityCooldown[i].localScale = new Vector3(1, curCooldown / a.getCoolDown(), 1);
                else
                    abilityCooldown[i].localScale = new Vector3(0, 0, 0);
            }
            else
            {
                abilityCooldown[i].localScale = new Vector3(0, 0, 0);
            }
        }
    }

    #endregion

    #region INVENTORY

    public void setInventory(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.addedItemListener += new ChangedEventHandler(onAddedItem);
        inventory.removedItemListener += new ChangedEventHandler(onRemovedItem);
        inventory.changeItemListener += new System.EventHandler(onQuickUseItemUpdate);
        itemButtons = new List<Button>[ITEM_TYPE_COUNT];
        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            itemButtons[i] = new List<Button>();
        }

        updateInventory();
        updateQuickUseItems();
    }

    private void updateInventory()
	{
        List<Item>[] items = new List<Item>[ITEM_TYPE_COUNT];
        items[TAB_CRAFTED] = new List<Item>();
        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            items[i] = new List<Item>();
        }
        items[TAB_CRAFTED].AddRange(inventory.getEquipmentItems().Cast<Item>());
        items[TAB_MATERIAL].AddRange(inventory.getMaterialItems().Cast<Item>());
        items[TAB_CONSUMABLE].AddRange(inventory.getConsumableItems().Cast<Item>());

		GameObject prefab = (GameObject)Resources.Load ("GUI/InventoryItemButton");

        int addedIndex = 0;

        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            List<Button> buttons = itemButtons[i];

            for (int j = 0; j < buttons.Count; j++)
            {
                if (j >= items[i].Count)
                {
                    Destroy(buttons[j].gameObject);
                    buttons.RemoveAt(j);
                    j--;
                    continue;
                }

                Item item = items[i][j];
                buttons[j].transform.FindChild("Text").GetComponent<Text>().text = item.getInventoryDisplay();
                Sprite sprite = item.getIcon();
                if (sprite != null)
                {
                    Image image = buttons[j].transform.FindChild("Icon").GetComponent<Image>();
                    image.enabled = true;
                    image.sprite = sprite;
                }
                else
                {
                    buttons[j].transform.FindChild("Icon").GetComponent<Image>().enabled = false;
                }
                int buttonIndex = (j + addedIndex);
                buttons[j].name = INVENTORY_BUTTON_NAME + (buttonIndex);
                buttons[j].onClick.RemoveAllListeners();
                buttons[j].onClick.AddListener(() => inventoryItemButtonClick(buttonIndex));
            }
            while (buttons.Count < items[i].Count)
            {
                int index = buttons.Count;
                int buttonIndex = (index + addedIndex);
                GameObject bo = (GameObject)Instantiate(prefab);
                bo.name = INVENTORY_BUTTON_NAME + (buttonIndex);
                bo.transform.SetParent(itemParents[i]);

                RectTransform rect = bo.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(8, 0 - index * ITEM_LINE_HEIGHT, 0);
                rect.transform.localScale = Vector3.one;

                Item item = items[i][index];
                Text text = bo.transform.FindChild("Text").GetComponent<Text>();
                text.text = item.getInventoryDisplay();
                text.color = itemTextColor;

                Sprite sprite = item.getIcon();
                if (sprite != null)
                {
                    Image image = bo.transform.FindChild("Icon").GetComponent<Image>();
                    image.enabled = true;
                    image.sprite = sprite;
                }
                else
                {
                    bo.transform.FindChild("Icon").GetComponent<Image>().enabled = false;
                }


                Button button = bo.GetComponent<Button>();
                button.onClick.AddListener(() => inventoryItemButtonClick(buttonIndex));

                buttons.Add(button);
            }
            addedIndex += buttons.Count;
        }

        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            float height = itemButtons[i].Count * ITEM_LINE_HEIGHT;
            itemParents[i].sizeDelta = new Vector2(itemParents[i].sizeDelta.x, height);
        }

        updateItemFolds();
	}

    public void updateItemFolds()
    {
        float ypos = 0;
        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            inventoryFolds[i].anchoredPosition = new Vector2(0, -ypos);
            ypos += inventoryFolds[i].sizeDelta.y;
            if (inventoryFoldOpen[i])
            {
                itemParents[i].gameObject.SetActive(true);
                itemParents[i].anchoredPosition = new Vector2(0, -ypos);
                ypos += itemParents[i].GetComponent<RectTransform>().sizeDelta.y;

                inventoryFoldArrows[i].localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                itemParents[i].gameObject.SetActive(false);
                inventoryFoldArrows[i].localEulerAngles = new Vector3(0, 0, 90);
            }
            
        }
        itemHolder.sizeDelta = new Vector2(itemHolder.sizeDelta.x, ypos);
        inventoryScrollRect.vertical = inventoryMask.sizeDelta.y < ypos;
    }

    public void closeInventoryFold(int index)
    {
        inventoryFoldOpen[index] = false;
        updateItemFolds();
    }

    public void openInventoryFold(int index)
    {
        inventoryFoldOpen[index] = true;
        updateItemFolds();
    }

	public void inventoryFoldButtonClick(int index)
	{
        inventoryFoldOpen[index] = !inventoryFoldOpen[index];
        updateItemFolds();
	}

	public void deselectItem()
	{
        if (selectedItem >= 0)
        {
            for (int i = 0; i < ITEM_TYPE_COUNT; i++)
            {
                if (selectedItem < itemButtons[i].Count)
                {
                    itemButtons[i][selectedItem].transform.FindChild("Text").GetComponent<Text>().color = itemTextColor;
                }
                else
                {
                    selectedItem -= itemButtons[i].Count;
                }
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

        if (selectedItem != index)
        {
            //deselect the previous selected item
            deselectItem();
        }

        //selected the new item
        int selectedButton = index;
        for (int i = 0; i < ITEM_TYPE_COUNT; i++)
        {
            if (selectedButton < itemButtons[i].Count)
            {
                itemButtons[i][selectedButton].transform.FindChild("Text").GetComponent<Text>().color = selectedItemTextColor;
                if (doubleclick)
                {
                    switch (i)
                    {
                        case(TAB_CRAFTED):
                            GameMaster.getGameController().requestItemChange(GameMaster.getPlayerUnitID(), selectedButton);
                            break;
                        case(TAB_MATERIAL):
                            break;
                        case(TAB_CONSUMABLE):
                            GameMaster.getGameController().requestItemConsume(GameMaster.getPlayerUnitID(), selectedButton);
                            break;
                        default:
                            break;

                    }
                }
                break;
            }
            else
            {
                selectedButton -= itemButtons[i].Count;
            }
        }
       
		lastClickedItem = index;
		selectedItem = index;
	}

    public int getSelectedItemIndex()
    {
        return selectedItem;
    }

    #endregion //INVENTORY

    #region CRAFTING

    private void updateCrafting()
	{
        List<RecipeData>[] recipes = new List<RecipeData>[RECIPE_TYPE_COUNT];
        recipes[TAB_CRAFTED] = new List<RecipeData>();
        for (int i = 0; i < RECIPE_TYPE_COUNT; i++)
        {
            recipes[i] = new List<RecipeData>();
        }
        recipes[TAB_CRAFTED].AddRange(DataHolder.Instance.getEquipmentRecipeData().Cast<RecipeData>());
        recipes[TAB_MATERIAL].AddRange(DataHolder.Instance.getMaterialRecipeData().Cast<RecipeData>());
        recipes[TAB_CONSUMABLE].AddRange(DataHolder.Instance.getConsumableRecipeData().Cast<RecipeData>());

        GameObject prefab = (GameObject)Resources.Load("GUI/CraftingButton");

        int addedIndex = 0;

        for (int i = 0; i < RECIPE_TYPE_COUNT; i++)
        {
            List<Button> buttons = craftingButtons[i];
            
            while (buttons.Count < recipes[i].Count)
            {
                int index = buttons.Count;
                int buttonIndex = (index + addedIndex);
                GameObject bo = (GameObject)Instantiate(prefab);
                bo.name = CRAFTING_BUTTON_NAME + (buttonIndex);
                bo.transform.SetParent(recipeParents[i]);

                RectTransform rect = bo.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(8, 0 - index * ITEM_LINE_HEIGHT, 0);
                rect.transform.localScale = Vector3.one;

                RecipeData recipe = recipes[i][index];
                Text text = bo.transform.FindChild("Text").GetComponent<Text>();
                text.text = recipe.gameName;
                text.color = itemTextColor;

                ItemData item = DataHolder.Instance.getItemData(recipe.product);
                if (item != null)
                {
                    Sprite sprite = item.getIcon();
                    if (sprite != null)
                    {
                        Image image = bo.transform.FindChild("Icon").GetComponent<Image>();
                        image.enabled = true;
                        image.sprite = sprite;
                    }
                    else
                    {
                        bo.transform.FindChild("Icon").GetComponent<Image>().enabled = false;
                    }
                }
                else
                {
                    bo.transform.FindChild("Icon").GetComponent<Image>().enabled = false;
                }


                Button button = bo.GetComponent<Button>();
                button.onClick.AddListener(() => craftItemButtonClick(recipe.name));

                buttons.Add(button);
            }
            addedIndex += buttons.Count;
        }

        for (int i = 0; i < RECIPE_TYPE_COUNT; i++)
        {
            float height = craftingButtons[i].Count * ITEM_LINE_HEIGHT;
            recipeParents[i].sizeDelta = new Vector2(recipeParents[i].sizeDelta.x, height);
        }

        updateRecipeFolds();
#if false
        Debug.Log("Update Crafting");
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
#endif
	}

    public void craftingTabButtonClick(int index)
    {
        Debug.Log("clicked tab " + index);
        selectedCraftingTab = index;
        onSelectCraftingTab();
    }


    public void updateRecipeFolds()
    {
        float ypos = 0;
        for (int i = 0; i < RECIPE_TYPE_COUNT; i++)
        {
            recipeFolds[i].anchoredPosition = new Vector2(0, -ypos);
            ypos += recipeFolds[i].sizeDelta.y;
            if (recipeFoldOpen[i])
            {
                recipeParents[i].gameObject.SetActive(true);
                recipeParents[i].anchoredPosition = new Vector2(0, -ypos);
                ypos += recipeParents[i].GetComponent<RectTransform>().sizeDelta.y;

                recipeFoldArrows[i].localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                recipeParents[i].gameObject.SetActive(false);
                recipeFoldArrows[i].localEulerAngles = new Vector3(0, 0, 90);
            }

        }
        recipeHolder.sizeDelta = new Vector2(recipeHolder.sizeDelta.x, ypos);
        craftingScrollRect.vertical = craftingMask.sizeDelta.y < ypos;
    }

    public void recipeFoldButtonClick(int index)
    {
        recipeFoldOpen[index] = !recipeFoldOpen[index];
        updateRecipeFolds();
    }

    public void onSelectCraftingTab()
    {
#if false
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
#endif
    }

	public void craftItemButtonClick(string name)
	{
		if(inventory.hasIngredients(DataHolder.Instance.getRecipeData(name).ingredients))
		{
			Debug.Log ("can craft");
			GameMaster.getGameController().requestItemCraft(GameMaster.getPlayerUnitID(), name);
		}
		else
		{
			Debug.Log ("cant afford this");
		}

	}

    #endregion //CRAFTING

    #region TOGGLE WINDOWS

    public void toggleCrafting()
    {
        activateInventory(!inventoryActive);
        activateCrafting(!inventoryActive);
    }

    public void toggleInventory()
    {
        activateInventory(!inventoryActive);
        activateCrafting(!inventoryActive);
    }

    public void toggleHeroStats()
    {
        activateHeroStats(!heroStatsActive);
    }

    public void toggleAbilityWindow()
    {
        activateAbilityWindow(!abilityWindowActive);
    }

	public void activateInventory(bool active)
	{
		inventoryActive = active;
		inventoryObject.SetActive(inventoryActive);
        craftingObject.SetActive(inventoryActive);
	}

	public void activateCrafting(bool active)
	{
        
	}

    public void activateHeroStats(bool active)
    {
        heroStatsActive = active;
        heroStatsObject.SetActive(heroStatsActive);
    }

    public void activateAbilityWindow(bool active)
    {
        abilityWindowActive = active;
        abilityWindowObject.SetActive(abilityWindowActive);
    }

    #endregion //TOGGLE WINDOWS

    #region CHAT

    public void addChatMessage(string msg)
    {
        chatOutputText.text += msg + "\n";
        chatCloseTime = Time.time + 5;
        chatObject.SetActive(true);
        chatOutputOpen = true;
    }

    public void toggleChatInput()
    {
        if (chatting)
        {
            //send message
            if (chatInputField.text.StartsWith("!"))
            {
                CheatCommand.sendCommandFromString(chatInputField.text.Substring(1));
            }
            else if (!chatInputField.text.Trim().Equals(""))
            {
                GameMaster.getGameController().sendChatMessage(chatInputField.text);
            }
            
            chatting = false;

        }
        else
        {
            chatting = true;
            chatObject.SetActive(true);
            chatInputObject.SetActive(true);
            justOpenedChat = true;
        }

    }

    #endregion //CHAT

    public bool isMouseOverGUI()
    {
        return mouseOverGUI;
    }

    public bool takeKeyboardInput()
    {
        return !chatting;
    }

    public void setPlayerHero(Hero hero)
    {
        this.playerHero = hero;

        setInventory(hero.getInventory());
        setUnitStats(hero.getUnitStats());
        setSkillManager(hero.getSkillManager());

        updateAbilityIcons();
    }
}
