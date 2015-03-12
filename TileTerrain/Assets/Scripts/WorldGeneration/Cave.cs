using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Cave  {

	public readonly int MARGIN = 10;
    public readonly Vector2i position;
    public readonly int width, height;

	private Vector2i entrancePosition;
    public Vector2 doorMatPosition;
	private Vector2i bossPosition;

    public List<Vector2i> waypoints;
    public List<Vector2> torchPositions;

	public Cave(Rect rect)
	{
		this.position = new Vector2i((int)rect.x, (int)rect.y);
        this.width = (int)rect.width;
        this.height = (int)rect.height;
        waypoints = new List<Vector2i>();
        torchPositions = new List<Vector2>();
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

    public float getExitRotation()
    {
        if (waypoints.Count < 2) return 0;
        Vector2 pos1 = entrancePosition.toVector2();
        Vector2 pos2 = doorMatPosition;

        Vector2 dir = pos2 - pos1;
        float rot = Mathf.Atan2(dir.x, dir.y);
        return rot;
    }

}
