using UnityEngine;
using System.Collections;

public class BuildingBuildCommand : Command {

    private float buildTime;
    private float buildProgress;

    private float offset = 1;

    private Vector3 endPosition;

    public BuildingBuildCommand(Building building, float buildTime)
        : base(building)
    {
        this.buildTime = buildTime;
        buildProgress = 0.0f;

        endPosition = building.getPosition();
        building.setPosition(endPosition - new Vector3(0, offset, 0));
    }

    public override void start()
    {
        
    }

    public override void update()
    {
        buildProgress += Time.deltaTime;

        if(buildProgress >= buildTime)
        {
            actor.setPosition(endPosition);
            setCompleted();
        }
        else
        {
            actor.setPosition(endPosition - new Vector3(0, offset * (1-(buildProgress/buildTime)), 0));
        }
    }
}
