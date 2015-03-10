using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Cave  {

	public readonly int MARGIN = 10;
    public readonly Vector2i position;
    public readonly int width, height;

	private Vector2i entrancePosition;
	private Vector2i bossPosition;

    public List<Vector2i> waypoints;

	public Cave(Rect rect)
	{
		this.position = new Vector2i((int)rect.x, (int)rect.y);
        this.width = (int)rect.width;
        this.height = (int)rect.height;
        waypoints = new List<Vector2i>();
	}

	

    public Vector2i entrancePos
    {
        get{return entrancePosition;}
        set{entrancePosition = value;}
    }

    public Vector2i bossPos
    {
        get{return bossPosition;}
        set{bossPosition = value;}
    }

}
