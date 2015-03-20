using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameController : MonoBehaviour{

	protected GameMaster gameMaster;

	protected int unitID;
	private float clickTimer = 0f;
	private readonly float clickTime = 0.033f;
	private bool targetSelected = false;
	private Vector3 targetPosition = new Vector3();
	private int targetUnitID = -1;
	private string targetTag;
	private readonly int RIGHT_MOUSE_BUTTON = 1;
	private readonly int LEFT_MOUSE_BUTTON = 0;
	private Renderer targetRenderer;
	private Texture2D moveCursor;
	private Texture2D chopCursor;
	private Texture2D lootCursor;
	private Texture2D attackCursor;



	public void init(GameMaster gameMaster, int unitID)
	{
		this.gameMaster = gameMaster;
		this.unitID = unitID;

		moveCursor = Resources.Load<Texture2D>("GUI/Cursors/move_cursor");
		chopCursor = Resources.Load<Texture2D>("GUI/Cursors/chop_cursor");
		lootCursor = Resources.Load<Texture2D>("GUI/Cursors/loot_cursor");
		attackCursor = Resources.Load<Texture2D>("GUI/Cursors/attack_cursor");
	}

    #region UNIT SPAWNING

    //Spawn a unit
    [RPC]
    public abstract void requestAIUnitSpawn(int unitID, string name, float x, float y);

    [RPC]
    public void approveAIUnitSpawn(int unitID, string name, float x, float y)
    {
        AIUnit unit = new AIUnit(name, new Vector3(x, 0, y), Vector3.zero, unitID);
		GameMaster.addUnit(unit);
    }

    //Spawner respawns all units that are dead
    public abstract void requestSpawnerRespawn(int spawnerID);
    public abstract void requestDaySpawnerRespawn(int spawnerID);
    public abstract void requestNightSpawnerRespawn(int spawnerID);

    [RPC]
    public void approveSpawnerRespawn(int spawnerID)
    {
        GameMaster.spawnerRespawn(spawnerID);
    }

    [RPC]
    public void approveDaySpawnerRespawn(int spawnerID)
    {
        GameMaster.daySpawnerRespawn(spawnerID);
    }

    [RPC]
    public void approveNightSpawnerRespawn(int spawnerID)
    {
        GameMaster.nightSpawnerRespawn(spawnerID);
    }

    //Spawner removes all its units
    public abstract void requestSpawnerRemoveAll(int spawnerID);
    public abstract void requestDaySpawnerRemoveAll(int spawnerID);
    public abstract void requestNightSpawnerRemoveAll(int spawnerID);

    [RPC]
    public void approveSpawnerRemoveAll(int spawnerID)
    {
        GameMaster.spawnerRemoveAll(spawnerID);
    }
    [RPC]
    public void approveDaySpawnerRemoveAll(int spawnerID)
    {
        GameMaster.daySpawnerRemoveAll(spawnerID);
    }
    [RPC]
    public void approveNightSpawnerRemoveAll(int spawnerID)
    {
        GameMaster.nightSpawnerRemoveAll(spawnerID);
    }

    #endregion

    #region UNIT COMMANDS

    [RPC]
	public abstract void requestMoveCommand(int unitID, float x, float y);
	
	[RPC]
	protected void approveMoveCommand(int unitID, float goalX, float goalY, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getUnit (unitID);
		if(unit == null) return;
		if(unit.getTile() != startTile) 
		{
			unit.setPosition(startPos); //If desynced: SYNC THAT
		}

		unit.giveMoveCommand(new Vector2(goalX,goalY));
	}

	[RPC]
	public abstract void requestGatherCommand(int unitID, float goalX, float goalY);

	[RPC]
	protected void approveGatherCommand(int unitID, float goalX, float goalY,Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getUnit (unitID);
		if(unit.getTile() != startTile) 
		{
			unit.setPosition(startPos); //If desynced: SYNC THAT
			//Debug.Log("desync!");
		}

		unit.giveGatherCommand(new Vector2(goalX, goalY));
	}

    [RPC]
    public abstract void requestActionCommand(int unitID, float goalX, float goalY);

    [RPC]
    protected void approveActionCommand(int unitID, float goalX, float goalY, Vector3 startPos)
    {
        Vector2i startTile = new Vector2i(startPos.x, startPos.z);
        Unit unit = GameMaster.getUnit(unitID);
        if (unit.getTile() != startTile)
        {
            unit.setPosition(startPos);
        }

        WarpObject warpObject = World.tileMap.getTile((int)goalX, (int)goalY).getTileObject() as WarpObject;
        if(warpObject != null) unit.giveCommand(new ActionCommand(unit, warpObject));
    }

	[RPC]
	public abstract void requestAttackCommand(int unitID, int targetID);

	[RPC]
	public abstract void requestRangedAttackCommand(int unitID, Vector3 target);

	[RPC]
	protected void approveAttackCommand(int unitID, int targetID, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getUnit (unitID);
		if(unit.getTile() != startTile) 
		{
			unit.setPosition(startPos); //If desynced: SYNC THAT
			//Debug.Log("desync!");
		}
		
		unit.giveAttackCommand(GameMaster.getUnit(targetID));
	}

	[RPC]
	protected void approveRangedAttackCommand(int unitID, Vector3 target, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getUnit (unitID);
		if(unit.getTile() != startTile) 
		{
			unit.setPosition(startPos); //If desynced: SYNC THAT
			//Debug.Log("desync!");
		}
		
		unit.giveRangedAttackCommand(target);
	}



	[RPC]
	public abstract void requestLootCommand(int unitID, int tileX, int tileY, int lootID);

	[RPC]
	protected void approveLootCommand(int unitID, int tileX, int tileY, int lootID, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getUnit(unitID);
		if(Vector2i.getManhattan(unit.getTile (), startTile) > 1)
		{
			unit.setPosition(startPos);
			//Debug.Log ("desync!");		
		}

		unit.giveLootCommand(new Vector2i(tileX, tileY), lootID);
	}

    [RPC]
    protected void sendMoveCommandWithLag(int unitID, float x, float y, float lag)
    {
        StartCoroutine(lagMove(unitID, x, y, lag));
    }

    [RPC]
    public abstract void requestAbilityCommand(int unitID, int targetID, int ability);

    [RPC]
    public abstract void requestAbilityCommand(int unitID, Vector3 target, int ability);

    [RPC]
    protected void approveAbilityCommand(int unitID, int targetID, Vector3 startPos, int ability)
    {
        Vector2i startTile = new Vector2i(startPos.x, startPos.z);
        Unit unit = GameMaster.getUnit(unitID);
        if (unit.getTile() != startTile)
        {
            unit.setPosition(startPos); //If desynced: SYNC THAT
            //Debug.Log("desync!");
        }

        unit.giveAbilityCommand(GameMaster.getUnit(targetID), ability);
    }

    [RPC]
    protected void approveAbilityCommand(int unitID, Vector3 target, Vector3 startPos, int ability)
    {
        Vector2i startTile = new Vector2i(startPos.x, startPos.z);
        Unit unit = GameMaster.getUnit(unitID);
        if (unit.getTile() != startTile)
        {
            unit.setPosition(startPos); //If desynced: SYNC THAT
            //Debug.Log("desync!");
        }

        unit.giveAbilityCommand(target, ability);
    }

    #endregion

    public abstract void update();

	protected void updateInput()
	{
		if(clickTimer > 0) clickTimer -= Time.deltaTime;

        if (Input.GetKeyDown("return"))
        {
            gameMaster.getGUIManager().toggleChatInput();
        }
        if (gameMaster.getGUIManager().takeKeyboardInput())
        {
            if (Input.GetKeyDown("s"))
            {
                GameMaster.getPlayerHero().giveCommand(new StunnedCommand(GameMaster.getPlayerHero(), 2));
            }
            if (Input.GetKeyDown("i"))
            {
                gameMaster.getGUIManager().toggleInventory();
            }
            
            if (Input.GetKeyDown("c"))
            {
                gameMaster.getGUIManager().toggleHeroStats();
                //gameMaster.getGUIManager().toggleCrafting();
            }
            if (Input.GetKeyDown("p"))
            {
                gameMaster.getGUIManager().toggleAbilityWindow();
            }
            if (Input.GetKeyDown("escape"))
            {
                gameMaster.getGUIManager().toggleIngameMenu();
            }
            if (Input.GetKeyDown("g"))
            {
                GameMaster.getWorld().toggleDrawGrid();
            }
            if (Input.GetKey("up"))
            {
                GameMaster.getWorld().changeSnowAmount(Time.deltaTime * 0.5f);
            }
            if (Input.GetKey("down"))
            {
                GameMaster.getWorld().changeSnowAmount(Time.deltaTime * -0.5f);
            }

            //item quick use
            for (int i = 0; i < GUIManager.QUICK_USE_ITEM_COUNT; i++)
            {
                if (Input.GetKeyDown((i+1).ToString()))
                {
                    if (Input.GetKey("left shift"))
                    {
                        gameMaster.getGUIManager().setQuickUseItem(i);
                    }
                    else
                    {
                        gameMaster.getGUIManager().quickUseItem(i);
                    }
                }
            }
        }
		
		if(!gameMaster.getGUIManager().isMouseOverGUI())
		{

			RaycastHit rayhit;
			if(targetRenderer != null)
			{
				foreach(Material mat in targetRenderer.materials)
				{
					mat.SetFloat("_Highlight", 0);
				}
			}
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayhit))
			{
				Vector3 targetPoint = rayhit.point;

				string holdTag = rayhit.collider.tag;
				if(holdTag == "Loot")
				{
					Cursor.SetCursor(lootCursor, Vector2.zero, CursorMode.Auto);
					targetRenderer = rayhit.collider.transform.GetComponentInChildren<Renderer>();
					foreach(Material mat in targetRenderer.materials)
					{
						mat.SetFloat("_Highlight", 1);
					}

					targetPoint = rayhit.transform.position;

				}
				else if(holdTag == "Resource")
				{
					Cursor.SetCursor(chopCursor, Vector2.zero, CursorMode.Auto);
					targetRenderer = rayhit.collider.transform.GetComponentInChildren<Renderer>();
					foreach(Material mat in targetRenderer.materials)
					{
						mat.SetFloat("_Highlight", 1);
					}

					targetPoint = rayhit.transform.position;
				}
                else if (holdTag == "Action")
                {
                    Cursor.SetCursor(moveCursor, Vector2.zero, CursorMode.Auto);
                    targetRenderer = rayhit.collider.transform.GetComponentInChildren<Renderer>();
                    foreach (Material mat in targetRenderer.materials)
                    {
                        mat.SetFloat("_Highlight", 1);
                    }

                    targetPoint = rayhit.transform.position;
                }
                else if (holdTag == "Unit")
                {
                    Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                    targetRenderer = rayhit.collider.transform.GetComponentInChildren<Renderer>();
                    foreach (Material mat in targetRenderer.materials)
                    {
                        mat.SetFloat("_Highlight", 1);
                    }
                    if (gameMaster.getGUIManager().takeKeyboardInput())
                    {
                        if (Input.GetKey("q"))
                        {
                            int targetID = rayhit.transform.GetComponent<UnitController>().getID();
                            requestAbilityCommand(unitID, targetID, 0);
                        }
                        else if (Input.GetKey("w"))
                        {
                            int targetID = rayhit.transform.GetComponent<UnitController>().getID();
                            requestAbilityCommand(unitID, targetID, 1);
                        }
                        else if (Input.GetKey("e"))
                        {
                            int targetID = rayhit.transform.GetComponent<UnitController>().getID();
                            requestAbilityCommand(unitID, targetID, 2);
                        }
                        else if (Input.GetKey("r"))
                        {
                            int targetID = rayhit.transform.GetComponent<UnitController>().getID();
                            requestAbilityCommand(unitID, targetID, 3);
                        }
                    }
                    targetPoint = rayhit.transform.position;
                }
                else if (holdTag == "Ground")
                {
                    Cursor.SetCursor(moveCursor, Vector2.zero, CursorMode.Auto);

                    if (gameMaster.getGUIManager().takeKeyboardInput())
                    {
                        if (Input.GetKey("q"))
                        {
                            requestAbilityCommand(unitID, targetPoint + Vector3.up, 0);
                        }
                        else if (Input.GetKey("w"))
                        {
                            requestAbilityCommand(unitID, targetPoint + Vector3.up, 1);
                        }
                        else if (Input.GetKey("e"))
                        {
                            requestAbilityCommand(unitID, targetPoint + Vector3.up, 2);
                        }
                        else if (Input.GetKey("r"))
                        {
                            requestAbilityCommand(unitID, targetPoint + Vector3.up, 3);
                        }
                    }
                }

				
			}
			//Clicking a target with LMB will set it selected until the button is released. 
			//While selected no other command can be excecuted
			if(Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
			{
				RaycastHit hit;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
				{
					targetTag = hit.collider.tag;
					if(targetTag == "Loot")
					{
						targetSelected = true;
						targetPosition = hit.transform.position;
						int lootID = hit.transform.GetComponent<LootId>().getId();
						requestLootCommand(unitID, Mathf.FloorToInt(targetPosition.x), Mathf.FloorToInt(targetPosition.z), lootID);					
					}
					else if(targetTag == "Resource")
					{
						if(GameMaster.getPlayerHero().isMelee())
						{
							targetSelected = true;
							targetPosition = hit.transform.position;
							requestGatherCommand(unitID, targetPosition.x, targetPosition.z);
						}
						else
						{
							targetPosition = hit.transform.position;
							requestMoveCommand(unitID, targetPosition.x, targetPosition.z);
						}

					}
                    else if (targetTag == "Action")
                    {
                        targetSelected = true;
                        targetPosition = hit.transform.position;
                        requestActionCommand(unitID, targetPosition.x, targetPosition.z);
                    }
                    else if (targetTag == "Unit")
                    {
                        targetSelected = false;
                        UnitController target = hit.transform.GetComponent<UnitController>();
                        targetUnitID = target.getID();
                        if (targetUnitID != unitID)
                        {
                            if (GameMaster.getPlayerHero().isMelee())
                            {
                                requestAttackCommand(unitID, targetUnitID);
                            }
                            else
                            {
                                requestRangedAttackCommand(unitID, target.transform.position + Vector3.up);
                            }
                        }
                        else
                        {
                            Debug.Log("Target is null");
                            targetUnitID = -1;
                        }
                    }
                    else if (targetTag == "Ground")
                    {
                        targetSelected = false;
                        requestMoveCommand(unitID, hit.point.x, hit.point.z);
                    }
					
				}
			}
			else if( Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON) 
			   || (Input.GetMouseButton(RIGHT_MOUSE_BUTTON) && clickTimer <= 0 )
			   || (Input.GetMouseButton(LEFT_MOUSE_BUTTON) && !targetSelected && targetUnitID < 0 && clickTimer <= 0)
			   || Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
			{
				clickTimer = clickTime;
				RaycastHit hit;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, 1 << 8))
				{
					string tag = hit.collider.tag;
					if(tag == "Ground")
					{
						requestMoveCommand(unitID, hit.point.x, hit.point.z);
					}
				}
			}

			else if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
			{
				targetSelected = false;
				targetUnitID = -1;
			}
			else if(Input.GetMouseButton(LEFT_MOUSE_BUTTON) && clickTimer <= 0)
			{
				if(targetSelected)
				{
					if(targetTag == "Resource")
					{
						if(GameMaster.getPlayerHero().isMelee())
						{
							requestGatherCommand(unitID, targetPosition.x, targetPosition.z);
						}
						else
						{
							requestMoveCommand(unitID, targetPosition.x, targetPosition.z);
						}
					}
                    else if (targetTag == "Action")
                    {
                        requestActionCommand(unitID, targetPosition.x, targetPosition.z);
                    }

				}
				else if(targetUnitID >= 0)
				{
					if(GameMaster.getPlayerHero().isMelee())
					{
						requestAttackCommand(unitID, targetUnitID);
					}
					else 
					{
						Unit target = GameMaster.getUnit(targetUnitID);
						if(target != null)
						requestRangedAttackCommand(unitID,  target.getPosition() + Vector3.up);
					}
				}

			}
		}
	}

    [RPC]
    public abstract void requestRemoveUnit(int unitID);

    [RPC]
    public void approveRemoveUnit(int unitID)
    {
        Unit unit = GameMaster.getUnit(unitID);
        unit.setAlive(false);
    }

    #region COMBAT

    [RPC]
	protected void killUnit(int targetID, int unitID)
	{
		Unit unit = GameMaster.getUnit (targetID); 
		unit.setAlive(false);
		requestUnitLootDrop(unit.getName(), unit.getTile());
	}
	
	[RPC]
	protected void hitUnit(int targetID, int unitID, int damage)
	{
		GameMaster.getUnit (targetID).takeDamage(damage, unitID);
	}

    public abstract void requestAttack(int unitID, int targetID);

    public abstract void requestFireProjectile(int unitID, Vector3 target);

    public abstract void requestFireProjectile(int unitID, Vector3 target, int damage, string projectileName);

    [RPC]
    protected void approveFireProjectile(int unitID, Vector3 goal, string name, int damage)
    {
        GameMaster.addProjectile(unitID, goal, name, damage);
    }

    public abstract void requestProjectileHit(int damage, int unitID, int targetID);

    public abstract void requestHit(int damage, int unitID, int targetID);

    [RPC]
    public abstract void requestLearnAbility(string ability, int unitID);

    [RPC]
    protected void approveLearnAbility(string ability, int unitID)
    {
        GameMaster.getHero(unitID).addAbility(ability);
    }

    #endregion

    #region BUFFS

    //public abstract void requestAddBuff(int unitID, params object[] parameters)
    #endregion

    #region GATHERING

    public abstract void requestGather(int unitID, Vector2i tile);

    [RPC]
    protected void hitResource(int x, int y, int damage)
    {
        World.tileMap.getTile(x, y).getResourceObject().dealDamage(damage);
    }

    [RPC]
    protected void gatherResource(int x, int y, int unitID)
    {
        string name = World.tileMap.getTile(x, y).removeTileObject();
        requestResourceLootDrop(name, new Vector2i(x, y), unitID);
    }

    public abstract void requestResourceLootDrop(string loot, Vector2i tile, int unitID);

    [RPC]
    protected void dropResourceLoot(string resourceName, int seed, int x, int y, int unitID)
    {
        GameMaster.addResourceLoot(resourceName, seed, x, y, unitID);
    }

    

    #endregion

    #region LOOT
    //REGION LOOT

    public abstract void requestLoot(int unitID, Vector2i tile, int lootID);

    [RPC]
	protected void lootObject(int unitID, int tileX, int tileY, int lootID)
	{
		Item loot = World.tileMap.getTile(tileX, tileY).removeAndReturnLootObject(lootID);
		if(loot != null)
		{
			GameMaster.getHero(unitID).getInventory().addItem(loot);
		}
	}

    public abstract void requestUnitLootDrop(string loot, Vector2i tile);

    [RPC]
    protected void dropUnitLoot(string unitName, int seed, int x, int y)
    {
        GameMaster.addUnitLoot(unitName, seed, x, y);
    }

    #endregion

    #region CHANGE UNIT STATS
    //REGION CHANGE UNIT STATS

    [RPC]
    protected void giveExperience(int targetID, int skill, int experience)
    {
        Hero hero = GameMaster.getHero(targetID);
        if (hero != null)
        {
            hero.grantExperience(skill, experience);
        }
    }

    [RPC]
    protected void changeHunger(int targetID, int food)
    {
        Hero hero = GameMaster.getHero(targetID);
        if (hero != null)
        {
            hero.changeHunger(food);
        }
    }

    [RPC]
    protected void changeEnergy(int targetID, int energy)
    {
        Hero hero = GameMaster.getHero(targetID);
        if (hero != null)
        {
            hero.changeEnergy(energy);
        }
    }
    #endregion

    #region INVENTORY AND CRAFTING
    //REGION INVENTORY AND CRAFTING

    [RPC]
	public abstract void requestItemCraft(int unitID, string name);

	[RPC]
	protected void approveItemCraft(int unitID, string name)
	{
		Inventory inv = GameMaster.getHero(unitID).getInventory();
		inv.craftItem(DataHolder.Instance.getRecipeData(name));
	}

	[RPC]
	public abstract void requestItemChange(int unitID, int itemIndex);

	[RPC]
	protected void approveItemChange(int unitID, int itemIndex)
	{
		Hero hero = GameMaster.getHero(unitID);
		hero.setItem(itemIndex);
	}

	[RPC]
	public abstract void requestItemConsume(int unitID, int itemIndex);

	[RPC]
	protected void approveItemConsume(int unitID, int itemIndex)
	{
		Hero hero = GameMaster.getHero(unitID);
		ConsumableItem item = hero.getInventory().getConsumableItems()[itemIndex];

		hero.changeHunger(item.getHungerChange());
		hero.getInventory().consumeItem(itemIndex);
	}

    #endregion

    #region CHAT AND CHEAT 
    //REGION CHAT AND CHEAT

    public abstract void requestWarp(int unitID, Vector2i tile);

    [RPC]
    protected void warpUnit(int unitID, float x, float y, float rot = 0)
    {
        Unit unit = GameMaster.getUnit(unitID);
        if (unit != null)
        {
            unit.warp(new Vector2(x, y));
            unit.setRotation(new Vector3(0, rot, 0));
        }
    }

    [RPC]
    public abstract void requestCheatCommand(int unitID, int commandID, string parameters);

    [RPC]
    protected void approveCheatCommand(int unitID, int commandID, string parameterString)
    {
        object[] parameters = CheatCommand.getParametersFromString(commandID, parameterString);
        if (parameters == null) return;
        Hero commander = GameMaster.getHero(unitID);
        if(commander == null) return;
        switch(commandID)
        {
            case(CheatCommand.SPAWN):
            {
                AIUnitData unitData = DataHolder.Instance.getAIUnitData((string)parameters[0]);
                if(unitData == null) return;
                GameMaster.addUnit(new AIUnit(unitData.name, commander.getPosition() + Vector3.forward, Vector3.zero, GameMaster.getNextUnitID()));
            }
            break;

            case(CheatCommand.GIVE):
            {
                ItemData itemData = DataHolder.Instance.getItemData((string)parameters[0]);
                if(itemData == null) return;
                int amount = 1;
                if(parameters.Length > 1) amount = (int)parameters[1];
                for(int i = 0; i < amount; i++)
                {
                    commander.getInventory().addItem(itemData.getItem());
                }
            }
            break;

            case(CheatCommand.WARP):
            {
                if(parameters.Length != 2) return;
                commander.warp(new Vector2((float)parameters[0],(float)parameters[1]));
            }
            break;

            case (CheatCommand.ADDXP):
            {
                if (parameters.Length != 2) return;
                int skillIndex = DataHolder.Instance.getSkillIndex((string)parameters[0]);
                if (skillIndex >= 0) commander.grantExperience(skillIndex, (int)parameters[1]);
            }
            break;
        }
    }

    public abstract void sendChatMessage(string msg);

    [RPC]
    public void recieveChatMessage(int unitID, string msg)
    {
        gameMaster.getGUIManager().addChatMessage(unitID + ": " + msg);
    }

    #endregion

    protected abstract IEnumerator lagMove(int unitID, float x, float y, float lag);


}
