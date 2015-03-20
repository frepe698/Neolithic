using UnityEngine;
using System.Collections;

public abstract class Command {

	private bool completed = false;
	protected static readonly float moveSensitivity = 0.1f;  
	protected Vector2 destination;

	protected Unit unit;


	public Command (Unit unit)
	{
		this.unit = unit;
	}

	public abstract void start();
	public abstract void update();
	

	public bool isCompleted()
	{
		return completed;
	}

	public void setCompleted()
	{
		completed = true;
	}

	public Vector2 getDestination()
	{
		return destination;
	}

	public void setDestination(Vector2 destination)
	{
		this.destination = destination;
	}

    public virtual bool canBeOverridden()
    {
        return true;
    }

    public virtual bool canAlwaysStart()
    {
        return false;
    }

    public virtual bool canStartOverride(Command command)
    {
        return !this.Equals(command);
    }

}
