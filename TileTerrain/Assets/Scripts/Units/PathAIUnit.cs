﻿using UnityEngine;
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
        justGotCommandTimer -= Time.deltaTime;
        if (!followingRoad && Vector2i.getManhattan(roadPos, getTile()) > RETURN_DISTANCE)
        {
            followingRoad = true;
            returnCounter = 0;
            findNextWaypoint();
            GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            justGotCommandTimer = 0.2f;
        }
        
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
        else if(followingRoad && command == null)
        {
            GameMaster.getGameController().requestMoveCommand(getID(), destination.x, destination.y);
            justGotCommandTimer = 0.2f;
        }
    }

    public override void setAwake(bool awake)
    {
        base.setAwake(true);
    }

    private void findNextWaypoint()
    {
        if (waypoints.Count == 0) return;
        List<WorldPoint> tempPoints = new List<WorldPoint>(waypoints);
        Vector2i deltaGoal = tempPoints[0].get2D() - getTile();
        int removedCounter = 0;
        for(int i = tempPoints.Count - 1; i >= 0; i--)
        {
            Vector2i deltaPoint = tempPoints[i].get2D() - getTile();
            if(deltaGoal.x * deltaPoint.x < 0 || deltaGoal.y * deltaPoint.y < 0) //point is in wrong direction from goal
            {
                destination = popLastWaypoint();
                removedCounter++;
            }
            else //If not we have a point in right direction and can return
            {
                Debug.Log("Removed: " + removedCounter);

                return;
            }
        }
        destination = tempPoints[0].get2D().toVector2();
        Debug.Log("Removed all");

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