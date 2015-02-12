using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode{

	public Vector2 pos;
	public Vector2i tile;
	public int gCost = 0;
	public int hCost = 0;
	private PathNode parentNode;

	public PathNode(Vector2i position)
	{
		tile = position;
		this.pos = new Vector2(position.x + 0.5f, position.y + 0.5f);
	}

	public PathNode(Vector2i position, PathNode parent)
	{
		tile = position;
		this.pos = new Vector2(position.x + 0.5f, position.y + 0.5f);
		parentNode = parent;
	}

	public PathNode(Vector2 pos)
	{
		this.pos = pos;
		this.tile = new Vector2i(pos);
	}

	public PathNode(Vector2 pos, PathNode parent)
	{
		this.pos = pos;
		this.tile = new Vector2i(pos);
		parentNode = parent;
	}

	public PathNode getParent()
	{
		return parentNode;
	}

	public void setParent(PathNode parent)
	{
		parentNode = parent;
	}

	public int getFCost()
	{
		return gCost + hCost;
	}

}
