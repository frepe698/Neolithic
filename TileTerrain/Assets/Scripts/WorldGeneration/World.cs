using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	
	private Material waterMaterial;
	private Material groundMaterial;
	private float waterOffset;
	private Transform water;
	private MeshRenderer waterRenderer;
	private MeshRenderer[] groundRenderers;
	private bool drawingGrid = false;
	private float snowAmount = 0;
	private float timer;
	
	private WorldSection[,] sections;
	
	public static readonly int GRASS = 0;
	public static readonly int ROAD = 1;
	
	public static TileMap tileMap;
	private static List<Tile> activeTiles = new List<Tile>();
	
	public static int summonDistanceFromBase = 4;
	public static int baseBounds = 0;
	public static int summonBounds = 1;
	public static int summonSize = 3;
	public static int baseSize = 5;
	public static int mapSize = 129;
	
	
	public bool cliffs = true;
	public bool testTiles = true;
	public readonly static int tilesize = 1;
	public readonly static int WATER_LEVEL = 0;
	public readonly static int MOUNTAIN_LEVEL = 6;
	public readonly static int SNOW_LEVEL = 9;
	
	public static int maxHeight = 10;
	public static int minHeight = -2;
	
	Vector2i lastPlayerPos = new Vector2i(-1, -1);
	private readonly Vector2i cameraOffset = new Vector2i(-3,3);
	private readonly static int resourceTileActivationRange = 30;
	private readonly static int resourceTileInactivationRange = 32;
	private readonly static int eyecandyTileActivationRange = 16;
	private readonly static int eyecandyTileInactivationRange = 18;
	
	public static int seed = -1;
	
	private Unit controlledUnit;

    public static List<Vector3> normalsstart = new List<Vector3>();
    public static List<Vector3> normalsend = new List<Vector3>();
	
	void Awake () {
		waterMaterial = (Material)Resources.Load ("water");
		groundMaterial = (Material)Resources.Load ("terrain");

	}


    void OnDestroy()
    {
        tileMap = null;
        activeTiles.Clear();
    }

    /*
	public void initWorld()
	{
		if(seed != -1)
		{
			Random.seed = seed;
		}
		generateHeights();
		tileMap.generateGround();
        tileMap.generateCaves();
		generateWorldSections();
		initObjectPools();
		generateWater();
		addObjects();
	}*/

    public void initPvPWorld(TrialOfTheGods mode)
    {
        if (seed != -1)
        {
            Random.seed = seed;
        }
        tileMap = new TileMap(4, 1);
        tileMap.generatePvPMap(mode);
        tileMap.generateGround();
        tileMap.generateCaves();
        generateWorldSections();
        initObjectPools();
        generateWater();
        addObjects();
        //addRoadSpawners(mode);
    }
	
	
	void initObjectPools()
	{
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/gran01"), 80,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/gran02"), 80,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/gran03"), 80,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/tree01"), 50,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/bigTree01"), 100,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/deadTree01"), 30,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/deadTree02"), 30,true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/deadTree03"), 30, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/deadTree04"), 30, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/greenTree01"), 50,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/greenTree02"), 50,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/greenTree03"), 50,true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Trees/birch01"), 50,true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Bushes/deadbush01"), 100, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Bushes/greenbush01"), 100, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Stones/stone01"), 50, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Stones/stone02"), 50, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Stones/stone03"), 50, true);

        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("EyecandyObjects/walltorch"), 4, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("EyecandyObjects/thehall_pillar"), 4, true);

        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("ActionObjects/caveEntrance"), 2, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("ActionObjects/caveExit"), 2, true);


        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/log"), 50, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/finewood"), 20, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/stick"), 200, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/stone"), 200, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/flint"), 50, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/gold"), 50, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/iron"), 50, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/deathcap"), 50, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/cep"), 50, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Loot/puffball"), 50, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/meat"), 20, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/harepelt"), 10, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Loot/fur"), 10, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Harvestable/blueberrybush"), 50, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Harvestable/nettle"), 50, true);
		
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/grass01"), 400, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/fern01"), 150, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/fern02"), 150, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/fernCluster02"), 100, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/cowparsley"), 100, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/eng01"), 200, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/eng02"), 200, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/vass01"), 200, true);
//		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Eyecandy/waterlily01"), 200, true);

        RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/grass01"));
        RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/fallengrass01"));
        RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/moss01"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/fern01"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/fern02"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/fernCluster02"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/cowparsley"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/eng01"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/eng02"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/vass01"));
		RenderDataPool.Instance.CreateEyecandyData((GameObject)Resources.Load ("Eyecandy/waterlily01"));
		
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Tools/flintaxe"), 10, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Tools/flintpickaxe"), 10, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Tools/shortbow"), 5, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Tools/quillrain"), 5, true);

        /* OLD PARTICLES
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Particles/treeChopParticles"), 4, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Particles/bushChopParticles"), 4, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/stoneMineParticles"), 4, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/bloodParticles"), 4, true);
        */

		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Units/Animals/hare"), 20, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Units/Animals/battlepig"), 20, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Units/Animals/wolf"), 20, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Units/Heroes/caveman"), 1, true);
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Units/Heroes/vrodl"), 1, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Units/Heroes/halftroll"), 1, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Units/Monsters/troll"), 4, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Units/Monsters/goblin"), 4, true);
        
		ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load ("Projectiles/rock"),20, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Projectiles/arrow"), 20, true);
        ObjectPoolingManager.Instance.CreatePool((GameObject)Resources.Load("Projectiles/fireball"), 10, true);


        //PARTICLES

        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/treeChopParticles"), 4, true);
        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/bushChopParticles"), 4, true);
        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/stoneMineParticles"), 4, true);
        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/bloodParticles"), 4, true);
        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/levelupParticles"), 2, true);
        ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/leap_land"), 2, true);
        //ParticlePoolingManager.Instance.CreatePool((GameObject)Resources.Load("Particles/bolt"), 2, true);
	}
	
	void generateWorldSections()
	{
		sections = new WorldSection[tileMap.sectionCount, tileMap.sectionCount];
		int[,] tileMapColor = new int[tileMap.getSize(),tileMap.getSize()];
		for(int x = 0; x < tileMap.getSize(); x++)
		{
			for(int y = 0; y < tileMap.getSize(); y++)
			{
				tileMapColor[x, y] = tileMap.getTile(x, y).getColor();
			}
		}
		//Texture2D[] textures = WorldSection.getWorldSplatTexture(tileMapColor, (tileMap.sectionCount*WorldSection.SIZE*2) + 1);
		Texture2D[] textures = WorldSection.getWorldSplatTexture(tileMapColor, tileMap.sectionCount*WorldSection.SIZE);
		Material material = (Material)Resources.Load("terrain");
		
		
		int xtri = WorldSection.SIZE*2;
		int ytri = WorldSection.SIZE;

		Vector3[,] terrainNormals = new Vector3[xtri*tileMap.sectionCount,ytri*tileMap.sectionCount];
		
		for(int y = 0; y < tileMap.sectionCount; y++)
		{
			for(int x = 0; x < tileMap.sectionCount; x++)
			{
				WorldSection ws = new WorldSection(new Vector2i(x, y));
                for (int ty = y * ytri; ty < (y + 1) * ytri; ty++)
                {
                    for (int tx = x * xtri; tx < (x + 1) * xtri; tx++)
                    {
                        int stx = tx - x * xtri;
                        int sty = ty - y * ytri;
                        Vector3 normal = ws.getTriangleNormal(stx + sty * xtri);
                        terrainNormals[tx, ty] = normal;

                        if (normal.y < TileMap.cliffThreshold)
                        {
                            tileMap.getTile(tx / 2, ty).setCliff(true);
                        }

                    }
                }
				
				sections[x, y] = ws;
				
			}
		}

       // Vector3[] normals = calculateNormals(terrainNormals);
		
		groundRenderers = new MeshRenderer[tileMap.sectionCount*tileMap.sectionCount];
		for(int y = 0; y < tileMap.sectionCount; y++)
		{
			for(int x = 0; x < tileMap.sectionCount; x++)
			{
                sections[x, y].calculateNormals(terrainNormals);
                //sections[x, y].setNormals(normals);
				
				GameObject section = new GameObject("World Section("+x+","+y+")");
				section.tag = "Ground";
				MeshFilter filter = section.AddComponent<MeshFilter>();
				filter.mesh = sections[x, y].getMesh ();
				
				MeshRenderer renderer = section.AddComponent<MeshRenderer>();
				
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				renderer.material = material;
				
				groundRenderers[x + y * tileMap.sectionCount] = renderer;
				
				MeshCollider col = section.AddComponent<MeshCollider>();
				col.sharedMesh = filter.mesh;
				
				section.transform.SetParent(transform);
				section.transform.localPosition = new Vector3(x*WorldSection.SIZE, 0, y*WorldSection.SIZE);
				section.layer = 8;
				
				
			}
		}
		
		foreach(MeshRenderer groundRenderer in groundRenderers)
		{
			for(int i = 0; i < SplatMapColor.colorCount; i++)
			{
				groundRenderer.material.SetTexture("_Splat"+i, textures[i]);
			}
			
			int scale = (tileMap.sectionCount*WorldSection.SIZE) / 16;
			groundRenderer.material.SetTextureScale("_ColorTexture", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture2", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture3", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture4", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture5", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture6", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture7", new Vector2(scale, scale));
			//		groundRenderer.sharedMaterial.SetTextureScale("_Texture8", new Vector2(scale, scale));
			groundRenderer.material.SetTextureScale("_GridTexture", new Vector2(tileMap.sectionCount*WorldSection.SIZE, tileMap.sectionCount*WorldSection.SIZE));
		}
		//initTerrainTexture();
	}

    Vector3[] calculateNormals(Vector3[,] triNormals)
    {
        int vertCount = tileMap.sectionCount * WorldSection.SIZE + 1;
		Vector3[] normals = new Vector3[vertCount * vertCount];
        for (int x = 0; x < vertCount; x++)
        {
            for (int y = 0; y < vertCount; y++)
            {
                int mapX = x * 2;
                int mapY = y;
                Vector3 normal = Vector3.zero;
                bool up = mapY > 0;
                bool right = mapX < vertCount * 2 - 2;
                bool down = mapY < vertCount - 2;
                bool left = mapX > 0;
                if (up)
                {

                    if (right)
                    {
                        normal += triNormals[mapX, mapY - 1];
                        normal += triNormals[mapX + 1, mapY - 1];
                    }
                    else
                    {
                        //Debug.Log(x);
                        normal += new Vector3(0, 2, 0);
                    }
                    if (left)
                    {
                        normal += triNormals[mapX - 1, mapY - 1];
                    }
                    else
                    {
                        normal += new Vector3(0, 1, 0);
                    }

                }
                else
                {
                    normal += new Vector3(0, 3, 0);
                }


                if (down)
                {
                    if (left)
                    {
                        normal += triNormals[mapX - 1, mapY];
                        normal += triNormals[mapX - 2, mapY];
                    }
                    else
                    {
                        normal += new Vector3(0, 2, 0);
                    }
                    if (right)
                    {
                        normal += triNormals[mapX, mapY];
                    }
                    else
                    {
                        normal += new Vector3(0, 1, 0);
                    }
                }
                else
                {
                    normal += new Vector3(0, 3, 0);
                }

                normals[x + y * vertCount] = normal.normalized;
            }
        }

        return normals;
    }

	void initTerrainTexture()
	{

		Texture2D texture = (Texture2D)groundRenderers[0].sharedMaterial.GetTexture("_ColorTexture");
		int tw = texture.width;
		int th = texture.height;
		for(int i = 1; i < texture.mipmapCount; i++)
		{
			int mw = (int)(tw/(Mathf.Pow(2, i))); //mip map width
			int mh = (int)(tw/(Mathf.Pow(2, i))); //mip map height
			int pw = tw/mw; //pixel width
			int ph = th/mh; //pixel height

			Color[] newPixels = new Color[texture.GetPixels32(i).Length];
			for(int x = 0; x < mw; x++)
			{
				for(int y = 0; y < mh; y++)
				{
					Color color = new Color();
					int count = 0;
					for(int px = 0; px < pw; px++)
					{
						for(int py = 0; py < ph; py++)
						{
							Color pixel = texture.GetPixel(px+x*pw, py+y*ph);
							if(pixel.a == 1)
							{
								color+=pixel;
								count++;
							}
						}
					}

					newPixels[x + y*mw] = color/count;
				}
			}

			texture.SetPixels(newPixels, i);
		}
		texture.Apply(false);

		groundRenderers[0].sharedMaterial.SetTexture("_ColorTexture", texture);
	}
	/*
	void generateHeights()
	{
		tileMap = new TileMap(4,1);
        tileMap.generateTeamMap();
		tileMap.generateRiver(new Vector2i((tileMap.getMainMapSize() - 1), (tileMap.getMainMapSize() - 1)/2), new Vector2i((tileMap.getMainMapSize() - 1)/2,(tileMap.getMainMapSize() - 1)),
		                      6, 30, -2, 0, 8, 3); 
		
		tileMap.smoothMap(4, 2);
		
		tileMap.smoothMap(1, 1);
		tileMap.flattenBaseAndSummons();
	}
	*/
	void addObjects()
    {
        for (int i = 0; i < 4; i++)
        {
            Cave cave = tileMap.getCave(i);
            Vector2 pos1 = tileMap.cavePos[i].toVector2();
            Vector2 pos2 = cave.mainEntrance.toVector2();
            Tile tile1 = tileMap.getTile(new Vector2i(pos1));
            Tile tile2 = tileMap.getTile(new Vector2i(pos2));

            float rot = cave.getExitRotation() * Mathf.Rad2Deg;
            tile1.setTileObject(new WarpObject(new Vector3(pos1.x, getHeight(pos1), pos1.y), 0, "caveEntrance", cave.mainDoormat, rot+180));
            tile2.setTileObject(new WarpObject(new Vector3(pos2.x, getHeight(pos2), pos2.y), rot, "caveExit", pos1 + new Vector2(0, 1), 180));

            foreach (Vector2 v in cave.torchPositions)
            {
                tileMap.getTile(new Vector2i(v)).setTileObject(new EyecandyObject(new Vector3(v.x, getHeight(v), v.y), 0, "walltorch"));
            }

        }

        //Add the hall entrance
        {
            Cave cave = tileMap.getCave(4);
            Vector2[] baseOffset = new Vector2[]
            {
                new Vector2(0, baseSize*4),
                new Vector2(0, -baseSize*4),
                new Vector2(baseSize*4, 0),
                new Vector2(-baseSize*4, 0),
            };

            List<Vector2i> entrances = cave.entrancePoses;
            List<Vector2> doormats = cave.doorMatPositions;
            for (int i = 0; i < entrances.Count; i++)
            {
                Vector2 pos1 = new Vector2(tileMap.getMainMapSize()/2,tileMap.getMainMapSize()/2)  + baseOffset[i];
                Vector2 pos2 = entrances[i].toVector2();
                Tile tile1 = tileMap.getTile(new Vector2i(pos1));
                Tile tile2 = tileMap.getTile(new Vector2i(pos2));

                float rot = cave.getExitRotation(i) * Mathf.Rad2Deg;
                tile1.setTileObject(new WarpObject(new Vector3(pos1.x, getHeight(pos1), pos1.y), 0, "caveEntrance", cave.doorMatPositions[i], rot + 180));
                tile2.setTileObject(new WarpObject(new Vector3(pos2.x, getHeight(pos2), pos2.y), rot, "caveExit", pos1 + new Vector2(0, 1), 180));
            }

            foreach (Vector2 v in cave.torchPositions)
            {
                tileMap.getTile(new Vector2i(v)).setTileObject(new EyecandyObject(new Vector3(v.x, getHeight(v), v.y), Random.Range(0, 4)*90, "thehall_pillar"));
            }
        }

        for (int x = 0; x < getMainMapSize(); x++)
        {
            for (int y = 0; y < getMainMapSize(); y++)
            {
                Tile tile = tileMap.getTile(x, y);
                GroundType ground = tile.getGroundType();
                if (ground.spawnResource())
                {
                    Vector2 pos = new Vector2(x + 0.4f + Random.value * 0.2f, y + 0.4f + Random.value * 0.2f);
                    tile.setTileObject(ground.getRandomResource(new Vector3(pos.x, getHeight(pos), pos.y)));
                }
                else
                {
                    int lootAmount = ground.spawnLootAmount();
                    for (int i = 0; i < lootAmount; i++)
                    {
                        Vector2 pos = new Vector2(x + Random.value, y + Random.value);
                        ItemData itemData = ground.getRandomLoot(new Vector3(pos.x, getHeight(pos), pos.y));
                        if (itemData != null)
                        {
                            tileMap.getTile(x, y).addLoot(itemData.getLootableObject(new Vector3(pos.x, getHeight(pos), pos.y),
                                                                                     Quaternion.Euler(0, Random.value * 360, 0)));

                        }
                    }
                }
                int candyAmount = ground.spawnEyecandyAmount();
                for (int i = 0; i < candyAmount; i++)
                {
                    Vector2 pos = new Vector2(x + Random.value, y + Random.value);
                    Eyecandy eyecandy = ground.getRandomEyecandy(new Vector3(pos.x, getHeight(pos), pos.y));
                    if (eyecandy != null) tileMap.getTile(x, y).addEyecandy(eyecandy);
                }
            }
        }
    }

    public void addSpawners()
    {
        //Random.seed = 10;
        for(int i = 0; i < tileMap.getCaves().Count; i++)
        {
            Cave cave = tileMap.getCaves()[i];
            if(i == 4)
            {
                GameMaster.addDaySpawner("trollking", 1, cave.bossPos);
            }
            else
            {
                GameMaster.addDaySpawner("troll", 1, cave.bossPos);
            }
            GameMaster.addDaySpawner("goblin", 4, cave.bossPos);
        }

        

        for (int x = 0; x < getMainMapSize(); x++)
        {
            for (int y = 0; y < getMainMapSize(); y++)
            {
                if (tileMap.getTile(x, y).isWalkable(-1) && Random.value > 0.994f)
                {
                    if (Random.value < 0.7f)
                    {
                        GameMaster.addDaySpawner("hare", 1, new Vector2i(x, y));
                    }
                    else if(Random.value < 0.1f)
                    {
                        GameMaster.addNightSpawner("wolf", 1, new Vector2i(x, y));
                    }
                    else if(Random.value < 0.1f)
                    {
                        GameMaster.addNightSpawner("goblin", Random.Range(3,6), new Vector2i(x, y));
                    }
                }
            }
        }
    }

    public void addRoadSpawners(TrialOfTheGods mode)
    {
        for(int i = 0; i < mode.teams.Length; i++)
        {
            
            for(int j = 0; j < mode.teams[i].roads.Length; j++)
            {
                Road road = mode.teams[i].roads[j];
                List<WorldPoint> points = new List<WorldPoint>(road.getWaypoints());
                GameMaster.addDaySpawner("goblin", Random.Range(3, 6), mode.teams[i].summonPositions[j], points);
            }
        }
    }

	public void addAnimals()
	{
        /*
       
		for(int x = tileMap.basePos.x - baseSize; x <= tileMap.basePos.x + baseSize; x += baseSize*2)
		{
			for(int y = tileMap.basePos.y - baseSize; y <= tileMap.basePos.y + baseSize; y += baseSize*2)
			{
				AIUnit unit = new AIUnit("goblin", new Vector3(x, 0, y), Vector3.zero, GameMaster.getNextUnitID());
				GameMaster.addUnit(unit);
			}
		}
		for(int x = 0; x < getMainMapSize(); x++)
		{
			for(int y = 0; y < getMainMapSize(); y++)
			{
				if(tileMap.getTile(x, y).isWalkable(-1) && Random.value > 0.994f)
				{
					if(Random.value < 0.9f)
					{
						AIUnit unit = new AIUnit("hare", new Vector3(x, 0, y), Vector3.zero, GameMaster.getNextUnitID());
						GameMaster.addUnit(unit);
					}
					else
					{
						AIUnit unit = new AIUnit("wolf", new Vector3(x, 0, y), Vector3.zero, GameMaster.getNextUnitID());
						GameMaster.addUnit(unit);
					}
				}
			}
		}
         */
       
	}
	
	public static void RecalculateTangents(Mesh mesh)
	{
		int triangleCount = mesh.triangles.Length;
		int vertexCount = mesh.vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for(long a = 0; a < triangleCount; a+=3)
		{
			long i1 = mesh.triangles[a+0];
			long i2 = mesh.triangles[a+1];
			long i3 = mesh.triangles[a+2];
			
			Vector3 v1 = mesh.vertices[i1];
			Vector3 v2 = mesh.vertices[i2];
			Vector3 v3 = mesh.vertices[i3];
			
			Vector2 w1 = mesh.uv[i1];
			Vector2 w2 = mesh.uv[i2];
			Vector2 w3 = mesh.uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float r = 1.0f / (s1 * t2 - s2 * t1);
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		
		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = mesh.normals[a];
			Vector3 t = tan1[a];
			
			Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		mesh.tangents = tangents;
	}
	
	
	void generateWater()
	{
		GameObject water = new GameObject("water");
		MeshFilter meshfilter = water.AddComponent<MeshFilter>();
		
		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(0,0, 0);
		verts[1] = new Vector3(getMainMapSize(),0,0);
		verts[2] = new Vector3(0,0,getMainMapSize());
		verts[3] = new Vector3(getMainMapSize(),0,getMainMapSize());
		
		int[] tris = new int[6];
		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		tris[3] = 2;
		tris[4] = 3;
		tris[5] = 1;
		
		Vector2[] newUV = new Vector2[verts.Length];
		for(int uv = 0; uv < newUV.Length; uv++)
		{
			newUV[uv] = new Vector2(verts[uv].x, verts[uv].z);
		}
		Mesh mesh = new Mesh();
		meshfilter.mesh = mesh;
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = newUV;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		RecalculateTangents(mesh);
		
		waterRenderer = water.AddComponent<MeshRenderer>();
		waterRenderer.material = waterMaterial;
		
		this.water = water.transform;
		water.transform.position = new Vector3(0,0.1f,0);
	}
	
	// Update is called once per frame
	void Update () {
		waterOffset+=0.2f*Time.deltaTime;
		if(waterOffset > 1) waterOffset--;
		waterRenderer.material.SetTextureOffset("_BumpMap", new Vector2(waterOffset, 0));
		
		timer+=Time.deltaTime;
		water.position = new Vector3(0,Mathf.Sin (timer)/40 + 0.5f, 0);
		
		//Debug.Log (lastPos.x + ", " + lastPos.y);
		Vector2i newPos = new Vector2i(controlledUnit.get2DPos());
		
		if(lastPlayerPos != newPos)
		{
			activateTileRow(newPos + cameraOffset, lastPlayerPos + cameraOffset);
            TimeManager.Instance.setIndoors(newPos.x > tileMap.getMainMapSize() || newPos.y > tileMap.getMainMapSize());
			lastPlayerPos = newPos;
		}

		foreach(Tile tile in activeTiles)
		{
			tile.renderEyecandy();
		}

        for (int i = 0; i < normalsstart.Count; i++)
        {
            if (i % 4 == 0) Debug.DrawLine(normalsstart[i], normalsend[i], Color.white);
            if (i % 4 == 1) Debug.DrawLine(normalsstart[i], normalsend[i], Color.yellow);
            if (i % 4 == 2) Debug.DrawLine(normalsstart[i], normalsend[i], Color.blue);
            if (i % 4 == 3)
            {
                Debug.DrawLine(normalsstart[i], normalsend[i], Color.magenta);
             //   Debug.Log(normalsend[i] - normalsstart[i]);

            }
        }
	}

	public static void addActiveTile(Tile tile)
	{
		activeTiles.Add(tile);
	}

	public static void removeActiveTile(Tile tile)
	{
		activeTiles.Remove(tile);
	}
	
	public bool activateTileIfInActivationRangeOfControlledUnit(int tileX, int tileY)
	{
		Vector2i unitTile = new Vector2i(controlledUnit.get2DPos());
		if(Mathf.Abs(tileX - unitTile.x) <= resourceTileActivationRange &&
		   Mathf.Abs(tileY - unitTile.y) <= resourceTileActivationRange)
		{
			tileMap.getTile(tileX, tileY).activateTile();
			return true;
		}
		
		return false;
	}
	
	void inactivateTileSquare(Vector2i pos)
	{
		for(int x = pos.x - resourceTileInactivationRange; x < pos.x + resourceTileInactivationRange + 1; x++)
		{
			for(int y = pos.y - resourceTileInactivationRange; y < pos.y + resourceTileInactivationRange + 1; y++)
			{
				if(tileMap.isValidTile(x, y))
				{
					if(tileMap.getTile(x,y).hasTileObject())
					{
						tileMap.getTile(x, y).inactivateTile();
					}
					else if(x >= pos.x - eyecandyTileInactivationRange && 
					        x <= pos.x + eyecandyTileInactivationRange &&
					        y >= pos.y - eyecandyTileInactivationRange && 
					        y <= pos.y + eyecandyTileInactivationRange)
					{
						tileMap.getTile(x, y).inactivateTile();
					}
				}
			}
		}
		ObjectPoolingManager.Instance.ShrinkPools();
	}
	
	void activateTileSquare(Vector2i pos)
	{
		for(int x = pos.x - resourceTileActivationRange; x < pos.x + resourceTileActivationRange + 1; x++)
		{
			for(int y = pos.y - resourceTileActivationRange; y < pos.y + resourceTileActivationRange + 1; y++)
			{
				if(tileMap.isValidTile(x, y))
				{
					if(tileMap.getTile(x,y).hasTileObject())
					{
						tileMap.getTile(x, y).activateTile();
					}
					else if(x >= pos.x - eyecandyTileActivationRange && 
					        x <= pos.x + eyecandyTileActivationRange &&
					        y >= pos.y - eyecandyTileActivationRange && 
					        y <= pos.y + eyecandyTileActivationRange)
					{
						tileMap.getTile(x, y).activateTile();
					}
				}
				
			}
		}
	}
	
	void activateTileRow(Vector2i newPos, Vector2i lastPos)
	{
		int dx = newPos.x - lastPos.x;
		int dy = newPos.y - lastPos.y;
		
		if(Mathf.Abs(dx) > 1 || Mathf.Abs(dy) > 1)
		{
			inactivateTileSquare(lastPos);
			activateTileSquare(newPos);
			return;
		}
		
		if(Mathf.Abs(dx) == 1)
		{
			//Activating the big stuff
			int x = newPos.x + resourceTileActivationRange * dx;
			for(int y = newPos.y-resourceTileActivationRange; y < newPos.y+resourceTileActivationRange+1; y++)
			{
				if(tileMap.isValidTile(x, y) && tileMap.getTile(x,y).hasTileObject())
				{
					tileMap.getTile(x, y).activateTile();
				}
			}
			
			x = newPos.x - resourceTileInactivationRange * dx;
			for(int y = newPos.y-resourceTileInactivationRange; y < newPos.y+resourceTileInactivationRange+1; y++)
			{
				if(tileMap.isValidTile(x, y) && tileMap.getTile(x,y).hasTileObject())
				{
					tileMap.getTile(x, y).inactivateTile();
				}
			}
			
			//Activating the small stuff
			x = newPos.x + eyecandyTileActivationRange * dx;
			for(int y = newPos.y-eyecandyTileActivationRange; y < newPos.y+eyecandyTileActivationRange+1; y++)
			{
				if(tileMap.isValidTile(x, y))
				{
					tileMap.getTile(x, y).activateTile();
				}
			}
			
			
			x = newPos.x - eyecandyTileInactivationRange * dx;
			for(int y = newPos.y-eyecandyTileInactivationRange; y < newPos.y+eyecandyTileInactivationRange+1; y++)
			{
				if(tileMap.isValidTile(x, y) && !tileMap.getTile (x,y).hasTileObject())
				{
					tileMap.getTile(x, y).inactivateTile();
				}
			}
		}
		
		if(Mathf.Abs(dy) == 1)
		{
			//Activating the big stuff
			int y = newPos.y + resourceTileActivationRange * dy;
			for(int x = newPos.x-resourceTileActivationRange; x < newPos.x+resourceTileActivationRange+1; x++)
			{
				if(tileMap.isValidTile(x, y) && tileMap.getTile (x,y).hasTileObject())
				{
					tileMap.getTile(x, y).activateTile();
				}
			}
			
			y = newPos.y - resourceTileInactivationRange * dy;
			for(int x = newPos.x-resourceTileInactivationRange; x < newPos.x+resourceTileInactivationRange+1; x++)
			{
				if(tileMap.isValidTile(x, y) && tileMap.getTile (x,y).hasTileObject())
				{
					tileMap.getTile(x, y).inactivateTile();
				}
			}
			
			//Activating the small stuff
			y = newPos.y + eyecandyTileActivationRange * dy;
			for(int x = newPos.x-eyecandyTileActivationRange; x < newPos.x+eyecandyTileActivationRange+1; x++)
			{
				if(tileMap.isValidTile(x, y) && !tileMap.getTile (x, y).hasTileObject())
				{
					tileMap.getTile(x, y).activateTile();
				}
			}
			
			y = newPos.y - eyecandyTileInactivationRange * dy;
			for(int x = newPos.x-eyecandyTileInactivationRange; x < newPos.x+eyecandyTileInactivationRange+1; x++)
			{
				if(tileMap.isValidTile(x, y) && !tileMap.getTile (x,y).hasTileObject())
				{
					tileMap.getTile(x, y).inactivateTile();
				}
			}
		}
		ObjectPoolingManager.Instance.ShrinkPools();
	}
	
	public static float getHeight(Vector2 point)
	{
		RaycastHit hit;
		if(Physics.Raycast(new Vector3(point.x, 50, point.y), Vector3.down, out hit, Mathf.Infinity , 1 << 8))
		{
			return hit.point.y;           
		}
		return 0;
		//		int x = (int)point.x;
		//		int y = (int)point.y;
		//
		//		int sx = x/WorldSection.SIZE;
		//		int sz = y/WorldSection.SIZE;
		//
		//		int xp = x+1;
		//		int yp = y+1;
		//
		//
		//		float tri0 = getSectionVertHeight(x, y);
		//		float tri1 = getSectionVertHeight(xp, y);
		//		float tri2 = getSectionVertHeight(x, yp);
		//		float tri3 = getSectionVertHeight(xp, yp);
		//
		//		float height = 0;
		//		float sqX = point.x - x;
		//		float sqY = point.y - y;
		//		if((sqX + sqY) < 1)
		//		{
		//			height = tri0;
		//			height+= (tri1 - tri0) * sqX;
		//			height+= (tri2 - tri0) * sqY;
		//		}
		//		else
		//		{
		//			height = tri3;
		//			height+= (tri1 - tri3) * (1.0f - sqX);
		//			height+= (tri2 - tri3) * (1.0f - sqY);
		//		}
		//		return height;
		
	}

    public static float getHeight(Vector2 point, out Vector3 normal)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(point.x, 50, point.y), Vector3.down, out hit, Mathf.Infinity, 1 << 8))
        {
            normal = hit.normal;
            return hit.point.y;
        }
        normal = new Vector3(0, 1, 0);
        return 0;
    }
	
	public float getSectionVertHeight(int x, int y)
	{
		
		int sx = x/(WorldSection.SIZE);
		int sy = y/(WorldSection.SIZE);
		Debug.Log (sx + ":" + x + ", " + sy + ":" + y);
		if(sx >= tileMap.sectionCount || sy >= tileMap.sectionCount)
		{
			Debug.Log (x + ", " + y);
			return 1;
		}
		
		return sections[sx, sy].getVertHeight(x-sx*WorldSection.SIZE, y-sy*WorldSection.SIZE);
	}
	
	Vector2 getRandomVector2(Vector2 start, int dist)
	{
		return start + new Vector2(Random.Range(-dist, dist+1), Random.Range (-dist, dist+1));
	}
	
	public static int getMapSize()
	{
		return tileMap.getSize();
	}

    public static int getMainMapSize()
    {
        return tileMap.getMainMapSize();
    }
	
	public static TileMap getMap()
	{
		return tileMap;
	}
	
	public void setUnit(Unit unit)
	{
		controlledUnit = unit;
		activateTileSquare(new Vector2i(controlledUnit.get2DPos())+cameraOffset);
	}
	
	public void setDrawGrid(bool draw)
	{
		drawingGrid = draw;
		float value = draw ? 1 : 0;
		foreach(MeshRenderer renderer in groundRenderers)
		{
			renderer.material.SetFloat("_DrawGrid", value);
		}
	}
	
	public void toggleDrawGrid()
	{
		setDrawGrid(!drawingGrid);
	}

	public void changeSnowAmount(float change)
	{
		snowAmount+=change;
		foreach(MeshRenderer renderer in groundRenderers)
		{
			renderer.material.SetFloat("_SnowAmount", snowAmount);
		}
	}

    void generateWorldSectionsNew()
    {
        sections = new WorldSection[tileMap.sectionCount, tileMap.sectionCount];
        int[,] tileMapColor = new int[tileMap.getSize(), tileMap.getSize()];
        for (int y = 0; y < tileMap.getSize(); y++)
        {
            for (int x = 0; x < tileMap.getSize(); x++)
            {
                tileMapColor[x, y] = tileMap.getTile(x, y).getColor();
            }
        }
        //Texture2D[] textures = WorldSection.getWorldSplatTexture(tileMapColor, (tileMap.sectionCount*WorldSection.SIZE*2) + 1);
        Texture2D[] textures = WorldSection.getWorldSplatTexture(tileMapColor, tileMap.sectionCount * WorldSection.SIZE);
        Material material = (Material)Resources.Load("terrain");


        int xtri = WorldSection.SIZE * 2;
        int ytri = WorldSection.SIZE;

        Vector3[,] terrainNormals = new Vector3[xtri * tileMap.sectionCount, ytri * tileMap.sectionCount];

        for (int y = 0; y < tileMap.sectionCount; y++)
        {
            for (int x = 0; x < tileMap.sectionCount; x++)
            {
                WorldSection ws = new WorldSection(new Vector2i(x, y));
                for (int ty = y * ytri; ty < (y + 1) * ytri; ty++)
                {
                    for (int tx = x * xtri; tx < (x + 1) * xtri; tx++)
                    {
                        int stx = tx - x * xtri;
                        int sty = ty - y * ytri;
                        terrainNormals[tx, ty] = ws.getTriangleNormal(stx + sty * xtri);
                    }
                }

                sections[x, y] = ws;

            }
        }

        // Vector3[] normals = calculateNormals(terrainNormals);

        groundRenderers = new MeshRenderer[tileMap.sectionCount * tileMap.sectionCount];
        for (int y = 0; y < tileMap.sectionCount; y++)
        {
            for (int x = 0; x < tileMap.sectionCount; x++)
            {
                sections[x, y].calculateNormals(terrainNormals);
                //sections[x, y].setNormals(normals);

                GameObject section = new GameObject("World Section(" + x + "," + y + ")");
                section.tag = "Ground";
                MeshFilter filter = section.AddComponent<MeshFilter>();
                filter.mesh = sections[x, y].getMesh();

                MeshRenderer renderer = section.AddComponent<MeshRenderer>();

                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.material = material;

                groundRenderers[x + y * tileMap.sectionCount] = renderer;

                MeshCollider col = section.AddComponent<MeshCollider>();
                col.sharedMesh = filter.mesh;

                section.transform.SetParent(transform);
                section.transform.localPosition = new Vector3(x * WorldSection.SIZE, 0, y * WorldSection.SIZE);
                section.layer = 8;


            }
        }

        foreach (MeshRenderer groundRenderer in groundRenderers)
        {
            for (int i = 0; i < SplatMapColor.colorCount; i++)
            {
                groundRenderer.material.SetTexture("_Splat" + i, textures[i]);
            }

            int scale = (tileMap.sectionCount * WorldSection.SIZE) / 16;
            groundRenderer.material.SetTextureScale("_ColorTexture", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture2", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture3", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture4", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture5", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture6", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture7", new Vector2(scale, scale));
            //		groundRenderer.sharedMaterial.SetTextureScale("_Texture8", new Vector2(scale, scale));
            groundRenderer.material.SetTextureScale("_GridTexture", new Vector2(tileMap.sectionCount * WorldSection.SIZE, tileMap.sectionCount * WorldSection.SIZE));
        }
        //initTerrainTexture();
    }
	
	
}
