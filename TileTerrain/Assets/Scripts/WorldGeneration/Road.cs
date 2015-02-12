using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road {
	Line[] bounds;
	List<WorldPoint> waypoints;
	Color color;

	public Road(WorldPoint start, WorldPoint end, Line[] bounds, Color color)
	{
		this.bounds = bounds;
		waypoints = new List<WorldPoint>();
		waypoints.Add(start);
		waypoints.Add (end);
		this.color = color;
	}

	public void subdivide()
	{
		for(int i = 0; i < waypoints.Count-1; i+=2)
		{
			Vector2i cur = waypoints[i].get2D();
			Vector2i next = waypoints[i+1].get2D();
			float angle = Vector2i.getAngle(cur, next) + Random.Range(-45, 45);

			float dist = Vector2i.getDistance(cur, next)/2;
			int height = Mathf.FloorToInt((float)(waypoints[i].height + waypoints[i+1].height)/2.0f);
			WorldPoint newpoint = new WorldPoint(Mathf.RoundToInt(Mathf.RoundToInt(Mathf.Cos(Mathf.Deg2Rad*angle)*dist)),
			                                     Mathf.RoundToInt(Mathf.Sin (Mathf.Deg2Rad*angle)*dist), height) + cur;
			int counter = 0;
			Line line = new Line(next, newpoint.get2D());
			while(Line.intersects(bounds[0], line))
			{
				line.draw(Color.black);
				counter++;
				if(counter > 20) break;
				angle-= 5;
				Debug.Log ("minus 10");
				newpoint = new WorldPoint(Mathf.RoundToInt(Mathf.RoundToInt(Mathf.Cos(Mathf.Deg2Rad*angle)*dist)),
				                                      Mathf.RoundToInt(Mathf.Sin (Mathf.Deg2Rad*angle)*dist), height) + cur;
				line = new Line(next, newpoint.get2D());
			}
			counter = 0;
			while(Line.intersects(bounds[1], line))
			{
				line.draw(Color.black);
				counter++;
				if(counter > 20) break;
				angle += 5;
				Debug.Log ("plus 10");
				newpoint = new WorldPoint(Mathf.RoundToInt(Mathf.RoundToInt(Mathf.Cos(Mathf.Deg2Rad*angle)*dist)),
				                                         Mathf.RoundToInt(Mathf.Sin (Mathf.Deg2Rad*angle)*dist), height) + cur;
				line = new Line(next, newpoint.get2D());
			}




			waypoints.Insert(i+1, newpoint);
		}

	}

	public void draw()
	{
		for(int i = 0; i < waypoints.Count-1; i++)
		{
			Debug.DrawLine(convert(waypoints[i]), convert(waypoints[i+1]), color);
		}
		bounds[0].draw(Color.blue);
		bounds[1].draw(Color.red);
	}

	public static Vector3 getHeight0(Vector3 v)
	{
		return new Vector3(v.x, 0, v.z);
	}

	public static Vector3 convert(WorldPoint point)
	{
		return new Vector3(point.x*4, point.height*2, point.y*4);
	}

	public static Vector2 getV2(Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}
	public List<WorldPoint> getWaypoints()
	{
		return waypoints;
	}

}
