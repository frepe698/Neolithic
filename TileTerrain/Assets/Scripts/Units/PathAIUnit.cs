using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathAIUnit : AIUnit
{
    private List<WorldPoint> waypoints;
    private Vector2 destination;

    private Vector2i roadPos;
    private bool followingRoad;

    private const int RETURN_DISTANCE = 10;

    private float returnCounter = 0;
    private readonly int RETURN_NOW = 3;

    public PathAIUnit(string unit, Vector3 position, Vector3 rotation, int id, List<WorldPoint> waypoints)
        : base(unit, position, rotation, id)
    {
        this.waypoints = new List<WorldPoint>(waypoints);
        if (this.waypoints.Count > 0) popLastWaypoint();
        
        destination = popLastWaypoint();
        GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
        justGotCommandTimer = 0.2f;
        followingRoad = false;
        roadPos = getTile();
    }

    public override void updateAI()
    {
        returnCounter += Time.deltaTime;
        if (!followingRoad && Vector2i.getManhattan(roadPos, getTile()) > RETURN_DISTANCE)
        {
            followingRoad = true;
            returnCounter = 0;
            GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            justGotCommandTimer = 0.2f;
        }
        justGotCommandTimer -= Time.deltaTime;
        if (returnCounter > RETURN_NOW && (command == null || command is MoveCommand) && justGotCommandTimer < 0)
        {
            Unit closestUnit = findClosestEnemy(lineOfSight);
            if (fleeOrFight(closestUnit))
            {
                roadPos = getTile();
                followingRoad = false;
            }
            else if(!followingRoad)
            {
                followingRoad = true;
                returnCounter = 0;
                GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
                justGotCommandTimer = 0.2f;
            }
        }

        if (followingRoad && Vector2.Distance(get2DPos(), destination) < 1 && waypoints.Count > 0)
        {
            destination = popLastWaypoint();
            
            GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            justGotCommandTimer = 0.2f;
        }
    }

    public override void setAwake(bool awake)
    {
        base.setAwake(true);
    }

    private Vector2 popLastWaypoint()
    {
        if (waypoints.Count <= 0)
        {
            Debug.Log("No mode checkpoints");
            return Vector2.zero;
        }
        int index = waypoints.Count - 1;
        WorldPoint point = waypoints[index];
        waypoints.RemoveAt(index);
        return point.get2D().toVector2();
    }
}
