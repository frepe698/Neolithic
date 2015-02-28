﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/* Every tile can have at most one static object. If there is an object on a tile it is 
 * not possible to walk there. 
 * 
 * If the player clicks a tile that is not walkable the algorithm first checks for the closest walkable tile
 * to the click. If there is no adjacent tile available, the tile first walkable tile along the bird path to the player from the click is chosen as
 * destination
 */

public class Pathfinder {

	private static readonly int NOT_WORTH_IT = 1000;
    private static readonly int RESOLUTION = 15; //number of spline-samples between every pair of checkpoints.

	private static readonly Vector2i[] neighborCoordinates = new Vector2i[]
	{
		new Vector2i(-1, 0),
		new Vector2i(0, -1),
		new Vector2i(1, 0),
		new Vector2i(0, 1)
	};

	public static Path findPath(TileMap map, Vector2 start, Vector2 end, int unitID)
	{
		if(!map.getTile(new Vector2i(end)).isWalkable(unitID)) return new Path();
		if(unhinderedTilePath(map, start,end, unitID)) return new Path(new Vector2[]{end});

		return aStarPathFinding(map, start, end, unitID); 

	}

	public static Path aStarPathFinding(TileMap map, Vector2 start, Vector2 end, int unitID)
	{
		Vector2i endTile = new Vector2i(end);
		List<PathNode> closedSet = new List<PathNode>();
		List<PathNode> openSet = new List<PathNode>();

		PathNode startNode = new PathNode(start);
		int worthCounter = 0;

		startNode.hCost = Vector2i.getManhattan(startNode.tile, endTile);
		openSet = addSorted(openSet,startNode);

		PathNode closestNode = startNode;
		
		while(openSet.Count > 0)
		{
			PathNode current = openSet[0];
			if(current.tile == endTile){
				current.pos = end;
				return createPath(current, unitID);
			}
			if(current.hCost < closestNode.hCost) closestNode = current;
			worthCounter++;
			if(worthCounter >= NOT_WORTH_IT) 
			{
				//Debug.Log ("Not worth it");
				return aStarPathFinding(map, start, closestNode.pos, unitID);
			}
			openSet = removeItem(openSet, current);
			closedSet.Add(current);

			// Add neighbors
			for(int i = 0; i < 4; i++)
			{
				int x = current.tile.x + neighborCoordinates[i].x;
				int y = current.tile.y + neighborCoordinates[i].y;
				if((map.isValidTile(x,y) && map.getTile(x, y).isWalkable(unitID)))
				{

					PathNode neighbor = new PathNode(new Vector2(x + 0.5f, y + 0.5f), current);
					if(containsNode(closedSet, neighbor) || Vector2i.getManhattan(current.tile, neighbor.tile) > 1)
					{
						continue;
					}
					int gCost = current.gCost + Mathf.Min(Vector2i.getManhattan(current.tile, neighbor.tile)*10, 14);
					int hCost = Vector2i.getManhattan(endTile, neighbor.tile);
					
					int nodeIndex = getNodeIndex(openSet, neighbor);
					if(nodeIndex == -1)
					{
						neighbor.gCost = gCost;
						neighbor.hCost = hCost;
						openSet = addSorted(openSet, neighbor);
					}
					else
					{
						if(openSet[nodeIndex].gCost > gCost)
						{
							openSet.RemoveAt(nodeIndex);
							neighbor.gCost = gCost;
							neighbor.hCost = hCost;
							openSet = addSorted(openSet, neighbor);
						}
					}
				}
			}
		}
		return new Path();
	}

	public static Path createPath(PathNode goal, int unitID)
	{
		Path path = new Path();
		PathNode currentNode = goal;
		while(currentNode.getParent() != null)
		{
			path.addCheckPoint(currentNode.pos);
			currentNode = currentNode.getParent();
		}
		path.addCheckPoint(currentNode.pos);
		

        //If path only contains 2 or less points we dont have to spline.
        path = smoothPath(path, unitID);
        if (path.getCheckPointCount() <= 2)
            return path;

        //get arguments for splinefunction
        List<Vector2> checkPoints = path.getCheckPoints();
        int numberOfCheckpoints = path.getCheckPointCount();
        float[] x = new float[numberOfCheckpoints];
        float[] y = new float[numberOfCheckpoints];
        float[] xs = new float[(numberOfCheckpoints - 1) * RESOLUTION];
        float[] ys = new float[(numberOfCheckpoints - 1) * RESOLUTION];
        int i = 0;
        foreach(Vector2 checkPoint in checkPoints)
        {
            x[i] = checkPoint.x;
            y[i] = checkPoint.y;
            i++;
        }

        //get spline and sample it with (numberOfCheckpoints - 1) * RESOLUTION samples.
        CubicSpline.FitGeometric(x, y, (numberOfCheckpoints - 1) * RESOLUTION, out xs, out ys);

        //add sampled values as checkpoints.
        path = new Path();
        for (int j = (numberOfCheckpoints - 1) * RESOLUTION - 1; j >= 0; j--)
        {
            path.addCheckPoint(new Vector2(xs[j],ys[j]));
        }

        return path;
        
	}

	private static Path smoothPath(Path path, int unitID)
	{
		if(path.getCheckPointCount() < 3) return path;
		//Remove v's
		float add = 0.3f;
//		for(int i = 0; i < path.getCheckPointCount()-2; i++)
//		{
//
//			Vector2i p1 = new Vector2i(path.getPoint(i));
//			Vector2i p2 = new Vector2i(path.getPoint(i+1));
//			Vector2i p3 = new Vector2i(path.getPoint(i+2));
//
//			if((p1.x == p3.x && p1.x != p2.x))
//			{
//				int dx = p1.x - p2.x;
//				int dy = p1.y - p2.y;
//
//				path.addToPoint(i+1, new Vector2(dx * add, dy*add));
//				path.addCheckPoint(i+2, new Vector2(0, dy*-add*2)+path.getPoint(i+1));
//			}
//			else if(p1.y == p3.y && p1.y != p2.y)
//			{
//				int dx = p1.x - p2.x;
//				int dy = p1.y - p2.y;
//				
//				path.addToPoint(i+1, new Vector2(dx * add, dy*add));
//				path.addCheckPoint(i+2, new Vector2(dx*-add*2, 0)+path.getPoint(i+1));
//			}
//		}
		//remove L's

		for(int i = 0; i < path.getCheckPointCount()-2; i++)
		{
			
			Vector2i p1 = new Vector2i(path.getPoint(i));
			Vector2i p2 = new Vector2i(path.getPoint(i+1));
			Vector2i p3 = new Vector2i(path.getPoint(i+2));
			
			if((p1.x != p3.x && p1.y != p3.y))
			{
				if(unhinderedTilePath(World.getMap(), path.getPoint(i), path.getPoint(i+2), unitID))
				{
					path.removePoint(i+1);
					continue;

				}
				float dx = p1.x - p3.x;
				if(p1.x == p2.x)
				{
					dx = p3.x - p1.x;
				}

				float dy = p1.y - p3.y;
				if(p1.y == p2.y)
				{
					dy = p3.y - p1.y;
				}
				path.addToPoint(i+1, new Vector2(dx * add, dy*add));
			}

		}

		for(int i = 0; i < path.getCheckPointCount()-2; i++)
		{
			if(path.getCheckPointCount() < 3) goto Done;
			if(unhinderedTilePath(World.getMap(), path.getPoint(i), path.getPoint(i+2), unitID))
			{
				path.removePoint(i+1);
				i--;
			}
		}
		//TODO: kan va lite buggig :)
		for(int i = path.getCheckPointCount()-1; i >= 2; i--)
		{
			if(path.getCheckPointCount() < 3) goto Done;
			if(unhinderedTilePath(World.getMap(), path.getPoint(i), path.getPoint(i-2), unitID))
			{
				path.removePoint(i-1);
			}
		}
	Done:
		return path;
	}
	
	public static bool unhinderedTilePath(TileMap map, Vector2 start, Vector2 end, int unitID)
	{
		Vector2i startTile = new Vector2i(start);
		Vector2i endTile = new Vector2i(end); 
		
		int dx = endTile.x - startTile.x;
		int dy = endTile.y - startTile.y;
		int d = Mathf.Abs(dx) + Mathf.Abs(dy);
		float xo = (float)dx/(float)d;
		float yo = (float)dy/(float)d;
		
		for(int i = 0; i < d; i++)
		{
			float xoff = xo*i;
			float yoff = yo*i;
			
			float xdec = xoff - (int)xoff;
			float ydec = yoff - (int)yoff;
			bool xcenter = ( (xdec < 0.55f && xdec > 0.45f) || (xdec > -0.55f && xdec < -0.45f) );
			bool ycenter = ( (ydec < 0.55f && ydec > 0.45f) || (ydec > -0.55f && ydec < -0.45f) );
			if(xcenter || ycenter)
			{
				int floorx = startTile.x + Mathf.FloorToInt(xo*i);
				int ceilx = startTile.x + Mathf.CeilToInt(xo*i);
				int floory = startTile.y + Mathf.FloorToInt(yo*i);
				int ceily = startTile.y + Mathf.CeilToInt(yo*i);
                if ((map.isValidTile(floorx, ceily) && !map.getTile(floorx, ceily).isWalkable(unitID)) ||
                   (map.isValidTile(ceilx, floory) && !map.getTile(ceilx, floory).isWalkable(unitID)) ||
                   (map.isValidTile(ceilx, ceily) && !map.getTile(ceilx, ceily).isWalkable(unitID)) ||
                   (map.isValidTile(floorx, floory) && !map.getTile(floorx, floory).isWalkable(unitID)))
                {
                    return false;

                }
			}
//			else if(xcenter)
//			{
//				Debug.Log("x center");
//				if(!map.getTile(startTile.x + Mathf.FloorToInt(xo*i), startTile.y + Mathf.RoundToInt(yo*i)).isWalkable() ||
//				   !map.getTile(startTile.x + Mathf.CeilToInt(xo*i), startTile.y + Mathf.RoundToInt(yo*i)).isWalkable()) return false;
//			}
//			else if(ycenter)
//			{
//				Debug.Log("y center");
//				if(!map.getTile(startTile.x + Mathf.RoundToInt(xo*i), startTile.y + Mathf.CeilToInt(yo*i)).isWalkable() ||
//				   !map.getTile(startTile.x + Mathf.RoundToInt(xo*i), startTile.y + Mathf.FloorToInt(yo*i)).isWalkable()) return false;
//			}
			else if(!map.getTile(startTile.x + Mathf.RoundToInt(xo*i), startTile.y + Mathf.RoundToInt(yo*i)).isWalkable(unitID))
			{

				return false;
			}
		}

		return true;
	}

	public static bool getClosestWalkablePoint(TileMap map, Vector2 clickPos, Vector2 startPos, out Vector2 result, int unitID)
	{
		Vector2i clickedTile = new Vector2i(clickPos);
		if(map.getTile(clickedTile).isWalkable(unitID))
		{
			result = clickPos;
			return true;
		}
		Vector2 newResult = new Vector2();
		if(getWalkableNeighbor(map, clickPos, clickedTile, out newResult, unitID, startPos))
		{
			result = newResult;
			return true;
		}
		result = new Vector2();
		return false;
	}

	private static bool getWalkableNeighbor(TileMap map, Vector2 clickPos, Vector2i clickedTile, out Vector2 result, int unitID, Vector2 unitPos)
	{
		List<TileInfo> neighbors = new List<TileInfo>();
		float margin = 0.15f;
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x - 1, clickedTile.y - 1), unitPos, new Vector2(clickedTile.x-margin, clickedTile.y-margin)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x    , clickedTile.y - 1), unitPos, new Vector2(clickPos.x, clickedTile.y-margin)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x + 1, clickedTile.y - 1), unitPos, new Vector2(clickedTile.x + 1 + margin, clickedTile.y-margin)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x - 1, clickedTile.y    ), unitPos, new Vector2(clickedTile.x-margin, clickPos.y)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x + 1, clickedTile.y    ), unitPos, new Vector2(clickedTile.x + 1 + margin, clickPos.y)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x - 1, clickedTile.y + 1), unitPos, new Vector2(clickedTile.x - margin, clickedTile.y + 1 + margin)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x    , clickedTile.y + 1), unitPos, new Vector2(clickPos.x, clickedTile.y + 1 + margin)));
		neighbors = addSorted(neighbors, new TileInfo(new Vector2i(clickedTile.x + 1, clickedTile.y + 1), unitPos, new Vector2(clickedTile.x + 1+margin, clickedTile.y + 1 + margin)));
		for(int i = 0; i < neighbors.Count; i++)
		{
			TileInfo info = neighbors[i];
			//Debug.Log ("Tile clicked: " + clickedTile.x + ", " + clickedTile.y + 
			//           "Checked Tile: " + info.getTile().x + ", " + info.getTile().y);

			if(map.isValidTile(info.getTile()) && map.getTile(info.getTile()).isWalkable(unitID))
			{
				result = info.getPoint();
				return true;
			}
		}
		result = new Vector2();
		return false;
	}

	private static List<TileInfo> addSorted(List<TileInfo> list, TileInfo item)
	{
		for(int i = 0; i < list.Count; i++)
		{
			if(list[i].getDistance() > item.getDistance()) 
			{
				list.Insert(i,item);
				return list;
			}
		}
		list.Add(item);
		return list;
	}

	private static List<PathNode> addSorted(List<PathNode> list, PathNode item)
	{
		for(int i = 0; i < list.Count; i++)
		{
			if(list[i].getFCost() > item.getFCost()) 
			{
				list.Insert(i,item);
				return list;
			}
		}
		list.Add(item);
		return list;
	}

	private static List<PathNode> removeItem(List<PathNode> list, PathNode item)
	{
		for(int i = 0; i < list.Count; i++)
		{
			if(list[i].tile == item.tile)
			{
				list.RemoveAt(i);
				return list;
			}
		}
		return list;
	}

	private static int getNodeIndex(List<PathNode> list, PathNode item)
	{
		for(int i = 0; i < list.Count; i++)
		{
			if(list[i].tile == item.tile) return i;
		}
		return -1;
	}

	private static bool containsNode(List<PathNode> list, PathNode item)
	{
		foreach(PathNode node in list)
		{
			if(node.tile == item.tile) return true;
		}
		return false;
	}


}
