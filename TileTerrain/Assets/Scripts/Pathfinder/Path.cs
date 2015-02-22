using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {

	private List<Vector2> checkPoints;

	public Path()
	{
		checkPoints = new List<Vector2>();
	}
	public Path(Vector2[] points)
	{
		checkPoints = new List<Vector2>();
		foreach(Vector2 point in points)
		{
			checkPoints.Add(point);
		}
	}
	public void addCheckPoint(Vector2 checkPoint)
	{
		checkPoints.Insert (0, checkPoint);
	}

	public void addCheckPoint(int index, Vector2 checkPoint)
	{
		checkPoints.Insert (index, checkPoint);
	}

	public Vector2 popCheckPoint()
	{
		Vector2 point = checkPoints[0];
		checkPoints.RemoveAt(0);
		return point;
	}

	public Vector2 getDestination()
	{
		return checkPoints[checkPoints.Count - 1];
	}

	public int getCheckPointCount()
	{
		return checkPoints.Count;
	}

    public List<Vector2> getCheckPoints()
    {
        return checkPoints;
    }

	public Vector2 getPoint(int index)
	{
		return checkPoints[index];
	}

	public void setPoint(int index, Vector2 point)
	{
		checkPoints[index] = point;
	}

	public void addToPoint(int index, Vector2 add)
	{
		checkPoints[index] += add;
	}

	public void removePoint(int index)
	{
		checkPoints.RemoveAt(index);
	}
	


}
