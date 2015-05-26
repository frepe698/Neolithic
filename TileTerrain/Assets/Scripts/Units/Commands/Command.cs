﻿using UnityEngine;
using System.Collections;

public abstract class Command {

	private bool completed = false;
	protected static readonly float moveSensitivity = 0.1f;  
	protected Vector2 destination;

	protected Actor actor;


	public Command (Actor actor)
	{
		this.actor = actor;
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

    public virtual bool canAlmostAlwaysStart()
    {
        return false;
    }

    public virtual bool canAlwaysStart()
    {
        return false;
    }

    public virtual bool canStartOverride(Command command)
    {
        return !this.Equals(command);
    }

    public virtual string getName()
    {
        return "command";
    }


    public virtual void cancel()
    {
        completed = true;
    }

}
