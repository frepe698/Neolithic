using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathAIUnit : AIUnit
{
    private List<WorldPoint> waypoints;
    private WorldPoint destination;

    private float returnCounter = 0;
    private readonly int RETURN_NOW = 3;

    public PathAIUnit(string unit, Vector3 position, Vector3 rotation, int id, Road road) : base(unit, position, rotation, id)
    {
        this.waypoints = road.getWaypoints();
        destination = popLastWaypoint();
    }

    public PathAIUnit(string unit, Vector3 position, Vector3 rotation, int id, List<WorldPoint> waypoints)
        : base(unit, position, rotation, id)
    {
        this.waypoints = new List<WorldPoint>(waypoints);
        if (this.waypoints.Count > 0) popLastWaypoint();
        
        destination = popLastWaypoint();
        GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
    }

    public override void updateAI()
    {
        if (Vector2i.getDistance(new Vector2i(getPosition()), destination.get2D()) < 4 && waypoints.Count > 0)
        {
            destination = popLastWaypoint();
            
            GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            returnCounter = 0;
        }

        if(command == null || !(command is MoveCommand) )
        {
            if (returnCounter >= RETURN_NOW)
                GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            else
                returnCounter += Time.deltaTime;
        }
        
        
       //base.updateAI();
    }

    public override void setAwake(bool awake)
    {
        base.setAwake(true);
    }

    private WorldPoint popLastWaypoint()
    {
        if (waypoints.Count <= 0)
        {
            Debug.Log("No mode checkpoints");
            return null;
        }
        int index = waypoints.Count - 1;
        WorldPoint point = waypoints[index];
        waypoints.RemoveAt(index);
        return point;
    }
}
