using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Cave  {

	public readonly int MARGIN = 10;
    public readonly Vector2i position;
    public readonly int width, height;

	private List<Vector2i> entrancePositions;
    public List<Vector2> doorMatPositions;
	private Vector2i bossPosition;

    public List<Vector2i> waypoints;
    public List<Vector2> torchPositions;

	public Cave(Rect rect)
	{
		this.position = new Vector2i((int)rect.x, (int)rect.y);
        this.width = (int)rect.width;
        this.height = (int)rect.height;
        entrancePositions = new List<Vector2i>();
        doorMatPositions = new List<Vector2>();
        waypoints = new List<Vector2i>();
        torchPositions = new List<Vector2>();
	}

	

    public List<Vector2i> entrancePoses
    {
        get{return entrancePositions;}
    }

    public Vector2i mainEntrance
    {
        get { return entrancePositions[0]; }
        set
        {
            if (entrancePositions.Count < 1)
                entrancePositions.Add(value);
            else
                entrancePositions[0] = value;
        }
    }

    public Vector2 mainDoormat
    {
        get { return doorMatPositions[0]; }
        set
        {
            if (doorMatPositions.Count < 1)
                doorMatPositions.Add(value);
            else
                doorMatPositions[0] = value;
        }
    }

    public void addEntrancePos(Vector2i entrancePos)
    {
        addEntrancePos(entrancePos, entrancePos.toVector2());
    }

    public void addEntrancePos(Vector2i entrancePos, Vector2 doormatPos)
    {
        entrancePositions.Add(entrancePos);
        doorMatPositions.Add(doormatPos);
    }

    public void setDoormatPos(int index, Vector2 pos)
    {
        doorMatPositions[index] = pos;
    }

    public Vector2i getEntrancePos(int index)
    {
        return entrancePositions[index];
    }

    public Vector2i bossPos
    {
        get{return bossPosition;}
        set{bossPosition = value;}
    }

    public float getExitRotation(int entrance = 0)
    {
        //if (waypoints.Count < 2) return 0;
        Vector2 pos1 = entrancePositions[entrance].toVector2();
        Vector2 pos2 = doorMatPositions[entrance];

        Vector2 dir = pos2 - pos1;
        float rot = Mathf.Atan2(dir.x, dir.y);
        return rot;
    }

}
