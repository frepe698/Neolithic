using UnityEngine;
using System.Collections;

public class TileInfo {

	private Vector2i tile;
	private float distanceToUnit;
	private Vector2 point;

	public TileInfo(Vector2i tile, Vector2 unitPoint, Vector2 point)
	{
		this.tile = tile;
		this.distanceToUnit = Vector2.Distance(unitPoint, point);
		this.point = point;

		Debug.DrawLine(new Vector3(tile.x + 0.5f, 1, tile.y + 0.5f), new Vector3(point.x, 1, point.y), Color.white, 4);
	}

	public float getDistance()
	{
		return distanceToUnit;
	}

	public Vector2 getPoint()
	{
		return point;
	}

	public Vector2i getTile()
	{
		return tile;
	}

}

