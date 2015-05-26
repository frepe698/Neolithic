using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public abstract class GameController : MonoBehaviour{

	protected GameMaster gameMaster;

	protected int unitID;
	private float clickTimer = 0f;
	private readonly float clickTime = 0.033f;
	private bool targetSelected = false;
	private Vector3 targetPosition = new Vector3();
	private int targetUnitID = -1;
	private string targetTag;
    private bool attackingGround = false;
    private Vector3 attackPosition;

    //Building placing
    private BuildingRecipeData currentBuildingData;
    private bool isPlacingBuilding;
    private GameObject buildingGhost;

	private static readonly int RIGHT_MOUSE_BUTTON = 1;
	private static readonly int LEFT_MOUSE_BUTTON = 0;
    private static readonly string ATTACK_GROUND = "left shift";
    private static readonly string[] ABILITY_INPUT = new string[] 
    {
        "q", "w", "e", "r"
    };
    public static int[] playerAbilityMapping = new int[]
    {
        0, 1, 2, 3
    };
	private Renderer[] targetRenderers;
	private Texture2D moveCursor;
	private Texture2D chopCursor;
	private Texture2D lootCursor;
	private Texture2D attackCursor;


	public void init(GameMaster gameMaster, int unitID)
	{
		this.gameMaster = gameMaster;
		this.unitID = unitID;

        buildingGhost = GameObject.Find("buildingGhost");
        if (buildingGhost == null) Debug.LogError("No building ghost found");
        else buildingGhost.SetActive(false);

		moveCursor = Resources.Load<Texture2D>("GUI/Cursors/move_cursor");
		chopCursor = Resources.Load<Texture2D>("GUI/Cursors/chop_cursor");
		lootCursor = Resources.Load<Texture2D>("GUI/Cursors/loot_cursor");
		attackCursor = Resources.Load<Texture2D>("GUI/Cursors/attack_cursor");
	}

    public abstract void requestGameStart();

    [RPC]
    protected void approveGameStart()
    {
        gameMaster.startGame();
    }

    [RPC]
    public abstract void setPlayerLoaded(int playerID);
        
    #region UNIT SPAWNING

    public abstract void requestLaneSpawning();

    [RPC]
    public void approveLaneSpawning()
    {
        GameMaster.startSpawning();
    }

    public abstract void requestAILevelIncrease();
    
    [RPC]
    protected void approveAILevelIncrease()
    {
        GameMaster.increaseAILevel();
    }
    
    public abstract void requestDamageBase(int team, int damage, int unitID);

    [RPC]
    public void approveDamageBase(int team, int damage, int unitID)
    {
        GameMaster.damageBase(team, damage);
        requestRemoveActor(unitID);
    }

    public abstract void requestAddFavour(int team, int favour);

    [RPC]
    public void approveAddFavour(int team, int favour)
    {
        GameMaster.addFavour(team, favour);
    }

    public abstract void requestHeroStartRespawn(int unitID);
    public abstract void requestRespawnHero(int unitID);

    [RPC]
    public void approveHeroStartRespawn(int unitID)
    {
        GameMaster.startHeroRespawn(unitID);
    }

    [RPC]
    public void approveRespawnHero(int unitID)
    {
        GameMaster.respawnHero(unitID);
    }
    //Spawn a unit
    [RPC]
    public abstract void requestAIUnitSpawn(int unitID, string name, float x, float y);

    [RPC]
    public void approveAIUnitSpawn(int unitID, string name, float x, float y)
    {
        AIUnit unit = new AIUnit(name, new Vector3(x, 0, y), Vector3.zero, unitID);
		GameMaster.addActor(unit);
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
		Unit unit = GameMaster.getActor (unitID) as Unit;
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
		Unit unit = GameMaster.getActor (unitID) as Unit;
        if (unit == null) return;
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
        Unit unit = GameMaster.getActor(unitID) as Unit;
        if (unit == null) return;
        if (unit.getTile() != startTile)
        {
            unit.setPosition(startPos);
        }

        WarpObject warpObject = World.tileMap.getTile((int)goalX, (int)goalY).getTileObject() as WarpObject;
        if(warpObject != null) unit.giveCommand(new ActionCommand(unit, warpObject));
    }

	[RPC]
	public abstract void requestAttackCommandUnit(int unitID, int targetID);

	[RPC]
	public abstract void requestAttackCommandPos(int unitID, Vector3 target);

	[RPC]
	protected void approveAttackCommandUnit(int unitID, int targetID, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Actor actor = GameMaster.getActor (unitID);
		if(actor.getTile() != startTile) 
		{
			actor.setPosition(startPos); //If desynced: SYNC THAT
			//Debug.Log("desync!");
		}
		
		actor.giveAttackCommand(GameMaster.getActor(targetID));
	}

	[RPC]
	protected void approveAttackCommandPos(int unitID, Vector3 target, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Actor actor = GameMaster.getActor (unitID);
		if(actor.getTile() != startTile) 
		{
			actor.setPosition(startPos); //If desynced: SYNC THAT
			//Debug.Log("desync!");
		}
		
		actor.giveAttackCommand(target);
	}



	[RPC]
	public abstract void requestLootCommand(int unitID, int tileX, int tileY, int lootID);

	[RPC]
	protected void approveLootCommand(int unitID, int tileX, int tileY, int lootID, Vector3 startPos)
	{
		Vector2i startTile = new Vector2i(startPos.x, startPos.z);
		Unit unit = GameMaster.getActor(unitID) as Unit;
        if (unit == null) return;
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
    public abstract void requestAbilityCommandID(int unitID, int targetID, int ability);

    [RPC]
    public abstract void requestAbilityCommandVec(int unitID, Vector3 target, int ability);

    [RPC]
    protected void approveAbilityCommandID(int unitID, int targetID, Vector3 startPos, int ability)
    {
        Vector2i startTile = new Vector2i(startPos.x, startPos.z);
        Actor actor = GameMaster.getActor(unitID);
        if (actor.getTile() != startTile)
        {
            actor.setPosition(startPos); //If desynced: SYNC THAT
            //Debug.Log("desync!");
        }

        actor.giveAbilityCommand(GameMaster.getActor(targetID), ability);
    }

    [RPC]
    protected void approveAbilityCommandVec(int unitID, Vector3 target, Vector3 startPos, int ability)
    {
        Vector2i startTile = new Vector2i(startPos.x, startPos.z);
        Actor unit = GameMaster.getActor(unitID);
        if (unit.getTile() != startTile)
        {
            unit.setPosition(startPos); //If desynced: SYNC THAT
            //Debug.Log("desync!");
        }

        unit.giveAbilityCommand(target, ability);
    }

    #endregion

    public abstract void update();

    private void setHighlightOnTarget(int highlight)
    {
        if (targetRenderers != null)
        {
            foreach (Renderer r in targetRenderers)
            {
                if (r == null) break;
                foreach (Material m in r.materials)
                {
                    m.SetFloat("_Highlight", highlight);
                }
            }
        }
    }

	protected void updateInput()
	{
		if(clickTimer > 0) clickTimer -= Time.deltaTime;

        if (Input.GetKeyDown("return"))
        {
            GameMaster.getGUIManager().toggleChatInput(Input.GetKey("left shift"), Input.GetKey("left ctrl"));
        }
        if (GameMaster.getGUIManager().takeKeyboardInput())
        {
            if (Input.GetKeyDown("i") || Input.GetKeyDown("c"))
            {
                GameMaster.getGUIManager().toggleInventory();
            }
            if (Input.GetKeyDown("b"))
            {
                GameMaster.getGUIManager().toogleBuilding();
            }
            
            if (Input.GetKeyDown("k"))
            {
                GameMaster.getGUIManager().toggleHeroStats();
            }
            if (Input.GetKeyDown("p"))
            {
                GameMaster.getGUIManager().toggleAbilityWindow();
            }
            if(Input.GetKeyDown("space"))
            {
                GameMaster.getGUIManager().closeAllWindows();
            }
            if (Input.GetKeyDown("escape"))
            {
                GameMaster.getGUIManager().toggleIngameMenu();
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
            if (Input.GetKeyDown("d"))
            {
                GameMaster.getGUIManager().dropSelectedItem();
            }

            //item quick use
            for (int i = 0; i < GUIManager.QUICK_USE_ITEM_COUNT; i++)
            {
                if (Input.GetKeyDown((i+1).ToString()))
                {
                    if (Input.GetKey("left shift"))
                    {
                        GameMaster.getGUIManager().setQuickUseItem(i);
                    }
                    else
                    {
                        GameMaster.getGUIManager().quickUseItem(i);
                    }
                }
            }
        }

        if (!GameMaster.getGUIManager().isMouseOverGUI())
        {
            Hero hero = GameMaster.getPlayerHero();
            bool playerIsMelee = hero.isMelee();
            //Find all objects under cursor
            RaycastHit[] rayhits;
            rayhits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

            //Loop through all and find the first of each kind
            Transform firstUnit = null;
            Transform firstLoot = null;
            Transform firstAction = null;
            Transform firstResource = null;
            Vector3 groundPosition = Vector3.zero;
            bool hitGround = false;
            foreach (RaycastHit hit in rayhits)
            {
                string tag = hit.collider.tag;
                if (tag == "Unit")
                {
                    if (firstUnit == null)
                    {
                        firstUnit = hit.transform;
                    }
                }
                else if (tag == "Loot")
                {
                    if (firstLoot == null)
                    {
                        firstLoot = hit.transform;
                    }
                }
                else if (tag == "Action")
                {
                    if (firstAction == null)
                    {
                        firstAction = hit.transform;
                    }
                }
                else if (playerIsMelee && tag == "Resource")
                {
                    if (firstResource == null)
                    {
                        firstResource = hit.transform;
                    }
                }
                else if (tag == "Ground")
                {
                    groundPosition = hit.point;
                    hitGround = true;
                }
            }

            //Dehighlight the previous target
            setHighlightOnTarget(0);

            //Highlight the prefered one and set cursor
            if (firstUnit != null)
            {
                Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
                targetRenderers = firstUnit.GetComponentsInChildren<Renderer>();
                setHighlightOnTarget(1);
            }
            else if (firstLoot != null)
            {
                Cursor.SetCursor(lootCursor, Vector2.zero, CursorMode.Auto);
                targetRenderers = firstLoot.GetComponentsInChildren<Renderer>();
                setHighlightOnTarget(1);
            }
            else if (firstAction != null)
            {
                Cursor.SetCursor(lootCursor, Vector2.zero, CursorMode.Auto);
                targetRenderers = firstAction.GetComponentsInChildren<Renderer>();
                setHighlightOnTarget(1);
            }
            else if (firstResource != null)
            {
                Cursor.SetCursor(chopCursor, Vector2.zero, CursorMode.Auto);
                targetRenderers = firstResource.GetComponentsInChildren<Renderer>();
                setHighlightOnTarget(1);
            }
            else //hit ground
            {
                Cursor.SetCursor(moveCursor, Vector2.zero, CursorMode.Auto);
            }


            if (isPlacingBuilding)
            {
                buildingGhost.transform.position = groundPosition;

                if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
                {
                    placeBuilding(currentBuildingData, new Vector2i((int)groundPosition.x, (int)groundPosition.z));
                }
                else if (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON))
                {
                    cancelPlacingBuilding();
                }
            }
            else
            {
                if (GameMaster.getGUIManager().takeKeyboardInput())
                {

                    for (int i = 0; i < ABILITY_INPUT.Length; i++)
                    {
                        if (Input.GetKeyDown(ABILITY_INPUT[i]))
                        {
                            int abilityIndex = playerAbilityMapping[i];
                            if (!hero.hasAbility(abilityIndex)) continue;
                            if (hitGround && Input.GetKey(ATTACK_GROUND))
                            {
                                Ability ability = hero.getAbility(abilityIndex);
                                Vector2 heroPos = hero.get2DPos();
                                Vector2 groundPos2D = new Vector2(groundPosition.x, groundPosition.z);

                                Vector3 attackPos = groundPosition;
                                if (Vector2.Distance(heroPos, groundPos2D) > ability.data.range)
                                {
                                    Vector2 dir = (groundPos2D - heroPos).normalized;
                                    attackPos = new Vector3(heroPos.x + dir.x * ability.data.range, 0, heroPos.y + dir.y * ability.data.range);
                                }
                                requestAbilityCommandVec(unitID, attackPos + Vector3.up, abilityIndex);
                            }
                            else if (firstUnit != null)
                            {
                                int targetID = firstUnit.GetComponent<UnitController>().getID();
                                requestAbilityCommandID(unitID, targetID, abilityIndex);
                            }
                            else if (hitGround)
                            {
                                requestAbilityCommandVec(unitID, groundPosition + Vector3.up, abilityIndex);
                            }
                        }
                    }
                }


                if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
                {
                    if (hitGround && Input.GetKey(ATTACK_GROUND))
                    {
                        Ability ability = hero.getBasicAttack();
                        Vector2 heroPos = hero.get2DPos();
                        Vector2 groundPos2D = new Vector2(groundPosition.x, groundPosition.z);

                        attackPosition = groundPosition;
                        if (Vector2.Distance(heroPos, groundPos2D) > ability.data.range)
                        {
                            Vector2 dir = (groundPos2D - heroPos).normalized;
                            attackPosition = new Vector3(heroPos.x + dir.x * ability.data.range, 0, heroPos.y + dir.y * ability.data.range);
                        }
                        requestAttackCommandPos(unitID, attackPosition + Vector3.up);
                        targetPosition = attackPosition + Vector3.up;
                        attackingGround = true;
                    }
                    else
                    {
                        if (firstUnit != null)
                        {
                            targetSelected = false;
                            UnitController target = firstUnit.GetComponent<UnitController>();
                            targetUnitID = target.getID();
                            if (targetUnitID != unitID)
                            {
                                requestAttackCommandUnit(unitID, targetUnitID);
                            }
                            else
                            {
                                Debug.Log("Target is null");
                                targetUnitID = -1;
                            }
                        }
                        else if (firstLoot != null)
                        {
                            targetSelected = true;
                            targetTag = "Loot";
                            targetPosition = firstLoot.position;
                            int lootID = firstLoot.GetComponent<LootId>().getId();
                            requestLootCommand(unitID, Mathf.FloorToInt(targetPosition.x), Mathf.FloorToInt(targetPosition.z), lootID);
                        }
                        else if (firstAction != null)
                        {
                            targetSelected = true;
                            targetTag = "Action";
                            targetPosition = firstAction.position;
                            requestActionCommand(unitID, targetPosition.x, targetPosition.z);
                        }
                        else if (firstResource != null)
                        {
                            targetSelected = true;
                            targetTag = "Resource";
                            targetPosition = firstResource.position;
                            requestGatherCommand(unitID, targetPosition.x, targetPosition.z);
                        }
                        else
                        {
                            targetSelected = false;
                            requestMoveCommand(unitID, groundPosition.x, groundPosition.z);
                        }
                    }
                }
                else if (hitGround && (Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON)
                       || (Input.GetMouseButton(RIGHT_MOUSE_BUTTON) && clickTimer <= 0)
                       || (Input.GetMouseButton(LEFT_MOUSE_BUTTON) && !attackingGround && !targetSelected && targetUnitID < 0 && clickTimer <= 0)
                       || Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)))
                {
                    clickTimer = clickTime;
                    requestMoveCommand(unitID, groundPosition.x, groundPosition.z);
                }
                else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
                {
                    targetSelected = false;
                    targetUnitID = -1;
                    attackingGround = false;
                }
                else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON) && clickTimer <= 0)
                {
                    if (targetSelected)
                    {
                        if (targetTag == "Resource")
                        {
                            if (GameMaster.getPlayerHero().isMelee())
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
                    else if (targetUnitID >= 0)
                    {
                        requestAttackCommandUnit(unitID, targetUnitID);
                    }
                    else if (attackingGround && hitGround)
                    {
                        Ability ability = hero.getBasicAttack();
                        Vector2 heroPos = hero.get2DPos();
                        Vector2 groundPos2D = new Vector2(groundPosition.x, groundPosition.z);

                        attackPosition = groundPosition;
                        if (Vector2.Distance(heroPos, groundPos2D) > ability.data.range)
                        {
                            Vector2 dir = (groundPos2D - heroPos).normalized;
                            attackPosition = new Vector3(heroPos.x + dir.x * ability.data.range, 0, heroPos.y + dir.y * ability.data.range);
                        }
                        requestAttackCommandPos(unitID, attackPosition + Vector3.up);
                        targetPosition = attackPosition + Vector3.up;
                    }

                }
            }
        }
    }

    public abstract void requestRemoveActor(int unitID);

    [RPC]
    public void approveRemoveActor(int unitID)
    {
        Actor actor = GameMaster.getActor(unitID);
        actor.setAlive(false);
    }

    #region COMBAT

    [RPC]
	protected void killActor(int targetID, int unitID)
	{
		Actor target = GameMaster.getActor (targetID); 
		target.setAlive(false);
		requestUnitLootDrop(target.getName(), target.getTile());
        Hero hero = GameMaster.getHero(unitID);
        if(hero != null)
        {
            requestAddFavour(hero.getTeam(), target.getFavour());
        }
	}
	
	[RPC]
	protected void hitActor(int targetID, int unitID, int damage)
	{
		GameMaster.getActor (targetID).takeDamage(damage, unitID);
	}

    public abstract void requestAttack(int unitID, int targetID);

    //public abstract void requestFireProjectile(int unitID, Vector3 target);

    public abstract void requestFireProjectile(int unitID, Vector3 target, string dataName, string projectileName);

    [RPC]
    protected void approveFireProjectile(int unitID, Vector3 goal, string dataName, string projectileName)
    {
        GameMaster.addProjectile(unitID, goal, projectileName, dataName);
    }

    public abstract void requestProjectileHit(int damage, int unitID, int targetID);

    public abstract void requestHit(int damage, int unitID, int targetID, int skill = -1);

    [RPC]
    public abstract void requestLearnAbility(string ability, int unitID, int index);

    [RPC]
    protected void approveLearnAbility(string ability, int unitID, int index)
    {
        Hero hero = GameMaster.getHero(unitID);
        hero.addAbility(ability, index);
    }

    #endregion

    #region BUFFS

    public abstract void requestAddBuff(int unitID, string name, params object[] parameters);

    [RPC]
    protected void approveAddBuff(int unitID, string name, params object[] parameters)
    {
        Actor target = GameMaster.getActor(unitID);
        MethodInfo info = typeof(BuffGetter).GetMethod( name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        Buff buff = (Buff)info.Invoke(this, new object[] { parameters });
        buff.apply(target);
    }


    public abstract void requestApplyEffect(int unitID, int targetID, string effectName);

    [RPC]
    protected void approveApplyEffect(int unitID, int targetID, string effectName)
    {
        Actor actor = GameMaster.getActor(unitID);
        Actor target = GameMaster.getActor(targetID);
        AbilityEffect.applyBuffs(effectName, actor, target);
    } 


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

    
    public abstract void syncVitals();
    
    [RPC]
    public void syncHeroVitals(int unitID, int health, int energy)
    {
        Hero hero = GameMaster.getHero(unitID);
        hero.getUnitStats().getHealth().setCurValue(health);
        hero.getUnitStats().getEnergy().setCurValue(energy);
    }

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

    public abstract void requestChangeEnergy(int targetID, int energy);
    [RPC]
    protected void changeEnergy(int targetID, int energy)
    {
        Hero hero = GameMaster.getHero(targetID);
        if (hero != null)
        {
            hero.changeEnergy(energy);
        }
    }

    public abstract void requestChangeHealth(int targetID, int health);
    [RPC]
    protected void changeHealth(int targetID, int health)
    {
        Actor actor = GameMaster.getActor(targetID);
        if (actor != null)
        {
            actor.heal(health);
        }
    }
    #endregion

    #region INVENTORY AND CRAFTING
    //REGION INVENTORY AND CRAFTING

    public void startPlacingBuilding(BuildingRecipeData recipe)
    {
        buildingGhost.SetActive(true);
        isPlacingBuilding = true;
        currentBuildingData = recipe;
    }

    public void cancelPlacingBuilding()
    {
        buildingGhost.SetActive(false);
        isPlacingBuilding = false;
    }

    public void placeBuilding(BuildingRecipeData recipe, Vector2i position)
    {
        BuildingData data = DataHolder.Instance.getBuildingData(recipe.product);
        if (data != null && GameMaster.getPlayerHero().getInventory().removeRecipeIngredients(recipe))
        {
            GameMaster.addActor(data.getBuilding(position, 0, GameMaster.getNextUnitID()));
        }
        buildingGhost.SetActive(false);
        isPlacingBuilding = false;
    }

    [RPC]
	public abstract void requestItemCraft(int unitID, string name);

	[RPC]
	protected void approveItemCraft(int unitID, string name)
	{
		Inventory inv = GameMaster.getHero(unitID).getInventory();
        RecipeData data = DataHolder.Instance.getRecipeData(name);
		inv.craftItem(data);
        giveExperience(unitID, (int)data.skill, data.expAmount);
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

        item.applyBuffs(hero);

		hero.getInventory().consumeItem(itemIndex);
	}

    [RPC]
    public abstract void requestItemDrop(int unitID, int itemIndex);

    [RPC]
    protected void approveItemDrop(int unitID, int itemIndex, string itemName, Vector3 startPos)
    {
        Hero hero = GameMaster.getHero(unitID);
        hero.getInventory().removeItem(itemIndex);


        if(hero.getTile() != new Vector2i(startPos)) 
		{
			hero.setPosition(startPos); //If desynced: SYNC THAT
		}

        ItemData data = DataHolder.Instance.getItemData(itemName);
        World.tileMap.getTile(hero.getTile()).addLoot(data.getLootableObject(new Vector2(startPos.x, startPos.z), UnityEngine.Random.value * 360));
        GameMaster.getWorld().activateTileIfInActivationRangeOfControlledUnit(hero.getTile().x, hero.getTile().y);
    }

    #endregion

    #region CHAT AND CHEAT 
    //REGION CHAT AND CHEAT

    public abstract void requestWarp(int unitID, Vector2i tile);

    [RPC]
    protected void warpUnit(int unitID, float x, float y, float rot = 0)
    {
        Unit unit = GameMaster.getActor(unitID) as Unit;
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
                GameMaster.addActor(new AIUnit(unitData.name, commander.getPosition() + Vector3.forward, Vector3.zero, GameMaster.getNextUnitID()));
            }
            break;

            case (CheatCommand.BUILD):
            {
                BuildingData data = DataHolder.Instance.getBuildingData((string)parameters[0]);
                if (data != null)
                {
                    GameMaster.addActor(data.getBuilding(new Vector2i(commander.getTile().x + 1, commander.getTile().y), 0, GameMaster.getNextUnitID()));
                }
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
    public void recieveChatMessage(int playerID, string msg)
    {
        OnlinePlayer player = NetworkMaster.findPlayer(playerID);
        if(player == null)
            GameMaster.getGUIManager().addChatMessage("<color=#" + GameMode.teamColorsHex[0] + ">" + "player" + ": " + msg + "</color>");
        else
            GameMaster.getGUIManager().addChatMessage("<color=#" + GameMode.teamColorsHex[player.getTeam()] + ">" + player.getName() + ": " + msg + "</color>");
    }

    #endregion

    protected abstract IEnumerator lagMove(int unitID, float x, float y, float lag);


}
