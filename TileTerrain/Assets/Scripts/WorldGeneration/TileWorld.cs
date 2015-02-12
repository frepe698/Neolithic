using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileWorld : MonoBehaviour {
	
	private Material waterMaterial;
	private float waterOffset;
	
	TileMap tileMap;
	
	public static int summonDistanceFromBase = 4;
	public static int baseBounds = 3;
	public static int summonBounds = 3;
	public static int summonSize = 2;
	public static int baseSize = 5;
	public static int mapSize = 64;
	
	public bool cliffs = true;
	public bool testTiles = true;
	private readonly static int tilesize = 2;
	private readonly static int TILE_HEIGHT = 1;
	private readonly static int WATER_LEVEL = 0;
	
	public static int maxHeight = 200;
	public static int minHeight = -200;
	
	
	GameObject tile;
	GameObject slope;
	GameObject mid1000;
	GameObject mid0111;
	GameObject mid1010;
	GameObject water_tile;
	GameObject water_mid1000;
	GameObject water_mid0111;
	GameObject water_mid1010;
	GameObject water_slope;
	GameObject tree;
	// Use this for initialization
	
	void generateHeights()
	{
		tileMap = new TileMap(mapSize);
		//Random.seed = 1;
		tileMap.generateKeyHeights();
		tileMap.generateBase();
		tileMap.generateSummons();
		tileMap.generateRoads();
		tileMap.generateRest();
	}
	
	
	
	Vector2 getNextTileDir(Vector2 v1, Vector2 v2)
	{
		Vector2 dist = v2-v1;
		if(Mathf.Abs(dist.x) > Mathf.Abs(dist.y))
		{
			return new Vector2(dist.x/Mathf.Abs (dist.x), 0);
		}
		else
		{
			return new Vector2(0,dist.y/Mathf.Abs (dist.y));
		}
	}
	
	
	
	
	
	void loadTilePrefabs()
	{
		water_tile = (GameObject)Resources.Load ("tile_water");
		water_mid1000 = (GameObject)Resources.Load ("tile_mid_water_1000");
		water_mid0111 = (GameObject)Resources.Load ("tile_mid_water_0111");
		water_mid1010 = (GameObject)Resources.Load ("tile_mid_water_1010");
		water_slope = (GameObject)Resources.Load ("tile_water_slope");
		tree = (GameObject)Resources.Load ("tree");
		if(cliffs)
		{
			tile = (GameObject)Resources.Load ("tile_0");
			slope = (GameObject)Resources.Load ("tile_fill_cliff");
			mid1000 = (GameObject)Resources.Load ("tile_mid_cliff_1000");
			mid0111 = (GameObject)Resources.Load ("tile_mid_cliff_0111");
			mid1010 = (GameObject)Resources.Load ("tile_mid_cliff_1010");
		}
		else{
			tile = (GameObject)Resources.Load ("tile_0");
			slope = (GameObject)Resources.Load ("tile_1");
			mid1000 = (GameObject)Resources.Load ("tile_mid_1000");
			mid0111 = (GameObject)Resources.Load ("tile_mid_0111");
			mid1010 = (GameObject)Resources.Load ("tile_mid_1010");
		}
	}
	
	void instantiateTestHeightTiles()
	{
		//		float xoff = Random.Range(0, 10000);
		//		float yoff = Random.Range(0, 10000);
		//
		tree = (GameObject)Resources.Load ("tree");
		GameObject testRoad = (GameObject)Resources.Load ("test_road");
		GameObject testWater = (GameObject)Resources.Load ("test_water");
		GameObject testGround = (GameObject)Resources.Load ("test_ground");
		
		//float txoff = Random.Range(0, 10000);
		//float tyoff = Random.Range(0, 10000);
		for(int x = 0; x < mapSize; x++){
			for(int z = 0; z < mapSize; z++){
				//height[x + z*size] = Random.Range (0, 2);
//				float smoothness = 30.0f;
				//heightMap[x + z*size] = Mathf.RoundToInt((Mathf.PerlinNoise(((float)x+xoff)/smoothness, ((float)z+yoff)/smoothness))*10);
				if(tileMap.getTile(x, z).height <= WATER_LEVEL) // tile is water
				{
					GameObject t = Instantiate(testWater, new Vector3(x*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, z*tilesize*2), Quaternion.identity) as GameObject;
					t.transform.SetParent(transform);
					t.transform.localScale = new Vector3(4, 2, 4);
				}
				else //tile is not water
				{
					GameObject t;
					if(tileMap.getTile(x, z).state == Tile.stFixed)
					{
						t = Instantiate(testRoad, new Vector3(x*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, z*tilesize*2), Quaternion.identity) as GameObject;
					}
					else
					{
						t = Instantiate(testGround, new Vector3(x*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, z*tilesize*2), Quaternion.identity) as GameObject;
					}
					t.transform.SetParent(transform);
					t.transform.localScale = new Vector3(4, 2, 4);
					
					//					for(float a = -0.25f; a < 0.15f; a+=0.38f)
					//					{
					//						for(float b = -0.25f; b < 0.15f; b+=0.38f)
					//						{
					//							float posx = x +a+Random.Range(0.0f, 0.21f);
					//							float posz = z + b+Random.Range(0.0f, 0.21f);
					//							if((Mathf.PerlinNoise((posx+txoff)/2.0f, (posz+tyoff)/2.0f)) < 0.5f){
					//								GameObject tred = Instantiate(tree, new Vector3(posx*tilesize*2, tiles[x, z].height*TILE_HEIGHT, posz*tilesize*2), Quaternion.identity) as GameObject;
					//								tred.transform.SetParent(transform);
					//							}
					//						}
					//					}
				}
			}
		}
	}
	
	void instantiateHeightTiles()
	{
		//		float xoff = Random.Range(0, 10000);
		//		float yoff = Random.Range(0, 10000);
		//		
		float txoff = Random.Range(0, 10000);
		float tyoff = Random.Range(0, 10000);
		for(int x = 0; x < mapSize; x++){
			for(int z = 0; z < mapSize; z++){
				//height[x + z*size] = Random.Range (0, 2);
				//float smoothness = 30.0f;
				//heightMap[x + z*size] = Mathf.RoundToInt((Mathf.PerlinNoise(((float)x+xoff)/smoothness, ((float)z+yoff)/smoothness))*10);
				if(tileMap.getTile(x, z).height <= WATER_LEVEL) // tile is water
				{
					GameObject t = Instantiate(water_tile, new Vector3(x*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, z*tilesize*2), Quaternion.identity) as GameObject;
					t.transform.SetParent(transform);
				}
				else //tile is not water
				{
					GameObject t = Instantiate(tile, new Vector3(x*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, z*tilesize*2), Quaternion.identity) as GameObject;
					t.transform.SetParent(transform);
					
					for(float a = -0.25f; a < 0.15f; a+=0.38f)
					{
						for(float b = -0.25f; b < 0.15f; b+=0.38f)
						{
							float posx = x +a+Random.Range(0.0f, 0.21f);
							float posz = z + b+Random.Range(0.0f, 0.21f);
							if((Mathf.PerlinNoise((posx+txoff)/2.0f, (posz+tyoff)/2.0f)) < 0.5f){
								GameObject tred = Instantiate(tree, new Vector3(posx*tilesize*2, tileMap.getTile(x, z).height*TILE_HEIGHT, posz*tilesize*2), Quaternion.identity) as GameObject;
								tred.transform.SetParent(transform);
							}
						}
					}
				}
			}
		}
	}
	
	void instantiateFillTiles()
	{
		for(int x = 0; x < mapSize-1; x++){
			for(int z = 0; z < mapSize-1; z++){
				int firstHeight = tileMap.getTile(x, z).height;
				//create fill in z direction
				int nextZ = tileMap.getTile(x, z+1).height;
				
				if(firstHeight == nextZ){
					instTile(TileType.tile, new Vector3(x*tilesize*2, firstHeight*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.identity);
				}else if(firstHeight < nextZ){
					instTile(TileType.slope, new Vector3(x*tilesize*2, firstHeight*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(new Vector3(0, 180, 0)));
				}
				else{
					instTile(TileType.slope, new Vector3(x*tilesize*2, nextZ*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(new Vector3(0, 0, 0)));
				}
				
				//create fill in x direction
				int nextX = tileMap.getTile(x+1, z).height;
				
				if(firstHeight == nextX){
					instTile(TileType.tile, new Vector3(x*tilesize*2+tilesize, firstHeight*TILE_HEIGHT, z*tilesize*2), Quaternion.identity);
				}else if(firstHeight < nextX){
					instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, firstHeight*TILE_HEIGHT, z*tilesize*2), Quaternion.Euler(new Vector3(0, 270, 0)));
				}
				else{
					instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, nextX*TILE_HEIGHT, z*tilesize*2), Quaternion.Euler(new Vector3(0, 90, 0)));
				}
				
				//create fill middle
				int ul = tileMap.getTile(x, z).height;
				int ur = tileMap.getTile(x+1, z).height;
				int ll = tileMap.getTile(x, z+1).height;
				int lr = tileMap.getTile(x+1, z+1).height;
				
				if((ul + ur + ll + lr) == ul*4){ // if all are the same height
					instTile(TileType.tile, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.identity);
				}
				else if(ul == ur && ll == lr) //if 2 adjacent tiles are the same height
				{
					if(ul < ll)
					{
						instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 180, 0));
					}else
					{
						instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, ll*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 0, 0));
					}
				}
				else if(ul == ll && ur == lr) //if 2 adjacent tiles are the same height
				{
					if(ul < ur)
					{
						instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 270, 0));
					}else
					{
						instTile(TileType.slope, new Vector3(x*tilesize*2+tilesize, ur*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 90, 0));
					}
				}
				else if(ul == lr && ur == ll) // if the crossing tiles are the same height
				{
					if(ul > ll)
					{
						instTile(TileType.mid1010, new Vector3(x*tilesize*2+tilesize, ll*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 90, 0));
					}
					else
					{
						instTile(TileType.mid1010, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 0, 0));
					}
				}
				else if((ul + ll + lr) == ul*3) //if all but "ur" are the same height
				{
					if(ur > ul)
					{
						instTile(TileType.mid1000, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 180, 0));
					}
					else{
						instTile(TileType.mid0111, new Vector3(x*tilesize*2+tilesize, ur*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 180, 0));
					}
				}
				else if((ul + ur + ll) == ul*3) //if all but "lr" are the same height
				{
					if(lr > ul)
					{
						instTile(TileType.mid1000, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 90, 0));
					}
					else{
						instTile(TileType.mid0111, new Vector3(x*tilesize*2+tilesize, lr*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 90, 0));
					}
				}
				else if((ul + ur + lr) == ul*3) //if all but "ll" are the same height
				{
					if(ll > ul)
					{
						instTile(TileType.mid1000, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 0, 0));
					}
					else{
						instTile(TileType.mid0111, new Vector3(x*tilesize*2+tilesize, ll*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 0, 0));
					}
				}
				else if(( ur + ll + lr) == ur*3) //if all but "ul" are the same height
				{
					if(ul > ur)
					{
						instTile(TileType.mid1000, new Vector3(x*tilesize*2+tilesize, ur*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 270, 0));
					}
					else{
						instTile(TileType.mid0111, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.Euler(0, 270, 0));
					}
				}
				else{
					instTile(TileType.tile, new Vector3(x*tilesize*2+tilesize, ul*TILE_HEIGHT, z*tilesize*2+tilesize), Quaternion.identity);
				}                     
			}
		}
	}
	
	void Awake () {
		waterMaterial = (Material)Resources.Load ("water");
		
		
		
		generateHeights();
		if(!testTiles)
		{
			loadTilePrefabs();
			instantiateHeightTiles();
			instantiateFillTiles();
		}
		else
		{
			instantiateTestHeightTiles();
		}
		
	}
	
	void instTile(TileType type, Vector3 pos, Quaternion rot)
	{
		GameObject go;
		if(pos.y <= WATER_LEVEL)
		{
			switch(type){
			case TileType.tile:
			default:
				go = Instantiate(water_tile, pos, rot) as GameObject;
				break;
			case TileType.slope:
				go = Instantiate(water_slope, pos, rot) as GameObject;
				break;
			case TileType.mid1000:
				go = Instantiate(water_mid1000, pos, rot) as GameObject;
				break;
			case TileType.mid0111:
				go = Instantiate(water_mid0111, pos, rot) as GameObject;
				break;
			case TileType.mid1010:
				go = Instantiate(water_mid1010, pos, rot) as GameObject;
				break;
			}
		}
		else
		{
			switch(type){
			case TileType.tile:
			default:
				go = Instantiate(tile, pos, rot) as GameObject;
				break;
			case TileType.slope:
				go = Instantiate(slope, pos, rot) as GameObject;
				break;
			case TileType.mid1000:
				go = Instantiate(mid1000, pos, rot) as GameObject;
				break;
			case TileType.mid0111:
				go = Instantiate(mid0111, pos, rot) as GameObject;
				break;
			case TileType.mid1010:
				go = Instantiate(mid1010, pos, rot) as GameObject;
				break;
			}
		}
		go.transform.SetParent(transform);
	}
	
	enum TileType
	{
		tile, slope, mid1000, mid0111, mid1010
	}
	
	// Update is called once per frame
	void Update () {
		waterOffset+=0.2f*Time.deltaTime;
		if(waterOffset > 1) waterOffset--;
		waterMaterial.SetTextureOffset("_BumpMap", new Vector2(waterOffset, 0));
		
		//		for(int i = 0; i < 4; i++)
		//		{
		//			roads[i].draw();
		//		}
		//		if(Input.GetKeyDown("space"))
		//		{
		//			for(int i = 0; i < 4; i++)
		//			{
		//				roads[i].subdivide();
		//			}
		//
		//		}
		for(int i = 0; i < 4; i++)
		{
			//roadAreas[i].draw(Color.blue);
		}
		
		
	}
	
	Vector2 getRandomVector2(Vector2 start, int dist)
	{
		return start + new Vector2(Random.Range(-dist, dist+1), Random.Range (-dist, dist+1));
	}
	
	Vector2 autoBounds(Vector2 pos, int size)
	{
		if(pos.x < size)
		{
			pos.x = size;
		}
		else if(pos.x > mapSize - size)
		{
			pos.x = mapSize - size;
		}
		if(pos.y < size)
		{
			pos.y = size;
		}
		else if(pos.y > mapSize - size)
		{
			pos.y = mapSize - size;
		}
		return pos;
	}
	
	
}
