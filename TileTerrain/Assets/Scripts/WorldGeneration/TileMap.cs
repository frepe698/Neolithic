using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap {

    private int mapSize;
	private int mainMapSize;
    public readonly int sectionCount;
    public readonly int mainMapSectionCount;
	private Tile[,] tiles;

	// Key locations
	public Vector2i basePos;
	public Vector2i[] summonPos;
	Vector2i[] summonPosDirections = new Vector2i[]{new Vector2i(0, -1), new Vector2i(1, 0), new Vector2i(0, 1), new Vector2i(-1, 0)};
	Road[] roads;
	Line[] roadAreas;

    private List<Cave> caves;

	public int grassTiles = 0;
	public int roadTiles = 0;

	private float roughness = 0.15f;

	private float groundRoughness = 0.05f;

    public const float cliffThreshold = 0.7f;


	private readonly static int KEY_RES = 8;

	public TileMap(int mainMapSections, int caveSections)
	{
		initTiles((mainMapSections + caveSections) * WorldSection.SIZE);
        mainMapSize = mainMapSections * WorldSection.SIZE + 1;
        sectionCount = mainMapSections + caveSections;
        this.mainMapSectionCount = mainMapSections;
	}

	public void initTiles(int size)
	{
        mapSize = size;
		tiles = new Tile[size, size];
		for(int x = 0; x < size; x++)
		{
			for(int y = 0; y < size; y++)
			{
				tiles[x, y] = new Tile();
			}
		}
	}

	public void generateBase()
	{
		//basePos = new Vector2(Random.Range(mapSize/2 - halfBaseBounds, mapSize/2+halfBaseBounds), Random.Range(mapSize/2 - halfBaseBounds, mapSize/2+halfBaseBounds));
		basePos = (new Vector2i(Random.Range (0, 2), Random.Range(0, 2)) + new Vector2i(mainMapSize/KEY_RES/2, mainMapSize/KEY_RES/2))*KEY_RES;
		tiles[(int)basePos.x, (int)basePos.y].height = (short)Mathf.Max(tiles[(int)basePos.x, (int)basePos.y].height, 1);
		short baseHeight = (short)Mathf.Max(calculateAvgHeight(basePos.x, basePos.y, World.baseSize/2), 2);
		
		for(int x = (int)basePos.x-World.baseSize; x < (int)basePos.x+World.baseSize; x++)
		{
			for(int y = (int)basePos.y-World.baseSize; y < (int)basePos.y+World.baseSize; y++)
			{
				if(Vector2i.getDistance(new Vector2i(x, y), basePos) < World.baseSize)
				{
					tiles[x,y].height = baseHeight;
					tiles[x,y].ground = (short)GroundType.Type.Road;
				}
			}
		}
	}
	
	public void generateSummons()
	{
		Vector2i baseSmallPos = basePos/KEY_RES;
		//0 = up, 1 = right, 2 = down, 3 = left
		Vector2i[] summonBasePos = new Vector2i[]{
			summonPosDirections[0]*World.summonDistanceFromBase,
			summonPosDirections[1]*World.summonDistanceFromBase,
			summonPosDirections[2]*World.summonDistanceFromBase,
			summonPosDirections[3]*World.summonDistanceFromBase};

		summonPos = new Vector2i[4];
		for(int i = 0; i < 4; i++)
		{
			
			Vector2i myPos = summonPos[i] = keyHeightBounds(baseSmallPos + summonBasePos[i]+new Vector2i(Random.Range(-World.summonBounds, World.summonBounds+1),
			                                                                                            Random.Range(-World.summonBounds, World.summonBounds+1)))*KEY_RES;
			
			//myPos = summonPos[i] = autoBounds(myPos, summonSize);
			short height = (short)Mathf.Max(calculateAvgHeight(myPos.x, myPos.y, World.summonSize), 2);
			for(int x = (int)myPos.x-World.summonSize; x < (int)myPos.x+World.summonSize; x++)
			{
				for(int y = (int)myPos.y-World.summonSize; y < (int)myPos.y+World.summonSize; y++)
				{
					if(Vector2i.getDistance(new Vector2i(x, y), myPos) < World.summonSize)
					{
						tiles[x,y].height = height;
						tiles[x,y].ground = (short)GroundType.Type.Road;
					}
				}
			}
		}
	}
	
	public void generateRoads()
	{
		
		roadAreas = new Line[4];
		for(int i = 0; i < 4; i++)
		{
			float baseToI = Vector2i.getAngle(basePos, summonPos[i]);
			float baseToI2 = Vector2i.getAngle(basePos, summonPos[(i+1)%4]);
			float angle = baseToI + Mathf.DeltaAngle(baseToI, baseToI2)/2;
			roadAreas[i] = new Line(basePos, basePos + new Vector2i((int)Mathf.Cos(Mathf.Deg2Rad*angle), (int)Mathf.Sin (Mathf.Deg2Rad*angle))*mainMapSize);
		}
		
		roads = new Road[4];
		for(int i = 0; i < 4; i++)
		{
			roads[i] = new Road(new WorldPoint(basePos.x + summonPosDirections[i].x*World.baseSize, basePos.y+ summonPosDirections[i].y*World.baseSize, tiles[(int)basePos.x, (int)basePos.y].height),
			                    new WorldPoint(summonPos[i].x - summonPosDirections[i].x*World.summonSize, summonPos[i].y- summonPosDirections[i].y*World.summonSize, tiles[(int)summonPos[i].x, (int)summonPos[i].y].height),
			                    new Line[]{roadAreas[i], roadAreas[(i+3)%4]},Color.yellow);
			
			roads[i].subdivide();
			//roads[i].subdivide();
			//roads[i].subdivide();
			List<WorldPoint> wpts = roads[i].getWaypoints();
			for(int w = 0; w < wpts.Count-1; w++)
			{
				Vector2i dir = wpts[w+1].get2D() - wpts[w].get2D();
				int lastx = wpts[w].x;
				int lasty = wpts[w].y;
				int dx =  dir.x;
				int dy =  dir.y;
				float dh = (float)(wpts[w+1].height - wpts[w].height) / (float)(Mathf.Abs(dx)+Mathf.Abs(dy) + 0.000000001);
				float lastHeight = wpts[w].height;
				while(true)
				{
					tiles[lastx, lasty].height = (short)Mathf.FloorToInt(lastHeight);
					tiles[lastx,lasty].ground = (short)GroundType.Type.Road;
					if(dx == 0 && dy == 0) break;
					if(dy == 0)
					{
						int change = (int)((dx/Mathf.Abs(dx)));
						lastx += change;
						dx-=change;
					}
					else if(dx == 0)
					{
						int change = (int)((dy/Mathf.Abs(dy)));
						lasty += change;
						dy -=change;
					}
					else
					{
						if(Random.value > 0.5)
						{
							int change = (int)((dx/Mathf.Abs(dx)));
							lastx += change;
							dx-=change;
						}
						else
						{
							int change = (int)((dy/Mathf.Abs(dy)));
							lasty += change;
							dy -=change;
						}
					}
					lastHeight+=dh;
				}
			}
		}
	}
	
	public void generateKeyHeights()
	{
		for(int x = 0; x < mainMapSize/KEY_RES/2; x++)
		{
			for(int y = 0; y < mainMapSize/KEY_RES/2; y++)
			{
				//					float m = mapSize/8;
				//					short maxHeight = (short)(((m-x)/m + (m-y)/m)*50);
				tiles[x*KEY_RES*2, y*KEY_RES*2].height = (short)(Random.Range(World.minHeight, World.maxHeight));
			}
		}
		for(int x = 0; x < mainMapSize/KEY_RES; x++)
		{
			for(int y = 0; y < mainMapSize/KEY_RES; y++)
			{
				//					float m = mapSize/8;
				//					short maxHeight = (short)(((m-x)/m + (m-y)/m)*50);
				tiles[x*KEY_RES, y*KEY_RES].height = calculateAvgHeight(x*KEY_RES, y*KEY_RES, 8);
			}
		}
	}

    public void generateCaves()
    {
        caves = new List<Cave>();
        for(int x = mainMapSectionCount; x < sectionCount; x++)
        {
            for (int y = 0; y < mainMapSectionCount; y+=2)
            {
                Cave cave = new Cave(new Rect(x * WorldSection.SIZE, 
                    y * WorldSection.SIZE, 
                    WorldSection.SIZE,
                    WorldSection.SIZE*2));
                generateCave(cave);
                caves.Add(cave);
                
            }
        }
        for(int x = 0; x < mainMapSectionCount; x+=2)
        {
            for (int y = mainMapSectionCount; y < sectionCount; y++)
            {
                Cave cave = new Cave(new Rect(x * WorldSection.SIZE,
                    y * WorldSection.SIZE,
                    WorldSection.SIZE * 2,
                    WorldSection.SIZE));
                generateCave(cave);
                caves.Add(cave);
            }
        }

        //GENERATE THE HALL
        if(sectionCount - mainMapSectionCount > 0)
        {
            Cave theHall = new Cave(new Rect(mainMapSectionCount * WorldSection.SIZE,
            mainMapSectionCount * WorldSection.SIZE,
            WorldSection.SIZE,
            WorldSection.SIZE));
            generateTheHall(theHall);
            caves.Add(theHall);
        }
        
    }

    private void generateTheHall(Cave cave)
    {
        int width = cave.width;
        int height = cave.height;

        cave.addEntrancePos(new Vector2i(width / 2, height - cave.MARGIN) + cave.position, new Vector2(width / 2, height - cave.MARGIN - 2) + cave.position.toVector2());
        cave.addEntrancePos(new Vector2i(width / 2, cave.MARGIN) + cave.position, new Vector2(width / 2, cave.MARGIN + 2) + cave.position.toVector2());
        cave.addEntrancePos(new Vector2i(width - cave.MARGIN, height / 2) + cave.position, new Vector2(width - cave.MARGIN - 2, height / 2) + cave.position.toVector2());
        cave.addEntrancePos(new Vector2i(cave.MARGIN, height / 2) + cave.position, new Vector2(cave.MARGIN + 2, height / 2) + cave.position.toVector2());

        cave.bossPos = new Vector2i(width / 2, height / 2) + cave.position;

        int size = width/2 - cave.MARGIN + 2;
        setCircleHeight(1, cave.bossPos, size);
        setCircleGround(GroundType.Type.Mountain, cave.bossPos, size);
        setCircleFixed(cave.bossPos, size);

        //Set ground type
        for (int x = cave.position.x; x < cave.position.x + width; x++)
        {
            for (int y = cave.position.y; y < cave.position.y + height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.state != Tile.stFixed)
                {
                    tile.height = 5;
                    tile.ground = (short)GroundType.Type.Mountain;
                    tile.setCliff(true);
                }
            }
        }
        int offset = size/3;
        cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(-offset, -offset));
        cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(-offset, offset));
        cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(offset, offset));
        cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(offset, -offset));
        //cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, -offset));
        //cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(-offset, 0));
        //cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, offset));
        //cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(offset, 0));
    }

    private void generateCave(Cave cave)
    {
        int width = cave.width;
        int height = cave.height;

        int ex, ey, bx, by;
        //Randomizing entrance and boss spawn
        if(width > height)
        {
            ey = Random.Range(cave.MARGIN, height - cave.MARGIN); //somewhere along the y axis
            ex = cave.MARGIN + (width - cave.MARGIN * 2) * Random.Range(0, 2); //either side of the cave

            by = Random.Range(cave.MARGIN, height - cave.MARGIN); //somewhere along y axis
            bx = width - ex; //other side of the cave
        }
        else
        {
            ex = Random.Range(cave.MARGIN, width - cave.MARGIN); //somewhere along the x axis
            ey = cave.MARGIN + (height - cave.MARGIN * 2) * Random.Range(0, 2); //either side of the cave

            bx = Random.Range(cave.MARGIN, width - cave.MARGIN); //somewhere along the x axis
            by = height - ey; //other side of the cave
        
        }
        cave.mainEntrance = new Vector2i(ex, ey) + cave.position;
        cave.bossPos = new Vector2i(bx, by) + cave.position;

        //Debug.Log("Entrance: " + cave.entrancePos.x + ", " + cave.entrancePos.y);
        //Debug.Log("Boss: " + cave.bossPos.x + ", " + cave.bossPos.y);

        //Make a road from entrance to boss
        cave.waypoints.Add(cave.mainEntrance);
        cave.waypoints.Add(cave.bossPos);

        
        setCaveFloor(cave);
        //Set ground type
        for (int x = cave.position.x; x < cave.position.x + width; x++)
        {
            for (int y = cave.position.y; y < cave.position.y + height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.state != Tile.stFixed)
                {
                    tile.height = 5;
                    tile.ground = (short)GroundType.Type.Mountain;
                    tile.setCliff(true);
                }
            }
        }

    }

    private void setCaveFloor(Cave cave)
    {
        int loops = 0;
        for (int w = 0; w < cave.waypoints.Count - 1; w++)
        {
            Vector2i dir = cave.waypoints[w + 1] - cave.waypoints[w];
            int lastx = cave.waypoints[w].x;
            int lasty = cave.waypoints[w].y;
            int totalX = dir.x;
            int totalY = dir.y;
            int absX = Mathf.Abs(totalX);
            int absY = Mathf.Abs(totalY);

            while (absX > 0 || absY > 0)
            {

                if (absX < 0) totalX = 0;
                if (absY < 0) totalY = 0;

                Vector2i center = new Vector2i(lastx, lasty);
                if (loops > 10 && Random.value > 0.99)
                {
                    int radius = 8;
                    int islandRadius = 2;
                    setCircleGround(GroundType.Type.Mountain, center, radius);
                    setCircleHeight(1, center, radius);
                    setCircleHeight(5, center, islandRadius);

                    setCircleFixed(center, radius);
                }
                else
                {
                    int radius = Random.Range(2, 3);
                    setCircleGround(GroundType.Type.Mountain, center, radius);
                    setCircleHeight(1, center, radius);
                    setCircleFixed(center, radius);
                }
                if (totalY == 0)
                {
                    int change = (int)((totalX / Mathf.Abs(totalX)));
                    lastx += change;
                    totalX -= change;
                    absX -= Mathf.Abs(change);
                }
                else if (totalX == 0)
                {
                    int change = (int)((totalY / Mathf.Abs(totalY)));
                    lasty += change;
                    totalY -= change;
                    absY -= Mathf.Abs(change);
                }
                else
                {
                    if (Random.value > 0.5)
                    {
                        int change = (int)((totalX / Mathf.Abs(totalX)));
                        lastx += change;
                        totalX -= change;
                        absX -= Mathf.Abs(change);
                    }
                    else
                    {
                        int change = (int)((totalY / Mathf.Abs(totalY)));
                        lasty += change;
                        totalY -= change;
                        absY -= Mathf.Abs(change);
                    }
                }
                loops++;
                if (loops == 2)
                {
                    cave.mainDoormat = new Vector2((float)lastx, (float)lasty);
                }
            }
        }
        setCircleHeight(1, cave.bossPos, cave.MARGIN - 2);
        setCircleGround(GroundType.Type.Mountain, cave.bossPos, cave.MARGIN - 2);
        setCircleFixed(cave.bossPos, cave.MARGIN - 2);

        if (cave.width > cave.height)
        {
            if(cave.mainEntrance.x < cave.bossPos.x)
                cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(cave.MARGIN - 1.5f, 0));
            else
                cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(-cave.MARGIN + 2.5f, 0));

            cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, cave.MARGIN - 1.5f));
            cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, -cave.MARGIN + 2.5f));
        }
        else
        {
            if (cave.mainEntrance.y < cave.bossPos.y)
                cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, cave.MARGIN - 1.5f));
            else
                cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(0, -cave.MARGIN + 2.5f));

            cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(cave.MARGIN - 1.5f, 0));
            cave.torchPositions.Add(cave.bossPos.toVector2() + new Vector2(-cave.MARGIN + 2.5f, 0));
        }

    }

    public void generateRest()
	{
		for(int x = 0; x < mainMapSize/4; x++)
		{
			for(int y = 0; y < mainMapSize/4; y++)
			{
				if(tiles[x*4, y*4].state != Tile.stFixed)
				{
					tiles[x*4, y*4].height = calculateAvgHeight(x*4, y*4, 4);
				}
			}
		}
		
		for(int x = 0; x < mainMapSize/2; x++)
		{
			for(int y = 0; y < mainMapSize/2; y++)
			{
				if(tiles[x*2, y*2].state != Tile.stFixed)
				{
					tiles[x*2, y*2].height = calculateAvgHeight(x*2, y*2, 2);
				}
			}
		}
		
		for(int x = 0; x < mainMapSize; x++)
		{
			for(int y = 0; y < mainMapSize; y++)
			{
				if(tiles[x, y].state != Tile.stFixed)
				{
					tiles[x, y].height = calculateAvgHeight(x, y, 1);
				}
			}
		}
	}

	public void smoothMap(int scale, int radius)
	{
		for(int x = 0; x < mainMapSize/scale; x++)
		{
			for(int y = 0; y < mainMapSize/scale; y++)
			{
				if(true)
				{
					tiles[x*scale, y*scale].height = calculateAvgHeight(x*scale, y*scale, radius);
				}
			}
		}
	}

	public void smoothGround(int scale, int radius)
	{
		for(int x = 0; x < mainMapSize/scale; x++)
		{
			for(int y = 0; y < mainMapSize/scale; y++)
			{
				Tile t = tiles[x*scale, y*scale];
				if(t.ground < 0) t.ground = calculateAvgGround(x*scale, y*scale, radius);
			}
		}
	}

	public void flattenBaseAndSummons()
	{
		flattenBase(basePos, World.baseSize+2, 5);
		for(int i = 0; i < 4; i++)
		{
			flattenBase(summonPos[i], World.summonSize+2, 5);
		}
	}

	void flattenBase(Vector2i center, int flatRadius, int smoothRadius)
	{
		int radius = flatRadius+smoothRadius;
		short height = getTile(center).height;
		for(int x = center.x-radius; x < center.x + radius; x++)
		{
			for(int y = center.y-radius; y < center.y + radius; y++)
			{
				float dist = Vector2i.getDistance(center, new Vector2i(x, y));
				if(dist <= flatRadius)
				{
					getTile(x, y).height = height;
				}
				else if(dist <= radius)
				{
					Tile tile = getTile(x, y);
					int smoothAmount = (int)((dist-flatRadius)/smoothRadius);
					int change = Mathf.Clamp(height - tile.height, -smoothAmount, smoothAmount);
					tile.height += (short)change;
				}
			}
		}
	}

	void clampMapCenterHeight(int min, int max)
	{
		int radius = (int)(World.summonDistanceFromBase*KEY_RES*1.5f);
		for(int x = basePos.x - radius; x < basePos.x + radius; x++)
		{
			for(int y = basePos.y - radius; y < basePos.y + radius; y++)
			{
				Tile tile = tiles[x, y];
				if(tile.state != Tile.stUnset && Vector2i.getDistance(basePos, new Vector2i(x, y)) < radius)
				{
					tile.height = (short)Mathf.Clamp(tile.height, min, max);
				}

			}
		}
	}

	public void generateDiamond()
	{
		int end = mainMapSize - 1;

		/* 10  	8  	5
		 * 
		 * 4   	5  	1
		 *
		 * 3   	1  	0	
		 *
		 */


		//TOP ROW
		tiles[0, 0].height = 10; //RT
		tiles[end/2, 0].height = 8; //MT
		tiles[end, 0].height = 5; //LT

		//MID ROW
		tiles[0, end/2].height = 4; //RM
		tiles[end/2, end/2].height = 5; //MID
		tiles[end, end/2].height = 1; //LM

		//BOT ROW
		tiles[0, end].height = 3; //RB
		tiles[end/2, end].height = 1; //MB
		tiles[end, end].height = 0; //LB



		divide(end/2, 8, 2, 12);

		generateBase();
		generateSummons();
		clampMapCenterHeight(2, 3);
		generateRoads();

		divide(8, 1, 0, 12);
		clampMapCenterHeight(2, 6);

	}



	void divide(int size, int stop, int minHeight, int maxHeight)
	{
		int half = size/2;
		int scale = (int)(roughness*(size/2 + 4));
		if(half < stop) return;

		for(int y = half; y < mainMapSize-1; y+=size)
		{
			for(int x = half; x < mainMapSize-1; x+=size)
			{
				//if(tiles[x, y].state != Tile.stFixed) square(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale));
				square(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale), minHeight, maxHeight);
			}
		}
		for(int y = 0; y <= mainMapSize-1; y+=half)
		{
			for(int x = (y + half) % size; x <= mainMapSize-1; x+=size)
			{
				//if(tiles[x, y].state != Tile.stFixed)diamond(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale));
				diamond(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale), minHeight, maxHeight);
			}
		}
		divide(size/2, stop, minHeight, maxHeight);
	}

	void diamond(int x, int y, int size, int offset, int minHeight, int maxHeight)
	{
		int ave = average(new short[]{
			getHeight(x, y-size),
	          getHeight(x+size, y),
	          getHeight(x, y+size),
	          getHeight(x-size, y)});
		tiles[x, y].height = (short)Mathf.Clamp(ave + offset, minHeight, maxHeight);
	}

	void square(int x, int y, int size, int offset, int minHeight, int maxHeight)
	{
		int ave = average(new short[]{
			getHeight(x-size, y-size),
			getHeight(x+size, y-size),
          getHeight(x+size, y+size),
          getHeight(x-size, y+size)});
		tiles[x, y].height = (short)Mathf.Clamp(ave + offset, minHeight, maxHeight);
	}

	int average(short[] values)
	{
		int total=0;
		int div=0;
		foreach(short i in values)
		{
			if(i > -9000)
			{
				total+=i;
				div++;
			}
		}

		if(div > 0)
		{
			return total/div;
		}
		return 0;
	}

	int averageGround(short[] values)
	{
//		List<int> v = new List<int>();
//		foreach(short i in values)
//		{
//			if(i != (short)GroundType.Type.Road) v.Add(i);
//		}
//		return v[Random.Range(0, v.Count)];
		int total=0;
		int div=0;
		foreach(short i in values)
		{
			if(i > -1 && i != (short)GroundType.Type.Road)
			{
				total+=i;
				div++;
			}
		}
		float ave = 0;
		if(div > 0)
		{
			ave = (float)total/(float)div;
		}

		int index = 0;
		float diff = Mathf.Abs(values[0] - ave);
		for(int i = 1; i < values.Length; i++)
		{
			float newDiff = Mathf.Abs (values[i] - ave);
			if(newDiff < diff)
			{
				index = i;
				diff = newDiff;
			}
		}

		return values[index];
	}

	public void generateRiver(Vector2i start, Vector2i end, 
	                          int sections, int randomDiameter, 
	                          int minHeight, int maxHeight, int width,
	                          int shallows)
	{
		if(!isValidTile(start) || !isValidTile(end)) return;

		int dx = end.x - start.x;
		int dy = end.y - start.y;

		int xStep = dx/sections;
		int yStep = dy/sections;

		int sectionHeight = Mathf.Min (randomDiameter, Mathf.Abs(xStep));
		int sectionWidth = Mathf.Min (randomDiameter, Mathf.Abs(yStep));

		List<Vector2i> checkPoints = new List<Vector2i>();
		checkPoints.Add(start); //Adding starting point


		//Adding all but last and first points
		for(int i = 1; i < sections; i++)
		{
			int x = start.x + i*xStep + Random.Range(-sectionWidth/2, sectionWidth/2);
			int y = start.y + i*yStep + Random.Range (-sectionHeight/2, sectionHeight/2);

			x = Mathf.Clamp(x, 0 + width, mainMapSize - 1 - width);
			y = Mathf.Clamp(y, 0 + width, mainMapSize - 1 - width);
			checkPoints.Add(new Vector2i(x,y));
		}
		//Adding endpoint
		checkPoints.Add(end);



		for(int w = 0; w < checkPoints.Count - 1; w++)
		{
			Vector2i dir = checkPoints[w+1] - checkPoints[w];
			int lastx = checkPoints[w].x;
			int lasty = checkPoints[w].y;
			int totalX =  dir.x;
			int totalY =  dir.y;
			int absX = Mathf.Abs(totalX);
			int absY = Mathf.Abs(totalY);

			while(absX > 0 || absY > 0)
			{

				if(absX < 0) totalX = 0;
				if(absY < 0) totalY = 0;

				addAreaHeight(-World.tileMap.getTile(lastx,lasty).height - 1, minHeight, maxHeight, new Vector2i(lastx, lasty), width/2);
				//riverSlice(minHeight,maxHeight, new Vector2i(lastx,lasty), lastDir, width/2);
				if(totalY == 0)
				{
					int change = (int)((totalX/Mathf.Abs(totalX)));
					lastx += change;
					totalX-=change;
					absX -= Mathf.Abs(change);
				}
				else if(totalX == 0)
				{
					int change = (int)((totalY/Mathf.Abs(totalY)));
					lasty += change;
					totalY -=change;
					absY -= Mathf.Abs(change);
				}
				else
				{
					if(Random.value > 0.5)
					{
						int change = (int)((totalX/Mathf.Abs(totalX)));
						lastx += change;
						totalX-=change;
						absX -= Mathf.Abs(change);
					}
					else
					{
						int change = (int)((totalY/Mathf.Abs(totalY)));
						lasty += change;
						totalY -=change;
						absY -= Mathf.Abs(change);
					}
				}
			}
		}
		shallows = Mathf.Clamp(shallows, 0, checkPoints.Count - 2);
		for(int i = 1; i < shallows; i++)
		{
			if(i%((int)(checkPoints.Count - 2)/(shallows + 1)) == 0)
			{
				setAreaHeight(0,checkPoints[i], width/2);
			}
		}

	}

	private void setAreaHeight(int height, Vector2i center, int radius)
	{
		for(int x = center.x - radius; x < center.x + radius + 1; x++)
		{
			for(int y = center.y - radius; y < center.y + radius + 1; y++)
			{
				if(isValidTile(x,y)) tiles[x,y].height = (short)height;
			}
		}
	}

    private void setCircleHeight(int height, Vector2i center, int radius)
    {
        for (int x = center.x - radius; x < center.x + radius + 1; x++)
        {
            for (int y = center.y - radius; y < center.y + radius + 1; y++)
            {
                if(Vector2i.getDistance(new Vector2i(x, y), center) <= radius && isValidTile(x, y))
                    tiles[x, y].height = (short)height;
            }
        }
    }

    private void setAreaGround(GroundType.Type ground, Vector2i center, int radius)
    {
        for (int x = center.x - radius; x < center.x + radius + 1; x++)
        {
            for (int y = center.y - radius; y < center.y + radius + 1; y++)
            {
                if (isValidTile(x, y)) tiles[x, y].ground = (short)ground;
            }
        }
    }

    private void setCircleGround(GroundType.Type ground, Vector2i center, int radius)
    {
        for (int x = center.x - radius; x < center.x + radius + 1; x++)
        {
            for (int y = center.y - radius; y < center.y + radius + 1; y++)
            {
                if (Vector2i.getDistance(new Vector2i(x, y), center) <= radius && isValidTile(x, y))
                    tiles[x, y].ground = (short)ground;
            }
        }
    }

    private void setAreaFixed(Vector2i center, int radius)
    {
        for (int x = center.x - radius; x < center.x + radius + 1; x++)
        {
            for (int y = center.y - radius; y < center.y + radius + 1; y++)
            {
                if (isValidTile(x, y)) tiles[x, y].state = Tile.stFixed;
            }
        }
    }

    private void setCircleFixed(Vector2i center, int radius)
    {
        for (int x = center.x - radius; x < center.x + radius + 1; x++)
        {
            for (int y = center.y - radius; y < center.y + radius + 1; y++)
            {
                if (Vector2i.getDistance(new Vector2i(x, y), center) <= radius && isValidTile(x, y))
                    tiles[x, y].state = Tile.stFixed;
            }
        }
    }

	private void riverSlice(int minHeight, int maxHeight, Vector2i center, Vector2i direction, int radius)
	{
		int dx = direction.x; 
		int dy = direction.y;

		if(dx == 0) // only do in y-dir
		{
			int x = center.x;
			for(int y = 0; y < 2*radius + 1; y++)
			{
				int height = minHeight + (int)((maxHeight - minHeight)*Mathf.Sin(Mathf.PI * y / (2 * radius)));
				if(isValidTile(x,y + center.y - radius)) tiles[x, y + center.y - radius].height = (short)height;
				
			}
		}
		else if(dy == 0) //only do in x-dir
		{
			int y = center.y;
			for(int x = 0; x < 2*radius + 1; x++)
			{
				int height = minHeight + (int)((maxHeight - minHeight)*Mathf.Sin(Mathf.PI * x / (2 * radius)));
				if(isValidTile(x + center.x - radius,y)) tiles[x + center.x - radius, y].height = (short)height;
				
			}
		}
	}

	private void addAreaHeight(int height, int minHeight, int maxHeight, Vector2i center, int radius)
	{
		for(int x = center.x - radius; x < center.x + radius + 1; x++)
		{
			for(int y = center.y - radius; y < center.y + radius + 1; y++)
			{
				if(isValidTile(x,y)) 
				{   
					int DistanceToCenter = (int)Mathf.Sqrt((x-center.x)*(x-center.x) + (y-center.y)*(y-center.y));
					if(DistanceToCenter > radius) continue;
					tiles[x,y].height += (short)height;
					tiles[x,y].height = (short)Mathf.Clamp(tiles[x,y].height, minHeight, maxHeight);
				}
			}
		}
	}

	public void generateGround()
	{
		tiles[0, 0].ground = 1;
		tiles[mainMapSize-1, 0].ground = 1;
		tiles[mainMapSize-1, mainMapSize-1].ground = 2;
		tiles[0, mainMapSize-1].ground = 1;
		tiles[mainMapSize/2, mainMapSize/2].ground = 0;
		
		divideGround(mainMapSize-1, 1, 0, 2);
		//smoothGround(1,4);
		setGroundTiles();
	}

	void divideGround(int size, int stop, int minHeight, int maxHeight)
	{
		int half = size/2;
		int scale = (int)(groundRoughness*size);
		if(half < stop) return;
		
		for(int y = half; y < mainMapSize-1; y+=size)
		{
			for(int x = half; x < mainMapSize-1; x+=size)
			{
				squareGround(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale), minHeight, maxHeight);
			}
		}
		for(int y = 0; y <= mainMapSize-1; y+=half)
		{
			for(int x = (y + half) % size; x <= mainMapSize-1; x+=size)
			{
				diamondGround(x, y, half, Mathf.RoundToInt(Random.value * scale * 2 - scale), minHeight, maxHeight);
			}
		}
		divideGround(size/2, stop, minHeight, maxHeight);
	}

	void setGroundTiles()
	{
		for(int x = 0; x < mainMapSize; x++)
		{
			for(int y = 0; y < mainMapSize; y++)
			{
				if(tiles[x, y].height <= World.WATER_LEVEL)
				{
					setWaterTile(x, y);
				}
				else if(tiles[x,y].height >= World.SNOW_LEVEL)
				{
					tiles[x,y].ground = (short)GroundType.Type.SnowMountain;
				}
				else if(tiles[x,y].height >= World.MOUNTAIN_LEVEL)
				{
					tiles[x,y].ground = (short)GroundType.Type.Mountain;
				}

			}
		}
	}

	void setWaterTile(int x, int y)
	{
		for(int xx = x - 1; xx < x+2; xx++)
		{
			for(int yy = y - 1; yy < y+2; yy++)
			{
				if(!isValidTile(xx, yy))
				{
					continue;
				}
				Tile tile = tiles[xx, yy];
				if(tile.height == 1)
				{
					tile.ground = (short)GroundType.Type.Beach;
				}
				else if(tile.height <= World.WATER_LEVEL)
				{
					if(hasLandNeighbor(xx, yy))
					{
						tile.ground = (short)GroundType.Type.Shore;
					}
					else
					{
						tile.ground = (short)GroundType.Type.Water;
					}
				}
			}
		}
	}

	bool hasWaterNeighbor(int x, int y)
	{
		for(int xx = x - 1; xx < x+2; xx++)
		{
			for(int yy = y - 1; yy < y+2; yy++)
			{
				if((xx != x && yy != y) && isValidTile(xx, yy) && tiles[xx, yy].height <= World.WATER_LEVEL)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool hasLandNeighbor(int x, int y)
	{
		for(int xx = x - 1; xx < x+2; xx++)
		{
			for(int yy = y - 1; yy < y+2; yy++)
			{
				if((xx != x && yy != y) && isValidTile(xx, yy) && tiles[xx, yy].height > World.WATER_LEVEL)
				{
					return true;
				}
			}
		}
		return false;
	}

	void diamondGround(int x, int y, int size, int offset, int minHeight, int maxHeight)
	{
		if(tiles[x, y].ground >= 0) return;
		int ave = averageGround(new short[]{
			getGround(x, y-size),
			getGround(x+size, y),
			getGround(x, y+size),
			getGround(x-size, y)});
		tiles[x, y].ground = (short)Mathf.Clamp(ave + offset, minHeight, maxHeight);
	}

	void squareGround(int x, int y, int size, int offset, int minHeight, int maxHeight)
	{
		if(tiles[x, y].ground >= 0) return;
		int ave = averageGround(new short[]{
			getGround(x-size, y-size),
			getGround(x+size, y-size),
			getGround(x+size, y+size),
			getGround(x-size, y+size)});
		tiles[x, y].ground = (short)Mathf.Clamp(ave + offset, minHeight, maxHeight);
	}

	public short getGround(int x, int y)
	{
		if(isValidTile(x, y))
		{
			return tiles[x, y].ground;
		}
		return -9000;
	}

	public short getHeight(int x,int y)
	{
		if(isValidTile(x, y))
		{
			return tiles[x, y].height;
		}
		return -9000;
	}
	
	short calculateAvgHeight(int x, int y, int r)
	{
		int total = 0;
		int div = 0;
		for(int i = x-r; i < x+r+1; i+=r)
		{
			for(int j = y-r; j < y+r+1; j+=r)
			{
				if(i > 0 && i < mainMapSize && j > 0 && j < mainMapSize)
				{
					if(tiles[i, j].state != Tile.stUnset)
					{
						int h = tiles[i, j].height;
						total+= h;
						div++;
					}
					
				}
			}
		}
		return (short)((float)total/(float)div);
	}

	short calculateAvgGround(int x, int y, int r)
	{
		int total = 0;
		int div = 0;
		for(int i = x-r; i < x+r+1; i+=r)
		{
			for(int j = y-r; j < y+r+1; j+=r)
			{
				if(i > 0 && i < mainMapSize && j > 0 && j < mainMapSize)
				{
					if(tiles[i, j].ground >= 0 && tiles[i, j].ground != (short)GroundType.Type.Road)
					{
						int g = tiles[i, j].ground;
						total+= g;
						div++;
					}
					
				}
			}
		}
		return (short)((float)total/(float)div);
	}

	Vector2i keyHeightBounds(Vector2i pos)
	{
		int size = mainMapSize/KEY_RES - 1;
		if(pos.x < 1)
		{
			pos.x = 1;
		}
		else if(pos.x > size)
		{
			pos.x = size;
		}
		if(pos.y < 1)
		{
			pos.y = 1;
		}
		else if(pos.y > size)
		{
			pos.y = size;
		}
		return pos;
	}

	public Tile getTile(int x, int y)
	{
		return tiles[x, y];
	}

	public Tile getTile(Vector2i pos)
	{
		return tiles[pos.x, pos.y];
	}

	public bool isValidTile(int x, int y)
	{
		return (x > -1 && x < mapSize && y > -1 && y < mapSize); 
	}

	public bool isValidTile(Vector2i pos)
	{
		return (pos.x > -1 && pos.x < mapSize && pos.y > -1 && pos.y < mapSize); 
	}

	public Vector3 getTilePos(Vector2i pos)
	{
		return new Vector3(pos.x, getHeight(pos.x, pos.y), pos.y);
	}

	public Vector3 getTilePos(int x, int y)
	{
		return new Vector3(x, getHeight(x, y), y);
	}

	public int getSize()
	{
		return mapSize;
	}

    public int getMainMapSize()
    {
        return mainMapSize - 1;
    }
	public void updateTileTypeCount()
	{
		grassTiles = 0;
		roadTiles = 0;
		for(int x = 0; x < mainMapSize; x++)
		{
			for(int y = 0; y < mainMapSize; y++)
			{
				if(tiles[x,y].state == Tile.stFixed)
					roadTiles++;
				else
					grassTiles++;
			}
		}
	}

    public List<Cave> getCaves()
    {
        return caves;
    }

    public Cave getCave(int index)
    {
        return caves[index];
    }

    public Vector2i getCaveEntrance(int index)
    {
        return caves[index].mainEntrance;
    }
}
