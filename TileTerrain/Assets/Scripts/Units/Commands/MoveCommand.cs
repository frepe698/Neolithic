using UnityEngine;
using System.Collections;

public class MoveCommand : Command {


	public MoveCommand(Unit unit, Vector2 destination) : base(unit)
	{
		this.destination = destination;
	}

	public override void start ()
	{
		actor.setPath(destination);
	}

	public override void update()
	{
		if(Vector2.Distance(actor.get2DPos(), destination) < Command.moveSensitivity)
		{
			setCompleted();
		}
	}

    public override bool canAlmostAlwaysStart()
    {
        return true;
    }

    public override bool canStartOverride(Command command)
    {
        return command == null || command.canBeOverridden();
    }

    public override string getName()
    {
        return "move";
    }
}
