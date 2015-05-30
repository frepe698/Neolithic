using UnityEngine;
using System.Collections;

public class BuildingCommand : Command {

	private Building building;

    public BuildingCommand (Unit unit, Building building)
        : base(unit)
	{
        this.building = building;
        this.destination = building.get2DPos();
	}

    public override void start()
    {
        actor.setPath(destination);
    }

    public override void update()
    {
        if (Vector2.Distance(actor.get2DPos(), destination) < 2)
        {
            actor.setMoving(false);
            if (building != null)
            {
                if (actor.getID() == GameMaster.getPlayerUnitID())
                {
                    GameMaster.getGUIManager().selectedBuilding(building);
                }
            }
            setCompleted();
        }
    }

    public override string getName()
    {
        return "building";
    }
}
