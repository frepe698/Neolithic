using UnityEngine;
using System.Collections;

public class MoveCommand : Command {


	public MoveCommand(Unit unit, Vector2 destination) : base(unit)
	{
		this.destination = destination;
	}

	public override void start ()
	{
		unit.setPath(destination);
	}

	public override void update()
	{
		if(Vector2.Distance(unit.get2DPos(), destination) < Command.moveSensitivity)
		{
			setCompleted();
		}
	}

    public override bool canAlwaysStart()
    {
        return true;
    }


}
