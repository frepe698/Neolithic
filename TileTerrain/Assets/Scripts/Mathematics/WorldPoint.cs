using UnityEngine;
using System.Collections;

public class WorldPoint {

	public int x, y, height;

	public WorldPoint(int x, int y, int height)
	{
		this.x = x;
		this.y = y;
		this.height = height;
	}

	public WorldPoint(Vector2i v, int height)
	{
		this.x = v.x;
		this.y = v.y;
		this.height = height;
	}

	public WorldPoint()
	{
		this.x = 0;
		this.y = 0;
		this.height = 0;
	}

    public WorldPoint getClone()
    {
        return new WorldPoint(x, y, height);
    }

	public static float get2DAngle(WorldPoint start, WorldPoint end)
	{
		float dx = end.x - start.x;
		float dy = end.y - start.y;
		float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
		
		return angle;
	}

	public Vector2i get2D()
	{
		return new Vector2i(x, y);
	}

	public static WorldPoint operator+ (WorldPoint v1, WorldPoint v2)
	{
		return new WorldPoint(v1.x + v2.x, v1.y + v2.y, v1.height + v2.height);
	}

	public static WorldPoint operator+ (WorldPoint v1, Vector2i v2)
	{
		return new WorldPoint(v1.x + v2.x, v1.y + v2.y, v1.height);
	}
	
	public static WorldPoint operator- (WorldPoint v1, WorldPoint v2)
	{
		return new WorldPoint(v1.x - v2.x, v1.y - v2.y, v1.height - v2.height);
	}
	
	public static WorldPoint operator* (WorldPoint v1, float mult)
	{
		return new WorldPoint((int)(v1.x * mult), (int)(v1.y * mult), (int)(v1.height * mult));
	}
	
	public static WorldPoint operator/ (WorldPoint v1, float div)
	{
		return new WorldPoint((int)(v1.x / div), (int)(v1.y / div), (int)(v1.height / div));
	}
	
	public static bool operator== (WorldPoint v1, WorldPoint v2)
	{
		return (v1.x == v2.x && v1.y == v2.y && v1.height == v2.height);
	}
	
	public static bool operator!= (WorldPoint v1, WorldPoint v2)
	{
		return !(v1.x == v2.x && v1.y == v2.y && v1.height == v2.height);
	}
	
	public override bool Equals(object v)
	{
		if(v is WorldPoint)
		{
			return this == (WorldPoint)v;
		}
		return false;
	}
	
	public override int GetHashCode()
	{
		return x*100000000 + y*10000000 + height;
	}
}
