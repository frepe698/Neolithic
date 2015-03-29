using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

    public static readonly string[] heroNames = new string[]
    {
        "vrodl",
        "halftroll"
    };
	
	public static Dictionary<int, int> playerToUnitID = new Dictionary<int, int>();
	static List<Hero> heroes = new List<Hero>();
	static List<Unit> units = new List<Unit>();
	static List<Unit> awakeUnits = new List<Unit>();
    static List<UnitSpawner> daySpawners = new List<UnitSpawner>();
    static List<UnitSpawner> nightSpawners = new List<UnitSpawner>();

    private static int aiSpawnLevel = 0;

	static List<Projectile> projectiles = new List<Projectile>();

	static private readonly int AWAKE_RADIUS = 20;
	static private int playerID;
	static private int playerUnitID;
	static private Hero hero;
	static private float updateTime;
	
	private NetworkView netView;
	private static GameController gameController;
	private GUIManager guiManager;
	
	private static World world;
	
	
	private float zoom = 0;
	private float maxzoom = 1;
	private float minzoom = -7;
	private Camera playerCamera;

    private static GameMode mode;

    private bool gameStarted = false;
	
	void Start () 
	{
		DataHolder.loadData();
		guiManager = gameObject.AddComponent<GUIManager>();
		world = GameObject.Find("World").GetComponent<World>();
		netView = GetComponent<NetworkView>();
        mode = new TrialOfTheGods(this);
        mode.initWorld();
        //world.initPvPWorld();
		
		if(!NetworkMaster.isConnected())
		{
			NetworkMaster.initializeOfflineServer();
			NetworkMaster.connect();
            OnlinePlayer player = new OnlinePlayer(Network.player, "player", 0);
            player.setHero(0);
            NetworkMaster.addPlayer(player);
		}

        mode.spawnHeroes();

		playerID = NetworkMaster.getMyPlayerID();
		
		try
		{
			playerUnitID = playerToUnitID[playerID];
            Debug.Log(playerUnitID);
		}
		catch(KeyNotFoundException)
		{
			playerUnitID = 2; //TODO fixa om man är offline med dictionary
            Debug.Log("key not find");
		}
		if(Network.isServer)
		{
			gameController = gameObject.AddComponent<ServerController>();
			gameController.init(this, playerUnitID);
		}
		else if(Network.isClient)
		{
			gameController = gameObject.AddComponent<ClientController>();
			gameController.init(this, playerUnitID);
		}
		else 
		{
			gameController = gameObject.AddComponent<OfflineController>();
			gameController.init(this, playerUnitID);
		}
		
        setPlayerUnit(playerUnitID);

        mode.spawnUnits();
		
		playerCamera = Camera.main;
		zoom = minzoom;

        guiManager.setPlayerHero(hero);

        gameController.setPlayerLoaded(NetworkMaster.getMyPlayerID());
	}

	private void setPlayerUnit(int id)
	{
		hero = getHero(id);
		world.setUnit(hero);
		hero.activateCollider(false);
	}

    public void startGame()
    {
        gameStarted = true;
        guiManager.startGame();
    }
	
	void Update () 
	{
        if (!gameStarted)
        {
            if (Time.timeSinceLevelLoad > 30) gameController.requestGameStart();
            return;
        }
        TimeManager.Instance.update();
		checkIfUnitsAwake();
		gameController.update();
        mode.update();
        
		for(int i = 0; i < units.Count; i++)
		{
			Unit unit = units[i];
			if(!unit.isAlive())
			{
                removeUnit(unit);
                i--;
                
			}
			else
			{
				unit.update();
			}
		}
        for (int i = 0; i < heroes.Count; i++ )
        {
            Hero hero = heroes[i];

            if (!hero.isAlive())
            {
                if(hero.isWaitingRespawn())
                {
                    if (hero.updateRespawnTimer())
                    {
                        gameController.requestRespawnHero(hero.getID());
                    }
                }
                else 
                {
                    gameController.requestHeroStartRespawn(hero.getID());
                }
                

            }

        }

        for (int i = 0; i < projectiles.Count; i++)
        {
            Projectile proj = projectiles[i];
            if (proj.isRemoved())
            {
                proj.Inactivate();
                projectiles.RemoveAt(i);
                i--;
            }
            else
            {
                proj.update();
            }
        }

		foreach(Hero hero in heroes)
		{
			hero.updateStats();
		}

		guiManager.update ();
	}

	public void checkIfUnitsAwake()
	{
		if(Time.time >= updateTime)
		{ 
			updateAwakeUnits();
			updateTime += 1;
		}
	}

    public static void requestLaneSpawningStart()
    {
        gameController.requestLaneSpawning();
    }
    public static void startSpawning()
    {
        mode.initSpawning();
    }

    
    public static void increaseAILevel()
    {
        aiSpawnLevel++;
    }
    public static int getAISpawnLevel()
    {
        return aiSpawnLevel;
    }

    public static void requestDamageBase(int team, int damage, int unitID)
    {
        gameController.requestDamageBase(team, damage, unitID);
    }

    public static void damageBase(int team, int damage)
    {
        mode.damageBase(team, damage);
    }

    public static void addFavour(int team, int favour)
    {
        mode.grantFavour(team, favour);
    }

	public void updateAI()
	{
        
		foreach(Unit unit in awakeUnits)
		{
			unit.updateAI();
		}
	}

	private void updateAwakeUnits()
	{
        
		for(int i = 0; i < awakeUnits.Count; i++)
		{
			Unit unit = awakeUnits[i];
			bool awake = false;
			foreach(Unit hero in heroes)
			{
				if(Vector2i.getLongestAxis(unit.getTile(), hero.getTile()) < AWAKE_RADIUS)
				{
					awake = true;
				}
			}
			unit.setAwake(awake);
			if(unit.isAwake() == false)
			{
				awakeUnits.Remove (unit);
				unit.inactivate();
				i--;
			}

		}
		foreach(Hero hero in heroes)
		{
			Vector2i tile = hero.getTile();

			for(int x = tile.x - AWAKE_RADIUS; x < tile.x + AWAKE_RADIUS; x++)
			{
				for(int y = tile.y - AWAKE_RADIUS; y < tile.y + AWAKE_RADIUS; y++)
				{
					if(!World.getMap().isValidTile(x,y)) continue;

					Tile curTile = World.getMap().getTile(x,y);
					foreach(Unit unit in curTile.getUnits ())
					{
						unit.setAwake(true);
						if(!awakeUnits.Contains(unit)) awakeUnits.Add(unit);
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		updateCamera();
	}
	
    public static void respawnAllSpawners()
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
           gameController.requestSpawnerRespawn(spawner.getID());
        }
        foreach (UnitSpawner spawner in nightSpawners)
        {
            gameController.requestSpawnerRespawn(spawner.getID());
        }
    }

    public static void respawnAllNightSpawners()
    {
        foreach (UnitSpawner spawner in nightSpawners)
        {
            gameController.requestNightSpawnerRespawn(spawner.getID());
        }
    }

    public static void respawnAllDaySpawners()
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
            gameController.requestDaySpawnerRespawn(spawner.getID());
        }
    }

    public static void allSpawnersRemoveUnits()
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
            gameController.requestSpawnerRemoveAll(spawner.getID());
        }
        foreach (UnitSpawner spawner in nightSpawners)
        {
            gameController.requestSpawnerRemoveAll(spawner.getID());
        }
    }

    public static void allDaySpawnersRemoveUnits()
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
            gameController.requestDaySpawnerRemoveAll(spawner.getID());
        }
    }

    public static void allNightSpawnersRemoveUnits()
    {
        foreach (UnitSpawner spawner in nightSpawners)
        {
            gameController.requestNightSpawnerRemoveAll(spawner.getID());
        }
    }
    public static void spawnerRespawn(int spawnerID)
    {
        foreach(UnitSpawner spawner in daySpawners)
        {
            if(spawner.getID() == spawnerID)
            {
                spawner.respawnUnits();
                //spawner.respawnIfInactive();
            }
        }
        foreach (UnitSpawner spawner in nightSpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.respawnIfInactive();
                spawner.respawnUnits();
            }
        }
    }

    public static void daySpawnerRespawn(int spawnerID)
    {
        foreach(UnitSpawner spawner in daySpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.respawnIfInactive();
                spawner.respawnUnits();
            }
        }
    }

    public static void nightSpawnerRespawn(int spawnerID)
    {
        foreach (UnitSpawner spawner in nightSpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.respawnIfInactive();
                spawner.respawnUnits();
            }
        }
    }

    public static void spawnerRemoveAll(int spawnerID)
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                spawner.removeUnits();
                //spawner.removeInactiveUnits();
            }
        }
        foreach (UnitSpawner spawner in nightSpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.removeInactiveUnits();
                spawner.removeUnits();
            }
        }
    }
    public static void daySpawnerRemoveAll(int spawnerID)
    {
        foreach (UnitSpawner spawner in daySpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.removeInactiveUnits();
                spawner.removeUnits();
            }
        }
    }

    public static void nightSpawnerRemoveAll(int spawnerID)
    {
        foreach (UnitSpawner spawner in nightSpawners)
        {
            if (spawner.getID() == spawnerID)
            {
                //spawner.removeInactiveUnits();
                spawner.removeUnits();
            }
        }
    }

    public static void addDaySpawner(string name, int maxUnits, Vector2i pos)
    {
        UnitSpawner spawner = new UnitSpawner(name, maxUnits, pos, getNextDaySpawnerID());
        daySpawners.Add(spawner);
    }
  
    public static void addDaySpawner(string name, int maxUnits, Vector2i pos, List<WorldPoint> points)
    {
        UnitSpawner spawner = new PathUnitSpawner(name, maxUnits, pos, getNextDaySpawnerID(), points);
        daySpawners.Add(spawner);
    }

    public static void addNightSpawner(string name, int maxUnits, Vector2i pos)
    {
        UnitSpawner spawner = new UnitSpawner(name, maxUnits, pos, getNextNightSpawnerID());
        nightSpawners.Add(spawner);
    }
  
    public static void addNightSpawner(string name, int maxUnits, Vector2i pos, List<WorldPoint> points)
    {
        UnitSpawner spawner = new PathUnitSpawner(name, maxUnits, pos, getNextNightSpawnerID(), points);
        nightSpawners.Add(spawner);
    }
  
    public static int getNextDaySpawnerID()
    {
        if (daySpawners == null || daySpawners.Count == 0) return 0;
        return daySpawners[daySpawners.Count - 1].getID() + 1;
    }
    public static int getNextNightSpawnerID()
    {
        if (nightSpawners == null || nightSpawners.Count == 0) return 0;
        return nightSpawners[nightSpawners.Count - 1].getID() + 1;
    }
    

	private void spawnHeroesNew()
	{

	}
	
	public static Hero getHero(int id)
	{
		foreach(Hero u in heroes)
		{
			if(u.getID () == id)
			{
				return u;
			}
		}
		return null;
	}

	public static Hero getPlayerHero()
	{
		return hero;
	}

	public static Unit getUnit(int id)
	{
		foreach(Unit u in units)
		{
			if(u.getID () == id)
			{
				return u;
			}
		}
		return null;
	}

	public List<Unit> getUnits()
	{
		return units;
	}

	public static void addUnit(Unit unit)
	{
		units.Add(unit);
	}

    public static void addAwakeUnit(Unit unit)
    {
        units.Add(unit);
        unit.setAwake(true);
        awakeUnits.Add(unit);
    }

    public static void addHero(Hero hero)
    {
        heroes.Add(hero);
        units.Add(hero);
        hero.activate();
    }

	public static void removeUnit(Unit unit)
	{
        if (unit == null) return;
        //Fix: if a unit is "afk" and you shoot it with an arrow, this function checks a tile outside of the map
        if (!World.getMap().getTile(unit.getTile()).removeUnit(unit)) Debug.LogError("Did not remove unit from tile");
        unit.setAlive(false);
		unit.setAwake(false);
		unit.inactivate();

        Hero hero = unit as Hero;
        if (hero != null && !hero.isWaitingRespawn())
        {
            gameController.requestHeroStartRespawn(hero.getID());
        }
        bool removedFromUnits = units.Remove(unit);
		bool removedFromAwake = awakeUnits.Remove(unit);
        if (removedFromAwake && !removedFromUnits) Debug.LogError("only removed unit from awake list");

        /*Hero hero = unit as Hero;
		if(hero != null) heroes.Remove(hero);*/
	}

    public static void startHeroRespawn(int unitID)
    {
        foreach(Hero hero in heroes)
        {
            if(hero.getID() == unitID)
            {
                hero.startRespawn();
            }
        }
    }

    public static void respawnHero(int unitID)
    {
        foreach(Hero hero in heroes)
        {
            if(hero.getID() == unitID)
            {
                hero.setAlive(true);
                hero.setAwake(true);
                hero.activate();
                hero.respawn();
                units.Insert(hero.getID(),hero);
                awakeUnits.Add(hero);
            }
        }
        

    }

	public static int getNextUnitID()
	{
		if(units == null || units.Count == 0) return 0;
		return units[units.Count-1].getID()+1;
	}
	
	public NetworkView getNetView()
	{
		return netView;
	}
	
	public void updateCamera()
	{
		if(!guiManager.isMouseOverGUI())
		{
			zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel"), minzoom, maxzoom);
		}
		playerCamera.transform.position = hero.getPosition() + new Vector3(0.55f-zoom*0.35f, 2-zoom*1f, -0.55f+zoom*0.35f);

	}
	
	public static GameController getGameController()
	{
		return gameController;
	}

	public GUIManager getGUIManager()
	{
		return guiManager;
	}
	
	public static void addResourceLoot(string resourceName, int seed, int x, int y, int unitID)
	{
		Random.seed = seed;
		ResourceData data = DataHolder.Instance.getResourceData(resourceName);
		if(data == null) return;

        UnitStats unitstats = getHero(unitID).getUnitStats();
        if (hero == null) return;

		for(int i = 0; i < data.safeDrops.Length; i++)
		{
			string itemName = data.safeDrops[i];
			ItemData itemData = DataHolder.Instance.getItemData(itemName);
			if(itemData != null)
			{
                World.getMap().getTile(x, y).addLoot(itemData.getLootableObject(new Vector2(x + Random.Range(0f, 1.0f), y + Random.Range(0f, 1.0f)),
                                                                                Random.Range(0, 360)));
			}
			
		}
		float rarity = 0;
        float amount = 0;

        switch (data.damageType)
        {
            case(DamageType.TREE):
                rarity = unitstats.getStatV(Stat.TreeDropRarity);
                amount = unitstats.getStatV(Stat.TreeDropAmount);
                break;
            case(DamageType.STONE):
                rarity = unitstats.getStatV(Stat.StoneDropRarity);
                amount = unitstats.getStatV(Stat.StoneDropAmount);
                break;
        }
		
		if(data.randomDrops != null)
		{
            bool hasRare = data.rareDrops != null;
			int randomDrops = Mathf.RoundToInt(Random.Range(data.minDrops, data.maxDrops+1) * amount);
			for(int i = 0; i < randomDrops; i++)
			{  
				string itemName = (!hasRare || Random.value > rarity) ? data.randomDrops[Random.Range(0, data.randomDrops.Length)] : data.rareDrops[Random.Range(0, data.rareDrops.Length)];
				ItemData itemData = DataHolder.Instance.getItemData(itemName);
				if(itemData != null)
				{
                    World.getMap().getTile(x, y).addLoot(itemData.getLootableObject(new Vector2(x + Random.Range(0f, 1.0f), y + Random.Range(0f, 1.0f)),
					                                                                Random.Range(0, 360)));
				}
			}
		}
		
		world.activateTileIfInActivationRangeOfControlledUnit(x, y);
		
	}
	
	public static void addUnitLoot(string unitName, int seed, int x, int y)
	{
		Random.seed = seed;
		UnitData data = DataHolder.Instance.getUnitData(unitName);
		if(data == null) return;
        string[] safedrops = data.getSafeDrops();
        if (safedrops != null)
        {
            for (int i = 0; i < safedrops.Length; i++)
            {
                string itemName = safedrops[i];
                ItemData itemData = DataHolder.Instance.getItemData(itemName);
                if (itemData != null)
                {
                    World.getMap().getTile(x, y).addLoot(itemData.getLootableObject(new Vector2(x + Random.Range(0f, 1.0f), y + Random.Range(0f, 1.0f)),
                                                                                    Random.Range(0, 360)));
                }

            }
        }
		
		
		if(data.getRandomDrops() != null)
		{
			int randomDrops = Random.Range(data.getMinDrops(), data.getMaxDrops()+1);
			for(int i = 0; i < randomDrops; i++)
			{  
				string itemName = data.getRandomDrops()[Random.Range(0, data.getRandomDrops().Length)];
				ItemData itemData = DataHolder.Instance.getItemData(itemName);
				if(itemData != null)
				{
                    World.getMap().getTile(x, y).addLoot(itemData.getLootableObject(new Vector2(x + Random.Range(0f, 1.0f), y + Random.Range(0f, 1.0f)),
                                                                                    Random.Range(0, 360)));
				}
			}
		}
		
		world.activateTileIfInActivationRangeOfControlledUnit(x, y);
		
	}

	public static void addProjectile(int unitID, Vector3 goal, string projectileName, string dataName)
	{
		Vector3 start = getUnit(unitID).getPosition() + Vector3.up;
		ProjectileData data = DataHolder.Instance.getProjectileData(projectileName);
        if (data == null) return;
		float speed = data.speed;
		float range = data.range;
		Projectile projectile = new Projectile(start, goal, range, speed, data.modelName, dataName, unitID);
		projectile.Activate();
		projectiles.Add(projectile);
	}

	public static World getWorld()
	{
		return world;
	}

	public static int getPlayerUnitID()
	{
		return playerUnitID;
	}

    public static List<Hero> getHeroes()
    {
        return heroes;
    }

    void OnDestroy()
    {
        ObjectPoolingManager.removeInstance();
        RenderDataPool.removeInstance();
        TimeManager.removeInstance();

        heroes.Clear();
	    units.Clear();
	    awakeUnits.Clear();

	    projectiles.Clear();
        hero = null;
        aiSpawnLevel = 0;
        playerToUnitID = new Dictionary<int, int>();
    }
	
}
