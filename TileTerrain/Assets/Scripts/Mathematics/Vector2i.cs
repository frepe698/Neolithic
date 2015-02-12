using UnityEngine;
using System.Collections;

public class Vector2i {

	public int x, y;

	public Vector2i(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2i(Vector2 v)
	{
		this.x = Mathf.FloorToInt(v.x);
		this.y = Mathf.FloorToInt(v.y);
	}

	public Vector2i(Vector3 v)
	{
		this.x = Mathf.FloorToInt(v.x);
		this.y = Mathf.FloorToInt(v.z);
	}

	public Vector2i(float x, float y)
	{
		this.x = Mathf.FloorToInt(x);
		this.y = Mathf.FloorToInt(y);
	}

	public Vector2i()
	{
		this.x = 0;
		this.y = 0;
	}

	public static float getAngle(Vector2i start, Vector2i end)
	{
		float dx = end.x - start.x;
		float dy = end.y - start.y;
		float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
		
		return angle;
	}

	public static float getDistance(Vector2i v1, Vector2i v2)
	{
		return Mathf.Sqrt((v1.x - v2.x)*(v1.x - v2.x) + (v1.y- v2.y)*(v1.y - v2.y));
	}

	public static int getManhattan(Vector2i v1, Vector2i v2)
	{
		return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y);
	}

	public static int getLongestAxis(Vector2i v1, Vector2i v2)
	{
		return Mathf.Max(Mathf.Abs(v1.x - v2.x), Mathf.Abs(v1.y - v2.y));
	}

	public static Vector2i operator+ (Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x + v2.x, v1.y + v2.y);
	}

	public static Vector2i operator- (Vector2i v1, Vector2i v2)
	{
		return new Vector2i(v1.x - v2.x, v1.y - v2.y);
	}

	public static Vector2i operator* (Vector2i v1, float mult)
	{
		return new Vector2i((int)(v1.x * mult), (int)(v1.y * mult));
	}

	public static Vector2i operator/ (Vector2i v1, float div)
	{
		return new Vector2i((int)(v1.x / div), (int)(v1.y / div));
	}

	public static bool operator== (Vector2i v1, Vector2i v2)
	{
		return (v1.x == v2.x && v1.y == v2.y);
	}

	public static bool operator!= (Vector2i v1, Vector2i v2)
	{
		return !(v1.x == v2.x && v1.y == v2.y);
	}

	public override bool Equals(object v)
	{
		if(v is Vector2i)
		{
			return this == (Vector2i)v;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x*10000000 + y;
	}

}
