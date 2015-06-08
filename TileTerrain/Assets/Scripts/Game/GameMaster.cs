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
	static List<Actor> actors = new List<Actor>();
	static List<Actor> awakeActors = new List<Actor>();
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
	private static GUIManager guiManager;
	
	private static World world;
	
	
	private float zoom = 0;
	private float maxzoom = 1;
	private float minzoom = -7;
	private Camera playerCamera;

    private static GameMode mode;

    private static float gameStartTime;
    private static bool gameStarted = false;
	
	void Start () 
	{
		DataHolder.loadData();
		guiManager = gameObject.AddComponent<GUIManager>();
        guiManager.setGameMaster(this);
		world = GameObject.Find("World").GetComponent<World>();
		netView = GetComponent<NetworkView>();
        mode = new MonumentMode(this);
        mode.initWorld();
        //world.initPvPWorld();
		
		if(!NetworkMaster.isConnected())
		{
			NetworkMaster.initializeOfflineServer();
			NetworkMaster.connect();
            OnlinePlayer player = new OnlinePlayer(Network.player, "player", 0, 0);
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
        GameMaster.getGUIManager().addMinimapHeroes();

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
        gameStartTime = Time.timeSinceLevelLoad;
        guiManager.startGame();
    }

    public static float getGameTime()
    {
        return Time.timeSinceLevelLoad - gameStartTime;
    }
	
	void Update () 
	{
        if (!gameStarted)
        {
            if (Time.timeSinceLevelLoad > 30) gameController.requestGameStart();
            return;
        }
        TimeManager.Instance.update();
		timedUpdates();
		gameController.update();
        mode.update();
        
		for(int i = 0; i < actors.Count; i++)
		{
			Actor actor = actors[i];
			if(!actor.isAlive())
			{
                removeActor(actor);
                i--;
                
			}
			else
			{
				actor.update();
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

	public void timedUpdates()
	{
		if(Time.time >= updateTime)
		{ 
			updateAwakeUnits();
            syncStats();
			updateTime += 1;
		}
	}

    public void syncStats()
    {
        gameController.syncVitals();
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
        aiSpawnLevel+=2;
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
        
		foreach(Actor actor in awakeActors)
		{
			actor.updateAI();
		}
	}

	private void updateAwakeUnits()
	{
        
		for(int i = 0; i < awakeActors.Count; i++)
		{
			Actor actor = awakeActors[i];
			bool awake = false;
			foreach(Unit hero in heroes)
			{
                if (Vector2i.getLongestAxis(actor.getTile(), hero.getTile()) < AWAKE_RADIUS)
				{
					awake = true;
				}
			}
            actor.setAwake(awake);
            if (actor.isAwake() == false)
			{
                awakeActors.Remove(actor);
                actor.inactivate();
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
					foreach(Actor actor in curTile.getActors ())
					{
						actor.setAwake(true);
						if(!awakeActors.Contains(actor)) awakeActors.Add(actor);
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		updateCamera();

        foreach (Actor actor in awakeActors)
        {
            actor.updateHealthbar();
        }
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

	public static Actor getActor(int id)
	{
		foreach(Actor a in actors)
		{
			if(a.getID () == id)
			{
				return a;
			}
		}
		return null;
	}

	public List<Actor> getActors()
	{
		return actors;
	}

	public static void addActor(Actor actor)
	{
		actors.Add(actor);
	}

    public static void addAwakeActor(Actor actor)
    {
        actors.Add(actor);
        actor.setAwake(true);
        awakeActors.Add(actor);
    }

    public static void addHero(Hero hero)
    {
        heroes.Add(hero);
        actors.Add(hero);
        hero.activate();
    }

	public static void removeActor(Actor actor)
	{
        if (actor == null) return;
        //Fix: if a unit is "afk" and you shoot it with an arrow, this function checks a tile outside of the map
        if (!World.getMap().getTile(actor.getTile()).removeActor(actor)) Debug.LogError("Did not remove unit from tile");
        actor.setAlive(false);
		actor.setAwake(false);
		actor.inactivate();

        Hero hero = actor as Hero;
        if (hero != null && !hero.isWaitingRespawn())
        {
            gameController.requestHeroStartRespawn(hero.getID());
        }
        bool removedFromUnits = actors.Remove(actor);
		bool removedFromAwake = awakeActors.Remove(actor);
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
                actors.Insert(hero.getID(),hero);
                awakeActors.Add(hero);
            }
        }
        

    }

	public static int getNextUnitID()
	{
		if(actors == null || actors.Count == 0) return 0;
		return actors[actors.Count-1].getID()+1;
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
		playerCamera.transform.position = hero.getPosition() + new Vector3(-3, 2-zoom*1f, -3);
        playerCamera.transform.LookAt(hero.getPosition(), Vector3.up);

	}
	
	public static GameController getGameController()
	{
		return gameController;
	}

	public static GUIManager getGUIManager()
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
		Vector3 start = getActor(unitID).getPosition() + Vector3.up;
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
	    actors.Clear();
	    awakeActors.Clear();

	    projectiles.Clear();
        hero = null;
        aiSpawnLevel = 0;
        playerToUnitID = new Dictionary<int, int>();
    }
	
}
